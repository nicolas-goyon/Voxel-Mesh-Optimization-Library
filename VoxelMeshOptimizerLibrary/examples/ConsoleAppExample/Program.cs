using VoxelMeshOptimizer.Core;
using VoxelMeshOptimizer.Core.OptimizationAlgorithms.DisjointSet;

namespace ConsoleAppExample;

class Program
{
    static void Main(string[] args)
    {
        ushort[,,] voxels = {
            { {0,0,0} , {0,0,0}, {0,0,0}},
            { {0,0,0} , {0,0,0}, {0,0,0}},
            { {0,0,0} , {0,0,0}, {0,0,0}},
        };
        var exampleChunk = new ExampleChunk(voxels);
        var mesh = new ExampleMesh();
        var optimizer = new DisjointSetMeshOptimizer(mesh);
        Mesh optimizedMesh = optimizer.Optimize(exampleChunk);

        Console.WriteLine("Mesh optimized successfully!");
    }
}

