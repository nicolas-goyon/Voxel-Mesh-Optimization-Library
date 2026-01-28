using System.Numerics;
using VoxelMeshOptimizer.Core;
using VoxelMeshOptimizer.Core.Multithreading;
using VoxelMeshOptimizer.Tests.Helpers;
using Xunit.Abstractions;

namespace VoxelMeshOptimizer.Tests.Multithreading;

public class ChunkMeshGenerationWorkerTests
{

    [Fact]
    public void ChunkGenerationWorker_ExecuteCorrectly_FullChunk()
    {
        Chunk chunk = ChunkGen.GenerateBasicChunk_full();
        ChunkMeshGenerationWorker worker = new(chunk);
        (Chunk chunk, Mesh mesh) res = worker.Execute();
        Assert.Equal(6, res.mesh.Quads.Count);
        MeshQuad expectedQuad = new()
        {
            Vertex0 = new Vector3(0,16,0),
            Vertex1 = new Vector3(0,0,0),
            Vertex2 = new Vector3(0,0,16),
            Vertex3 = new Vector3(0,16,16),
            Normal  = new Vector3(1,0,0),
            VoxelId = 1
        };
        Assert.Equal(expectedQuad, res.mesh.Quads[0]);
    }
    
    
    [Fact]
    public void ChunkGenerationWorker_ExecuteCorrectly_PartiallyFullChunk()
    {
        Chunk chunk = ChunkGen.GenerateBasicChunk_partiallyFull();
        ChunkMeshGenerationWorker worker = new(chunk);
        (Chunk chunk, Mesh mesh) res = worker.Execute();
        Assert.Equal(6, res.mesh.Quads.Count);
        MeshQuad expectedQuad = new()
        {
            Vertex0 = new Vector3(0,9,0),
            Vertex1 = new Vector3(0,0,0),
            Vertex2 = new Vector3(0,0,16),
            Vertex3 = new Vector3(0,9,16),
            Normal  = new Vector3(1,0,0),
            VoxelId = 1
        };
        Assert.Equivalent(expectedQuad, res.mesh.Quads[0]);
    }
}