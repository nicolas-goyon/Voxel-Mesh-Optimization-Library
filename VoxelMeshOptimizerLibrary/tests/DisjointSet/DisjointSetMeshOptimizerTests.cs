using System.Numerics;
using VoxelMeshOptimizer.Core;
using VoxelMeshOptimizer.Core.OptimizationAlgorithms.DisjointSet;
using VoxelMeshOptimizer.Tests.Helpers;

namespace VoxelMeshOptimizer.Tests.DisjointSetTesting;

public class DisjointSetMeshOptimizerTests
{

    

    [Fact]
    public void Optimize_ShouldThrow_WhenMeshIsNotEmpty()
    {
        Mesh mesh = new Mesh();
        mesh.Quads.Add(new MeshQuad());


        Assert.Throws<ArgumentException>(() => new DisjointSetMeshOptimizer(mesh));
    }

    [Fact]
    public void Optimize_ShouldReturnEmptyMesh_WhenChunkIsEmpty()
    {
        Mesh mesh = new Mesh();
        Chunk chunk = ChunkGen.GenerateChunk(2, 2, 2);

        // All voxels are null by default (empty)
        DisjointSetMeshOptimizer optimizer = new DisjointSetMeshOptimizer(mesh);
        Mesh result = optimizer.Optimize(chunk);

        Assert.Empty(result.Quads);
    }

    [Fact]
    public void Optimize_ShouldProduce6Quads_ForSingleSolidVoxel()
    {
        Mesh mesh = new Mesh();
        Chunk chunk = ChunkGen.GenerateChunk(1, 1, 1);
        chunk.Set(0, 0, 0, new Voxel(42));

        DisjointSetMeshOptimizer optimizer = new DisjointSetMeshOptimizer(mesh);
        Mesh result = optimizer.Optimize(chunk);

        Assert.Equal(6, result.Quads.Count);
        Assert.All(result.Quads, quad => Assert.Equal(42, quad.VoxelId));
    }

    [Fact]
    public void Optimize_ShouldPreserveVoxelIDs_InAllGeneratedQuads()
    {
        Mesh mesh = new Mesh();
        Chunk chunk = ChunkGen.GenerateChunk(2, 1, 1);
        chunk.Set(0, 0, 0, new Voxel(100));
        chunk.Set(1, 0, 0, new Voxel(200));

        DisjointSetMeshOptimizer optimizer = new DisjointSetMeshOptimizer(mesh);
        Mesh result = optimizer.Optimize(chunk);

        Assert.Contains(result.Quads, q => q.VoxelId == 100);
        Assert.Contains(result.Quads, q => q.VoxelId == 200);
    }

    [Fact]
    public void Optimize_ShouldGenerateCorrectNormals_BasedOnFaceOrientation()
    {
        Mesh mesh = new Mesh();
        Chunk chunk = ChunkGen.GenerateChunk(1, 1, 1);
        chunk.Set(0, 0, 0, new Voxel(5));

        DisjointSetMeshOptimizer optimizer = new DisjointSetMeshOptimizer(mesh);
        Mesh result = optimizer.Optimize(chunk);

        List<Vector3> expectedNormals = new List<Vector3>
        {
            new Vector3(0, 0, 1),   // +Z
            new Vector3(0, 0, -1),  // -Z
            new Vector3(-1, 0, 0),  // -X
            new Vector3(1, 0, 0),   // +X
            new Vector3(0, 1, 0),   // +Y
            new Vector3(0, -1, 0)   // -Y
        };

        foreach (MeshQuad quad in result.Quads)
        {
            Assert.Contains(quad.Normal, expectedNormals);
        }
    }
} 
