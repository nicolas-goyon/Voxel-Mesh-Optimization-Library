using VoxelMeshOptimizer.Core;
using VoxelMeshOptimizer.Core.OptimizationAlgorithms.DisjointSet;

namespace ConsoleAppExample;

class Program
{
    static void Main(string[] args)
    {
        // ushort[,,] voxels = {
        //     { {0,0,0} , {0,0,0}, {0,0,0}},
        //     { {0,0,0} , {0,0,0}, {0,0,0}},
        //     { {0,0,0} , {0,0,0}, {0,0,0}},
        // };
        // var exampleChunk = new ExampleChunk(voxels);
        // var optimizer = new DisjointSetMeshOptimizer();
        // Mesh optimizedMesh = optimizer.Optimize(exampleChunk);

        // Console.WriteLine("Mesh optimized successfully!");

        Console.WriteLine($"Results equal");
        SimdExample.Run();
        Console.WriteLine($"Results equal");
    }
}

