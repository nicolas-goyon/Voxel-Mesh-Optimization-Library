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
        var path = Path.Combine("/workspaces/Voxel-Mesh-Optimization-Library/VoxelMeshOptimizerLibrary/examples", "TetsChunk" + ".obj");

        // Act
        ObjExporter.Export(mesh, path);



        var quad = new MeshQuad
        {
            Vertex0 = new Vector3(0, 0, 0),
            Vertex1 = new Vector3(1, 0, 0),
            Vertex2 = new Vector3(1, 1, 0),
            Vertex3 = new Vector3(0, 1, 0),
            Normal = new Vector3(0, 0, 1),
            VoxelID = 1
        };
        var list = new List<MeshQuad>();
        list.Add(quad);
        mesh = new ExampleMesh(list);
        path = Path.Combine("/workspaces/Voxel-Mesh-Optimization-Library/VoxelMeshOptimizerLibrary/examples", Path.GetRandomFileName() + ".obj");

        // Act
        ObjExporter.Export(mesh, path);
    }
}

