using System.Collections.Concurrent;
using VoxelMeshOptimizer.Core;
using VoxelMeshOptimizer.Core.OptimizationAlgorithms.DisjointSet;
using VoxelMeshOptimizer.Core.TaskManager;
using VoxelMeshOptimizer.Helper;

namespace ConsoleAppExample;
using VoxelMeshOptimizer.Toolkit;

class Program
{
    private static async Task Main(string[] args)
    {
        using var cts = new CancellationTokenSource();
        // Example usage:
        TaskManager<(Int3 pos, Func<Int3, Chunk> gen), (Chunk chunk, Mesh mesh)> manager = new(
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

        const int iterations = 3;
        Int3 chunkPos = new() { x = 0, y = 0, z = 0 };

        for (int i = 0; i < iterations; i++)
        {
            _ = manager.Enqueue((chunkPos, p => new Chunk(PerlinNoiseChunkGen.CreatePerlinLandscape(size: 16, seed: 123))));
        }

        // Option 1: poll without waiting
        if (manager.TryDequeueCompleted(out TaskManager<(Int3 pos, Func<Int3, Chunk> gen), (Chunk chunk, Mesh mesh)>.Completion r))
        {
            if (r.Success) Console.WriteLine("Got mesh!");
            else Console.WriteLine($"Job failed: {r.Error}");
        }
        else
        {
            Console.WriteLine("Too fast !");
        }

        // Option 2: wait for the next completion
        TaskManager<(Int3 pos, Func<Int3, Chunk> gen), (Chunk chunk, Mesh mesh)>.Completion next = await manager.WaitForCompletedAsync(cts.Token);
        Console.WriteLine($"Next success={next.Success}");

        await manager.DisposeAsync();


    }

}

