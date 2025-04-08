using VoxelMeshOptimizer.Core;
using VoxelMeshOptimizer.Core.OcclusionAlgorithms;
using VoxelMeshOptimizer.Core.OcclusionAlgorithms.Common;
using VoxelMeshOptimizer.Tests.DummyClasses;
using Xunit;
using Xunit.Abstractions;

namespace VoxelMeshOptimizer.Tests.Occlusion
{
    public class VoxelOcclusionOptimizerTests
    {
        private readonly ITestOutputHelper output;

        public VoxelOcclusionOptimizerTests(ITestOutputHelper output)
        {
            this.output = output;
        }

        [Fact]
        public void SingleVoxel_ShouldProduceAllSixPlanes_IfNotSkippingEmptyOnes()
        {
            // Arrange
            var chunk = new TestChunk(1, 1, 1);

            // Only one voxel => set it to solid
            chunk.ForEachCoordinate(
                Axis.X, AxisOrder.Ascending,
                Axis.Y, AxisOrder.Ascending,
                Axis.Z, AxisOrder.Ascending,
                (x,y,z) => 
                {
                    // pos.X, pos.Y, pos.Z are all zero
                    chunk.Set(x,y,z, new TestVoxel(id: 1, isSolid: true));
                }
            );

            var optimizer = new VoxelOcclusionOptimizer(chunk);

            // Act
            VisibleFaces faces = optimizer.ComputeVisiblePlanes();

            // Assert
            // We expect 6 axes (FrontToBack, BackToFront, etc.), each with 1 plane
            Assert.Equal(6, faces.PlanesByAxis.Count);

            // For each axis, we have a single 1×1 plane containing our one voxel
            foreach (var kvp in faces.PlanesByAxis)
            {
                var axis = kvp.Key; // e.g. HumanAxis.FrontToBack
                var planeList = kvp.Value;

                // We expect exactly one slice for a 1×1×1 chunk
                Assert.Single(planeList);

                var plane = planeList[0];
                Assert.NotNull(plane);

                // The plane's 'MinorAxis' typically matches 'axis' if your code 
                // sets it that way. If you want, you can assert that 
                // plane.MinorAxis == axis, or plane.SliceIndex == 0, etc.
                Assert.Equal((uint)0, plane.SliceIndex);

                // In a 1x1x1 chunk, plane dimensions are always 1×1
                Assert.Equal(1, plane.Voxels.GetLength(0));
                Assert.Equal(1, plane.Voxels.GetLength(1));

                // We expect the single voxel to be non-null
                Assert.NotNull(plane.Voxels[0,0]);
            }
        }

        // [Fact]
        // public void Solid2x2x2Chunk_OnlyOuterPlanesShouldHaveVoxels()
        // {
        //     // Arrange
        //     var chunk = new TestChunk(2, 2, 2);

        //     // Fill entire chunk with solid voxels
        //     chunk.ForEachCoordinate(
        //         Axis.X, AxisOrder.Ascending,   // major
        //         Axis.Z, AxisOrder.Ascending,   // middle
        //         Axis.Y, AxisOrder.Ascending,   // minor
        //         (x,y,z) =>
        //         {
        //             chunk.Set(x,y,z, new TestVoxel(id: 1, isSolid: true));
        //         }
        //     );

        //     var optimizer = new VoxelOcclusionOptimizer(chunk);

        //     // Act
        //     VisibleFaces faces = optimizer.ComputeVisiblePlanes();

        //     // Assert
        //     // For each axis, we expect exactly ONE visible plane 
        //     // (the outer boundary).
        //     // For instance, HumanAxis.FrontToBack => a single slice at z=1,
        //     // but your code might unify them as "sliceIndex=1" or "sliceIndex=0" 
        //     // depending on how you skip empties.

        //     var frontPlanes = faces.PlanesByAxis[HumanAxis.FrontToBack];
        //     Assert.Single(frontPlanes);
        //     AssertPlaneNotEmpty(frontPlanes[0], 4);

        //     var backPlanes = faces.PlanesByAxis[HumanAxis.BackToFront];
        //     Assert.Single(backPlanes);
        //     AssertPlaneNotEmpty(backPlanes[0], 4);

        //     // Similarly for LeftToRight, RightToLeft, BottomToTop, TopToBottom
        // }

        [Fact]
        public void PartiallyEmpty2x2Chunk_ShouldHaveMultipleVisiblePlanesInside()
        {
            // Arrange
            var chunk = new TestChunk(2, 2, 2);

            // Only fill "front" row at z=1, leaving z=0 empty
            
            // Fill entire chunk with solid voxels
            chunk.ForEachCoordinate(
                Axis.X, AxisOrder.Ascending,   // major
                Axis.Z, AxisOrder.Ascending,   // middle
                Axis.Y, AxisOrder.Ascending,   // minor
                (x,y,z) =>
                {
                    if (z == 1)
                    {
                        chunk.Set(x,y,z, new TestVoxel(id: 42, isSolid: true));
                    }
                }
            );

            var optimizer = new VoxelOcclusionOptimizer(chunk);

            // Act
            var visibleFaces = optimizer.ComputeVisiblePlanes();

            // Assert
            var xAsc = visibleFaces.PlanesByAxis[(Axis.X, AxisOrder.Ascending)];
            Assert.Single(xAsc);
            AssertPlaneNotEmpty(xAsc[0], 4);
            var xDesc = visibleFaces.PlanesByAxis[(Axis.X, AxisOrder.Descending)];
            Assert.Single(xDesc);
            AssertPlaneNotEmpty(xDesc[0], 4);


            var yAsc = visibleFaces.PlanesByAxis[(Axis.Y, AxisOrder.Ascending)];
            Assert.Single(yAsc);
            AssertPlaneNotEmpty(yAsc[0], 2);
            var yDesc = visibleFaces.PlanesByAxis[(Axis.Y, AxisOrder.Descending)];
            Assert.Single(yDesc);
            AssertPlaneNotEmpty(yDesc[0], 2);


            var zAsc = visibleFaces.PlanesByAxis[(Axis.Z, AxisOrder.Ascending)];
            Assert.Single(zAsc);
            AssertPlaneNotEmpty(zAsc[0], 2);
            var zDesc = visibleFaces.PlanesByAxis[(Axis.Z, AxisOrder.Descending)];
            Assert.Single(zDesc);
            AssertPlaneNotEmpty(zDesc[0], 2);
        }

        // [Fact]
        // public void AlmostSolid2x2x2_OneMissingVoxel_ShouldRevealInternalFaces()
        // {
        //     // Arrange
        //     // Fill every voxel with a solid one, except (1,1,1) is air.
        //     var chunk = new TestChunk(2, 2, 2);

        //     chunk.ForEachCoordinate(
        //         Axis.X, AxisOrder.Ascending,   // major
        //         Axis.Z, AxisOrder.Ascending,   // middle
        //         Axis.Y, AxisOrder.Ascending,   // minor
        //         (x,y,z) =>
        //         {
        //             bool isMissing = x == 1 && y == 1 && z == 1;
        //             chunk.Set(x,y,z, new TestVoxel(
        //                 id: (ushort)((x+1)*100 + (y+1)*10 + (z+1)),
        //                 isSolid: !isMissing
        //             ));
        //         }
        //     );

        //     var optimizer = new VoxelOcclusionOptimizer(chunk);

        //     // Act
        //     VisibleFaces faces = optimizer.ComputeVisiblePlanes();

        //     // Debug: see what's in the front planes
        //     var frontPlanes = faces.PlanesByAxis[HumanAxis.FrontToBack];
        //     foreach (var plane in frontPlanes)
        //     {
        //         output.WriteLine("plane :");
        //         output.WriteLine(plane.Describe());
        //     }

        //     // Assert
        //     // Check outer boundary plane for front => z=1 
        //     Assert.Single(frontPlanes);
        //     Assert.Equal((uint)1, frontPlanes[0].SliceIndex);
        //     AssertPlaneNotEmpty(frontPlanes[0], expectedCount: 3);

        //     // And similarly for back, left, right, top, bottom
        //     var backPlanes = faces.PlanesByAxis[HumanAxis.BackToFront];
        //     Assert.Single(backPlanes);
        //     Assert.Equal((uint)0, backPlanes[0].SliceIndex);
        //     AssertPlaneNotEmpty(backPlanes[0], 4);

        //     var rightPlanes = faces.PlanesByAxis[HumanAxis.RightToLeft];
        //     Assert.Single(rightPlanes);
        //     Assert.Equal((uint)1, rightPlanes[0].SliceIndex);
        //     AssertPlaneNotEmpty(rightPlanes[0], 3);

        //     var leftPlanes = faces.PlanesByAxis[HumanAxis.LeftToRight];
        //     Assert.Single(leftPlanes);
        //     Assert.Equal((uint)0, leftPlanes[0].SliceIndex);
        //     AssertPlaneNotEmpty(leftPlanes[0], 4);

        //     var topPlanes = faces.PlanesByAxis[HumanAxis.TopToBottom];
        //     Assert.Single(topPlanes);
        //     Assert.Equal((uint)1, topPlanes[0].SliceIndex);
        //     AssertPlaneNotEmpty(topPlanes[0], 3);

        //     var bottomPlanes = faces.PlanesByAxis[HumanAxis.BottomToTop];
        //     Assert.Single(bottomPlanes);
        //     Assert.Equal((uint)0, bottomPlanes[0].SliceIndex);
        //     AssertPlaneNotEmpty(bottomPlanes[0], 4);
        // }

        // Helper method:
        static void AssertPlaneNotEmpty(VisiblePlane plane, int expectedCount)
        {
            int actualCount = 0;
            var w = plane.Voxels.GetLength(0);
            var h = plane.Voxels.GetLength(1);
            for (int i = 0; i < w; i++)
            {
                for (int j = 0; j < h; j++)
                {
                    if (plane.Voxels[i, j] != null) actualCount++;
                }
            }
            Assert.Equal(expectedCount, actualCount);
        }
    }
}
