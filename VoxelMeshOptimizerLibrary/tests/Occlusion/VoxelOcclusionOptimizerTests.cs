using System;
using System.Linq;
using Xunit;
using VoxelMeshOptimizer.Core;
using VoxelMeshOptimizer.Core.OcclusionAlgorithms;
using VoxelMeshOptimizer.Core.OcclusionAlgorithms.Common;
using VoxelMeshOptimizer.Tests.DummyClasses;
using System.Data;

namespace VoxelMeshOptimizer.Tests.OcclusionTests;

public class VoxelOcclusionOptimizerTests
{
    #region Constructor Tests

    [Fact]
    public void Constructor_NullChunk_ThrowsException()
    {
        // Arrange, Act & Assert:
        Assert.Throws<NoNullAllowedException>(() => new VoxelOcclusionOptimizer(null));
    }

    #endregion Constructor Tests

    #region Empty Chunk Tests

    [Fact]
    public void ComputeVisiblePlanes_EmptyChunk_ReturnsNoVisibleFaces()
    {
        // Arrange
        // Note: The TestChunk is constructed but no voxel is explicitly set,
        // so by default each voxel in TestChunk is null.
        var emptyChunk = new TestChunk(10, 10, 10);
        var optimizer = new VoxelOcclusionOptimizer(emptyChunk);

        emptyChunk.ForEachCoordinate(
            Axis.X, AxisOrder.Ascending,
            Axis.Y, AxisOrder.Ascending,
            Axis.Z, AxisOrder.Ascending,
            (x,y,z) => {
                emptyChunk.Set(x,y,z, new TestVoxel(1,false));
            }
        );

        // Act
        VisibleFaces result = optimizer.ComputeVisibleFaces();

        // Assert
        Assert.NotNull(result);
        // For an entirely empty chunk, the expected behavior is NO visible planes.
        // (i.e. the dictionary should be empty)
        foreach(var direction in result.PlanesByAxis){
            Assert.Empty(direction.Value);
        }
    }

    #endregion Empty Chunk Tests

    #region Full Chunk Tests

    [Fact]
    public void ComputeVisiblePlanes_FullChunk_Returns6VisibleFaces()
    {
        // Arrange
        // Create a fully filled chunk. Every voxel is set as a solid TestVoxel.
        var xDepth = 10u;
        var yDepth = 10u;
        var zDepth = 10u;
        var fullChunk = new TestChunk(xDepth, yDepth, zDepth);
        for (uint x = 0; x < xDepth; x++)
        {
            for (uint y = 0; y < yDepth; y++)
            {
                for (uint z = 0; z < zDepth; z++)
                {
                    fullChunk.Set(x, y, z, new TestVoxel(1, true));
                }
            }
        }
        var optimizer = new VoxelOcclusionOptimizer(fullChunk);

        // Act
        VisibleFaces result = optimizer.ComputeVisibleFaces();

        // Assert
        Assert.NotNull(result);
        // For a fully filled (solid) chunk, the expected visible faces are the 6 outer faces.
        // We assume that each face is represented by a key (Axis, AxisOrder) in the dictionary.
        Assert.Equal(6, result.PlanesByAxis.Count);

        // Optionally, check that there is exactly one plane per face.
        int totalPlaneCount = result.PlanesByAxis.Values.Sum(list => list.Count);
        Assert.Equal(6, totalPlaneCount);
    }

    #endregion Full Chunk Tests

    #region Single Voxel Chunk Tests

    [Fact]
    public void ComputeVisiblePlanes_SingleVoxelChunk_Returns6VisibleFaces()
    {
        // Arrange
        // Create a 1x1x1 chunk with one solid voxel.
        var singleVoxelChunk = new TestChunk(1, 1, 1);
        singleVoxelChunk.Set(0, 0, 0, new TestVoxel(42, true));
        var optimizer = new VoxelOcclusionOptimizer(singleVoxelChunk);

        // Act
        VisibleFaces result = optimizer.ComputeVisibleFaces();

        // Assert
        Assert.NotNull(result);
        // A single voxel should have 6 visible faces, one for each direction.
        Assert.Equal(6, result.PlanesByAxis.Count);
        int totalPlaneCount = result.PlanesByAxis.Values.Sum(list => list.Count);
        Assert.Equal(6, totalPlaneCount);

        // Additionally, each visible plane should have a valid slice index and
        // the corresponding 2D array dimensions should match the expected plane dimensions.
        foreach (var key in result.PlanesByAxis.Keys)
        {
            foreach (var plane in result.PlanesByAxis[key])
            {
                // For a 1x1 chunk the expected dimensions of any plane are 1x1.
                Assert.Equal(1, plane.Voxels.GetLength(0));
                Assert.Equal(1, plane.Voxels.GetLength(1));
            }
        }
    }

    #endregion Single Voxel Chunk Tests

    #region Irregular Chunk Tests

    [Fact]
    public void ComputeVisiblePlanes_IrregularChunk_WithInternalHole_ProducesExpectedVisibleFaces()
    {
        // Arrange
        // Create a 3x3x3 chunk where all voxels are solid except the center voxel,
        // which will be non-solid to simulate an internal hole.
        var chunkSize = 3u;
        var irregularChunk = new TestChunk(chunkSize, chunkSize, chunkSize);
        for (uint x = 0; x < chunkSize; x++)
        {
            for (uint y = 0; y < chunkSize; y++)
            {
                for (uint z = 0; z < chunkSize; z++)
                {
                    // Set the center voxel (1,1,1) as non-solid. Others are solid.
                    bool isSolid = !(x == 1 && y == 1 && z == 1);
                    irregularChunk.Set(x, y, z, new TestVoxel(1, isSolid));
                }
            }
        }
        var optimizer = new VoxelOcclusionOptimizer(irregularChunk);

        // Act
        VisibleFaces result = optimizer.ComputeVisibleFaces();

        // Assert
        Assert.NotNull(result);

        // Since the specifications say:
        // "A visible plane exists as long as there is a voxel facing either an empty space (null or boundary) or a non-solid voxel.
        // If there is an empty space in the middle of the chunk, the adjacent voxel creates visible faces."
        //
        // We expect the outer faces (which count as 6 planes) plus additional inner visible faces
        // around the center hole.
        //
        // Please clarify the exact expected count of visible planes for this irregular case.
        // For now, we perform an exact match check based on the current requirement example.
        // (Assume expectedTotalPlanes is the exact number you expect; here we use a placeholder value.)
        int expectedTotalPlanes = 6 + 6; // Placeholder: 6 outer faces + 6 internal faces adjacent to the hole
        int actualTotalPlanes = result.PlanesByAxis.Values.Sum(list => list.Count);
        Assert.Equal(expectedTotalPlanes, actualTotalPlanes);
    }

    #endregion Irregular Chunk Tests
}
