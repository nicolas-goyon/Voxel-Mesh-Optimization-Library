using BenchmarkDotNet.Attributes;
using VoxelMeshOptimizer.Core;
using VoxelMeshOptimizer.Core.OptimizationAlgorithms.DisjointSet;
using VoxelMeshOptimizer.Core.TaskManager;
using VoxelMeshOptimizer.Helper;
using VoxelMeshOptimizer.Toolkit;

namespace Benchmark;

public class TasksBenchmark
{

    private void VerifyResult(TaskManager<(Int3 pos, Func<Int3, Chunk> gen), (Chunk chunk, Mesh mesh)>.Completion result)
    {
        if (!result.Success || result.Result.chunk is null || result.Result.mesh is null) throw new InvalidOperationException();
    }
    
    [Benchmark]
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

    [Benchmark]
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

        VerifyResult(next);
        
        await manager.DisposeAsync();
    }
    
    
    [Benchmark]
    [Arguments(2)]
    [Arguments(5)]
    [Arguments(10)]
    [Arguments(20)]
    [Arguments(100)]
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
            VerifyResult(next);
        }

        
        await manager.DisposeAsync();
    }
    
    
    
    [Benchmark]
    [Arguments(2)]
    [Arguments(5)]
    [Arguments(10)]
    [Arguments(20)]
    [Arguments(100)]
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
                VerifyResult(r);
            }
            else
            {
                i--;
                maxSleep--;
                if (maxSleep <= 0)
                {
                    throw new InvalidOperationException("Benchmark failed, waited too long, probably infinite loop");
                }
                Thread.Sleep(100);
            }
            
        }

        
        await manager.DisposeAsync();
    }
}