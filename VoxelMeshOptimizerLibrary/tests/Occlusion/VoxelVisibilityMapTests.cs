using VoxelMeshOptimizer.Core;
using VoxelMeshOptimizer.Tests.DummyClasses;
using Xunit;

namespace VoxelMeshOptimizer.Tests.Occlusion;

public class VoxelVisibilityMapTests
{
    [Fact]
    public void SolidChunk_AllOuterFacesShouldBeVisible_InnerFacesNotVisible()
    {
        // Arrange
        var chunk = new TestChunk(2, 2, 2);
        
        // Fill entire chunk with solid voxels
        for (uint x = 0; x < 2; x++)
        {
            for (uint y = 0; y < 2; y++)
            {
                for (uint z = 0; z < 2; z++)
                {
                    chunk.Set(x, y, z, new TestVoxel(id: 1, isSolid: true));
                }
            }
        }

        // Act
        var visibilityMap = new VoxelVisibilityMap(chunk);

        // Assert
        // For each voxel, check which faces are visible.
        // Because the chunk is fully solid, only boundary voxels have visible faces 
        // (those that are on the outside). 
        // Inner faces between two adjacent voxels are NOT visible.

        // Let’s check corners, edges, etc.
        // Corner voxel at (0,0,0) => it's on the "left, bottom, back" corner 
        // so it should be visible on Left, Bottom, Back faces
        var faces000 = visibilityMap.GetVisibleFaces(0,0,0);
        Assert.True(faces000.HasFlag(VoxelFace.Left));
        Assert.True(faces000.HasFlag(VoxelFace.Bottom));
        Assert.True(faces000.HasFlag(VoxelFace.Back));
        Assert.False(faces000.HasFlag(VoxelFace.Right));
        Assert.False(faces000.HasFlag(VoxelFace.Top));
        Assert.False(faces000.HasFlag(VoxelFace.Front));

        // Corner voxel at (1,1,1) => "right, top, front" corner
        var faces111 = visibilityMap.GetVisibleFaces(1,1,1);
        Assert.True(faces111.HasFlag(VoxelFace.Right));
        Assert.True(faces111.HasFlag(VoxelFace.Top));
        Assert.True(faces111.HasFlag(VoxelFace.Front));
        Assert.False(faces111.HasFlag(VoxelFace.Left));
        Assert.False(faces111.HasFlag(VoxelFace.Bottom));
        Assert.False(faces111.HasFlag(VoxelFace.Back));

        // The voxel at (0,0,1), for example, is on the front but also left/bottom edges:
        // left, bottom, front
        var faces001 = visibilityMap.GetVisibleFaces(0,0,1);
        Assert.True(faces001.HasFlag(VoxelFace.Left));
        Assert.True(faces001.HasFlag(VoxelFace.Bottom));
        Assert.True(faces001.HasFlag(VoxelFace.Front));
        Assert.False(faces001.HasFlag(VoxelFace.Back));
        Assert.False(faces001.HasFlag(VoxelFace.Right));
        Assert.False(faces001.HasFlag(VoxelFace.Top));

        // The "inner face" between (0,0,0) and (1,0,0) is not visible, so:
        var faces100 = visibilityMap.GetVisibleFaces(1,0,0);
        // It should NOT have Left face visible (because there's a solid voxel at (0,0,0)).
        Assert.False(faces100.HasFlag(VoxelFace.Left));
    }


    [Fact]
    public void SingleVoxel_AllSixFacesShouldBeVisible()
    {
        // Arrange
        var chunk = new TestChunk(1, 1, 1);
        chunk.Set(0, 0, 0, new TestVoxel(id: 99, isSolid: true));

        // Act
        var visibilityMap = new VoxelVisibilityMap(chunk);

        // Assert
        var faces = visibilityMap.GetVisibleFaces(0, 0, 0);

        // If there's only one voxel in the entire chunk, it's exposed on all sides
        Assert.True(faces.HasFlag(VoxelFace.Front));
        Assert.True(faces.HasFlag(VoxelFace.Back));
        Assert.True(faces.HasFlag(VoxelFace.Left));
        Assert.True(faces.HasFlag(VoxelFace.Right));
        Assert.True(faces.HasFlag(VoxelFace.Top));
        Assert.True(faces.HasFlag(VoxelFace.Bottom));
    }

    [Fact]
    public void EmptyChunk_NoVoxelsNoVisibleFaces()
    {
        // Arrange
        var chunk = new TestChunk(2, 2, 2);
        // no voxels set => they are all null

        // Act
        var visibilityMap = new VoxelVisibilityMap(chunk);

        // Assert
        for (uint x = 0; x < 2; x++)
        {
            for (uint y = 0; y < 2; y++)
            {
                for (uint z = 0; z < 2; z++)
                {
                    var faces = visibilityMap.GetVisibleFaces(x, y, z);
                    Assert.Equal(VoxelFace.None, faces);
                }
            }
        }
    }

    [Fact]
    public void MixedSolidAndNull_CheckTransitions()
    {
        // Arrange
        var chunk = new TestChunk(2, 2, 2);

        // Place a solid voxel in one corner, empty in others.
        chunk.Set(0, 0, 0, new TestVoxel(id: 1, isSolid: true));
        // Let (0,0,1), (0,1,0), (0,1,1), etc. remain null => air
        // so that (1,0,0) => also air, etc.

        // Act
        var visibilityMap = new VoxelVisibilityMap(chunk);

        // Assert
        // The only solid voxel is at (0,0,0). 
        // Because all adjacent positions are "air", it should have all 6 faces visible.
        var faces = visibilityMap.GetVisibleFaces(0,0,0);
        Assert.Equal(
            VoxelFace.Front | VoxelFace.Back | VoxelFace.Left |
            VoxelFace.Right | VoxelFace.Top   | VoxelFace.Bottom, 
            faces
        );

        // All other coordinates are null => no voxel => faces = None
        var faces100 = visibilityMap.GetVisibleFaces(1,0,0);
        Assert.Equal(VoxelFace.None, faces100);
    }


    [Fact]
    public void CheckErrorHandling_OutOfRangeShouldReturnNone()
    {
        // Arrange
        var chunk = new TestChunk(1, 1, 1);
        chunk.Set(0, 0, 0, new TestVoxel(id: 123, isSolid: true));
        var visibilityMap = new VoxelVisibilityMap(chunk);

        // Act
        // Query something out of range
        var result = visibilityMap.GetVisibleFaces(99, 99, 99);

        // Assert
        Assert.Equal(VoxelFace.None, result);
    }
}
