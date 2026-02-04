using VoxelMeshOptimizer.Core;
using VoxelMeshOptimizer.Core.OptimizationAlgorithms.DisjointSet;
using VoxelMeshOptimizer.Core.TaskManager;
using VoxelMeshOptimizer.Helper;
using VoxelMeshOptimizer.Toolkit;
using Xunit;

namespace VoxelMeshOptimizer.Tests.TaskManager;

public class TaskManagerTests
{
    [Fact]
    public async Task TestManagerStart_StopNoErrors()
    {
        using CancellationTokenSource cts = new();
        // Example usage:
        TaskManager<(Int3 pos, Func<Int3, Chunk> gen), (Chunk chunk, Mesh mesh)> manager =  new(
            workerCount: Environment.ProcessorCount, // set to 1 if you want one worker
            processor: (job, ct) =>
            {
                (Int3 pos, Func<Int3, Chunk> gen) = job;
                Chunk chunk = gen(pos);

                DisjointSetMeshOptimizer optimizer = new(new Mesh([]));
                Mesh mesh = optimizer.Optimize(chunk);

                return ValueTask.FromResult((chunk, mesh));
            }
        );
        await manager.DisposeAsync();
    }

    [Fact]
    public async Task TestManagerOneTaskAndWait()
    {
        
        using CancellationTokenSource cts = new();
        // Example usage:
        TaskManager<(Int3 pos, Func<Int3, Chunk> gen), (Chunk chunk, Mesh mesh)> manager =  new(
            workerCount: Environment.ProcessorCount, // set to 1 if you want one worker
            processor: (job, ct) =>
            {
                (Int3 pos, Func<Int3, Chunk> gen) = job;
                Chunk chunk = gen(pos);

                DisjointSetMeshOptimizer optimizer = new(new Mesh([]));
                Mesh mesh = optimizer.Optimize(chunk);

                return ValueTask.FromResult((chunk, mesh));
            }
        );
        
        Int3 chunkPos = new() { x = 0, y = 0, z = 0 };
        _ = manager.Enqueue((chunkPos, p => new Chunk(PerlinNoiseChunkGen.CreatePerlinLandscape(size: 16, seed: 123))));

        
        TaskManager<(Int3 pos, Func<Int3, Chunk> gen), (Chunk chunk, Mesh mesh)>.Completion next = await manager.WaitForCompletedAsync(cts.Token);

        Assert.True(next.Success);
        Assert.NotNull(next.Result.chunk);
        Assert.NotNull(next.Result.mesh);
        
        await manager.DisposeAsync();
    }
    
    
    [Theory]
    [InlineData(2)]
    [InlineData(10)]
    [InlineData(100)]
    public async Task TestManagerMultipleTaskAndWait(int iterations)
    {
        
        using CancellationTokenSource cts = new();
        // Example usage:
        TaskManager<(Int3 pos, Func<Int3, Chunk> gen), (Chunk chunk, Mesh mesh)> manager =  new(
            workerCount: Environment.ProcessorCount, // set to 1 if you want one worker
            processor: (job, ct) =>
            {
                (Int3 pos, Func<Int3, Chunk> gen) = job;
                Chunk chunk = gen(pos);

                DisjointSetMeshOptimizer optimizer = new(new Mesh([]));
                Mesh mesh = optimizer.Optimize(chunk);

                return ValueTask.FromResult((chunk, mesh));
            }
        );
        
        Int3 chunkPos = new() { x = 0, y = 0, z = 0 };

        for (int i = 0; i < iterations; i++)
        {
            _ = manager.Enqueue((chunkPos,
                p => new Chunk(PerlinNoiseChunkGen.CreatePerlinLandscape(size: 16, seed: 123))));
        }


        for (int i = 0; i < iterations; i++)
        {
            TaskManager<(Int3 pos, Func<Int3, Chunk> gen), (Chunk chunk, Mesh mesh)>.Completion next = await manager.WaitForCompletedAsync(cts.Token);
            Assert.True(next.Success);
            Assert.NotNull(next.Result.chunk);
            Assert.NotNull(next.Result.mesh);
        }

        
        await manager.DisposeAsync();
    }
    
    
    
    [Theory]
    [InlineData(2)]
    [InlineData(10)]
    [InlineData(100)]
    public async Task TestManagerMultipleTaskAndNoWaitButThreadSleep(int iterations)
    {
        
        using CancellationTokenSource cts = new();
        // Example usage:
        TaskManager<(Int3 pos, Func<Int3, Chunk> gen), (Chunk chunk, Mesh mesh)> manager =  new(
            workerCount: Environment.ProcessorCount, // set to 1 if you want one worker
            processor: (job, ct) =>
            {
                (Int3 pos, Func<Int3, Chunk> gen) = job;
                Chunk chunk = gen(pos);

                DisjointSetMeshOptimizer optimizer = new(new Mesh([]));
                Mesh mesh = optimizer.Optimize(chunk);

                return ValueTask.FromResult((chunk, mesh));
            }
        );
        
        Int3 chunkPos = new() { x = 0, y = 0, z = 0 };

        for (int i = 0; i < iterations; i++)
        {
            _ = manager.Enqueue((chunkPos,
                p => new Chunk(PerlinNoiseChunkGen.CreatePerlinLandscape(size: 16, seed: 123))));
        }

        int maxSleep = iterations * 1000;
        for (int i = 0; i < iterations; i++)
        {
            if (manager.TryDequeueCompleted(out TaskManager<(Int3 pos, Func<Int3, Chunk> gen), (Chunk chunk, Mesh mesh)>.Completion r))
            {
                Assert.True(r.Success);
                Assert.NotNull(r.Result.chunk);
                Assert.NotNull(r.Result.mesh);
            }
            else
            {
                i--;
                maxSleep--;
                if (maxSleep <= 0)
                {
                    Assert.True(false); // Test probably failed, waited too long, probably infinite loop
                }
                Thread.Sleep(100);
            }
            
        }

        
        await manager.DisposeAsync();
    }
}