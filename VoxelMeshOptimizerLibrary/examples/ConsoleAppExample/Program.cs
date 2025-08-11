using VoxelMeshOptimizer.Core;
using VoxelMeshOptimizer.Core.OptimizationAlgorithms.DisjointSet;

namespace ConsoleAppExample;
using System.Numerics;

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


        
        ushort[,,] voxels = {
            { {1,1,1} , {1,1,1}, {1,1,1}},
            { {1,1,1} , {1,1,1}, {0,0,0}},
            { {1,1,1} , {0,0,0}, {0,0,0}},
        };
        var exampleChunk = new ExampleChunk(voxels);

        var mesh = exampleChunk.ToMesh();
        // var path = Path.Combine("/workspaces/Voxel-Mesh-Optimization-Library/VoxelMeshOptimizerLibrary/examples", "Test" + ".obj");

        // Act
        // ObjExporter.Export(mesh, path);



    }
}

