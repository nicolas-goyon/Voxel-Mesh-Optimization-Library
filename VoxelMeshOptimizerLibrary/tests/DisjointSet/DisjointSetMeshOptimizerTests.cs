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
        var mesh = new Mesh();
        mesh.Quads.Add(new MeshQuad());


        Assert.Throws<ArgumentException>(() => new DisjointSetMeshOptimizer(mesh));
    }

    [Fact]
    public void Optimize_ShouldReturnEmptyMesh_WhenChunkIsEmpty()
    {
        var mesh = new Mesh();
        var chunk = ChunkGen.GenerateChunk(2, 2, 2);

        // All voxels are null by default (empty)
        var optimizer = new DisjointSetMeshOptimizer(mesh);
        var result = optimizer.Optimize(chunk);

        Assert.Empty(result.Quads);
    }

    [Fact]
    public void Optimize_ShouldProduce6Quads_ForSingleSolidVoxel()
    {
        var mesh = new Mesh();
        var chunk = ChunkGen.GenerateChunk(1, 1, 1);
        chunk.Set(0, 0, 0, new Voxel(42));

        var optimizer = new DisjointSetMeshOptimizer(mesh);
        var result = optimizer.Optimize(chunk);

        Assert.Equal(6, result.Quads.Count);
        Assert.All(result.Quads, quad => Assert.Equal(42, quad.VoxelID));
    }

    [Fact]
    public void Optimize_ShouldPreserveVoxelIDs_InAllGeneratedQuads()
    {
        var mesh = new Mesh();
        var chunk = ChunkGen.GenerateChunk(2, 1, 1);
        chunk.Set(0, 0, 0, new Voxel(100));
        chunk.Set(1, 0, 0, new Voxel(200));

        var optimizer = new DisjointSetMeshOptimizer(mesh);
        var result = optimizer.Optimize(chunk);

        Assert.Contains(result.Quads, q => q.VoxelID == 100);
        Assert.Contains(result.Quads, q => q.VoxelID == 200);
    }

    [Fact]
    public void Optimize_ShouldGenerateCorrectNormals_BasedOnFaceOrientation()
    {
        var mesh = new Mesh();
        var chunk = ChunkGen.GenerateChunk(1, 1, 1);
        chunk.Set(0, 0, 0, new Voxel(5));

        var optimizer = new DisjointSetMeshOptimizer(mesh);
        var result = optimizer.Optimize(chunk);

        var expectedNormals = new List<Vector3>
        {
            new Vector3(0, 0, 1),   // +Z
            new Vector3(0, 0, -1),  // -Z
            new Vector3(-1, 0, 0),  // -X
            new Vector3(1, 0, 0),   // +X
            new Vector3(0, 1, 0),   // +Y
            new Vector3(0, -1, 0)   // -Y
        };

        foreach (var quad in result.Quads)
        {
            Assert.Contains(quad.Normal, expectedNormals);
        }
    }
} 
