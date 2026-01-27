using VoxelMeshOptimizer.Core;
using VoxelMeshOptimizer.Tests.Helpers;

namespace VoxelMeshOptimizer.Tests.Occlusion;

public class VoxelVisibilityMapTests
{
    [Fact]
    public void SolidChunk_AllOuterFacesShouldBeVisible_InnerFacesNotVisible()
    {
        // Arrange
        Chunk chunk = ChunkGen.GenerateChunk(2, 2, 2);
        
        // Fill entire chunk with solid voxels
        for (uint x = 0; x < 2; x++)
        {
            for (uint y = 0; y < 2; y++)
            {
                for (uint z = 0; z < 2; z++)
                {
                    chunk.Set(x, y, z, new Voxel(id: 1));
                }
            }
        }

        // Act
        VoxelVisibilityMap visibilityMap = new(chunk);

        // Assert
        // For each voxel, check which faces are visible.
        // Because the chunk is fully solid, only boundary voxels have visible faces 
        // (those that are on the outside). 
        // Inner faces between two adjacent voxels are NOT visible.

        // Let’s check corners, edges, etc.
        // Corner voxel at (0,0,0) => it's on the "left, bottom, back" corner 
        // so it should be visible on Left, Bottom, Back faces
        VoxelFace faces000 = visibilityMap.GetVisibleFaces(0,0,0);
        Assert.True(faces000.HasFlag(VoxelFace.Xneg));
        Assert.True(faces000.HasFlag(VoxelFace.Yneg));
        Assert.True(faces000.HasFlag(VoxelFace.Zneg));
        Assert.False(faces000.HasFlag(VoxelFace.Xpos));
        Assert.False(faces000.HasFlag(VoxelFace.Ypos));
        Assert.False(faces000.HasFlag(VoxelFace.Zpos));

        // Corner voxel at (1,1,1) => "right, top, front" corner
        VoxelFace faces111 = visibilityMap.GetVisibleFaces(1,1,1);
        Assert.True(faces111.HasFlag(VoxelFace.Xpos));
        Assert.True(faces111.HasFlag(VoxelFace.Ypos));
        Assert.True(faces111.HasFlag(VoxelFace.Zpos));
        Assert.False(faces111.HasFlag(VoxelFace.Xneg));
        Assert.False(faces111.HasFlag(VoxelFace.Yneg));
        Assert.False(faces111.HasFlag(VoxelFace.Zneg));

        // The voxel at (0,0,1), for example, is on the front but also left/bottom edges:
        // left, bottom, front
        VoxelFace faces001 = visibilityMap.GetVisibleFaces(0,0,1);
        Assert.True(faces001.HasFlag(VoxelFace.Xneg));
        Assert.True(faces001.HasFlag(VoxelFace.Yneg));
        Assert.True(faces001.HasFlag(VoxelFace.Zpos));
        Assert.False(faces001.HasFlag(VoxelFace.Zneg));
        Assert.False(faces001.HasFlag(VoxelFace.Xpos));
        Assert.False(faces001.HasFlag(VoxelFace.Ypos));

        // The "inner face" between (0,0,0) and (1,0,0) is not visible, so:
        VoxelFace faces100 = visibilityMap.GetVisibleFaces(1,0,0);
        // It should NOT have Left face visible (because there's a solid voxel at (0,0,0)).
        Assert.False(faces100.HasFlag(VoxelFace.Xneg));
    }


    [Fact]
    public void SingleVoxel_AllSixFacesShouldBeVisible()
    {
        // Arrange
        Chunk chunk = ChunkGen.GenerateChunk(1, 1, 1);
        chunk.Set(0, 0, 0, new Voxel(id: 99));

        // Act
        VoxelVisibilityMap visibilityMap = new(chunk);

        // Assert
        VoxelFace faces = visibilityMap.GetVisibleFaces(0, 0, 0);

        // If there's only one voxel in the entire chunk, it's exposed on all sides
        Assert.True(faces.HasFlag(VoxelFace.Zpos));
        Assert.True(faces.HasFlag(VoxelFace.Zneg));
        Assert.True(faces.HasFlag(VoxelFace.Xneg));
        Assert.True(faces.HasFlag(VoxelFace.Xpos));
        Assert.True(faces.HasFlag(VoxelFace.Ypos));
        Assert.True(faces.HasFlag(VoxelFace.Yneg));
    }

    [Fact]
    public void EmptyChunk_NoVoxelsNoVisibleFaces()
    {
        // Arrange
        Chunk chunk = ChunkGen.GenerateChunk(2, 2, 2);
        // no voxels set => they are all null

        // Act
        VoxelVisibilityMap visibilityMap = new(chunk);

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
        Chunk chunk = ChunkGen.GenerateChunk(2, 2, 2);

        // Place a solid voxel in one corner, empty in others.
        chunk.Set(0, 0, 0, new Voxel(id: 1));
        // Let (0,0,1), (0,1,0), (0,1,1), etc. remain null => air
        // so that (1,0,0) => also air, etc.

        // Act
        VoxelVisibilityMap visibilityMap = new(chunk);

        // Assert
        // The only solid voxel is at (0,0,0). 
        // Because all adjacent positions are "air", it should have all 6 faces visible.
        VoxelFace faces = visibilityMap.GetVisibleFaces(0,0,0);
        Assert.Equal(
            VoxelFace.Zpos | VoxelFace.Zneg | VoxelFace.Xneg |
            VoxelFace.Xpos | VoxelFace.Ypos   | VoxelFace.Yneg, 
            faces
        );

        // All other coordinates are null => no voxel => faces = None
        VoxelFace faces100 = visibilityMap.GetVisibleFaces(1,0,0);
        Assert.Equal(VoxelFace.None, faces100);
    }


    [Fact]
    public void CheckErrorHandling_OutOfRangeShouldReturnNone()
    {
        // Arrange
        Chunk chunk = ChunkGen.GenerateChunk(1, 1, 1);
        chunk.Set(0, 0, 0, new Voxel(id: 123));
        VoxelVisibilityMap visibilityMap = new(chunk);

        // Act
        // Query something out of range
        VoxelFace result = visibilityMap.GetVisibleFaces(99, 99, 99);

        // Assert
        Assert.Equal(VoxelFace.None, result);
    }
}
