using System.Numerics;
using VoxelMeshOptimizer.Core;
using VoxelMeshOptimizer.Core.Multithreading;
using VoxelMeshOptimizer.Helper;
using VoxelMeshOptimizer.Tests.Helpers;
using Xunit.Abstractions;

namespace VoxelMeshOptimizer.Tests.Multithreading;
public class ChunkGenerationThreadTests
{
    [Fact]
    public void ChunkGenerationWorkerThread_ExecuteAndWaitCorrect()
    {
        ChunkGenerationThread thread = new();
        GenerateNewChunkAt(new Int3(){x=0,y=0,z=0}, thread);
        TimeSpan timeout = TimeSpan.FromSeconds(5);
        (Chunk chunk, Mesh mesh) res = thread.WaitForFinishedWork(timeout);
        Assert.NotNull(res.chunk);
        Assert.NotNull(res.mesh);
        thread.Dispose();
    }
    
    
    [Fact]
    public void ChunkGenerationWorkerThread_ExecuteAndWaitCorrect_MultipleWorks()
    {
        ChunkGenerationThread thread = new();
        GenerateNewChunkAt(new Int3(){x=0,y=0,z=0}, thread);
        GenerateNewChunkAt(new Int3(){x=0,y=1,z=0}, thread);
        GenerateNewChunkAt(new Int3(){x=0,y=2,z=0}, thread);
        
        GenerateNewChunkAt(new Int3(){x=0,y=3,z=0}, thread);
        GenerateNewChunkAt(new Int3(){x=0,y=4,z=0}, thread);
        GenerateNewChunkAt(new Int3(){x=0,y=5,z=0}, thread);
        TimeSpan timeout = TimeSpan.FromSeconds(5);
        (Chunk chunk, Mesh mesh) res = thread.WaitForFinishedWork(timeout);
        Assert.NotNull(res.chunk);
        Assert.NotNull(res.mesh);
        res = thread.WaitForFinishedWork(timeout);
        Assert.NotNull(res.chunk);
        Assert.NotNull(res.mesh);
        res = thread.WaitForFinishedWork(timeout);
        Assert.NotNull(res.chunk);
        Assert.NotNull(res.mesh);
        
        res = thread.WaitForFinishedWork(timeout);
        Assert.NotNull(res.chunk);
        Assert.NotNull(res.mesh);
        res = thread.WaitForFinishedWork(timeout);
        Assert.NotNull(res.chunk);
        Assert.NotNull(res.mesh);
        res = thread.WaitForFinishedWork(timeout);
        Assert.NotNull(res.chunk);
        Assert.NotNull(res.mesh);
        
        thread.Dispose();
    }
    
    private static void GenerateNewChunkAt(Int3 chunkPos, ChunkGenerationThread thread)
    {
        thread.EnqueueChunk(chunkPos, int3 => new Chunk(ChunkGen.GenerateBasicVoxelArray_PartiallyFull()));
    }

}