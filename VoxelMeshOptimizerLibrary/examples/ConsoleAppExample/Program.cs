using VoxelMeshOptimizer.Core;
using VoxelMeshOptimizer.Core.OptimizationAlgorithms.DisjointSet;

namespace ConsoleAppExample;

class Program
{
    static void Main(string[] args)
    {
        Chunk exampleChunk = new ExampleChunk();
        MeshOptimizer optimizer = new DisjointSetMeshOptimizer();
        Mesh optimizedMesh = optimizer.Optimize(exampleChunk);
        
        Console.WriteLine("Mesh optimized successfully!");
    }
}
