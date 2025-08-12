using VoxelMeshOptimizer.Core;
using VoxelMeshOptimizer.Core.OptimizationAlgorithms.DisjointSet;

namespace ConsoleAppExample;
using System.Numerics;
using VoxelMeshOptimizer.Core.OcclusionAlgorithms;
using VoxelMeshOptimizer.Core.OcclusionAlgorithms.Common;

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



        // var exampleChunk = new ExampleChunk("TestChunkPerlinNoiseGen.chk");

        // // var mesh = exampleChunk.ToMesh();
        // var optimizer = new DisjointSetMeshOptimizer(new ExampleMesh());
        // Mesh optimizedMesh = optimizer.Optimize(exampleChunk);

        // var path = Path.Combine("/workspaces/Voxel-Mesh-Optimization-Library/VoxelMeshOptimizerLibrary/examples", "Test2" + ".obj");

        // // Act
        // ObjExporter.Export(optimizedMesh, path);



        var exampleChunk = PerlinNoiseChunkGen.CreatePerlinLandscape(50, 123);

        var baseMesh = exampleChunk.ToMesh();
        var path = Path.Combine("/workspaces/Voxel-Mesh-Optimization-Library/VoxelMeshOptimizerLibrary/examples", "ChunkBase" + ".obj");
        ObjExporter.Export(baseMesh, path);


        
        var occluder = new VoxelOcclusionOptimizer(exampleChunk);
        var visibileFaces = occluder.ComputeVisibleFaces();
        var occludedQuads = VisibleFacesMesher.Build(visibileFaces, exampleChunk);
        var occludedMesh = new ExampleMesh(occludedQuads);
        path = Path.Combine("/workspaces/Voxel-Mesh-Optimization-Library/VoxelMeshOptimizerLibrary/examples", "ChunkBaseOccluded" + ".obj");
        ObjExporter.Export(occludedMesh, path);


        // var mesh = exampleChunk.ToMesh();
        var optimizer = new DisjointSetMeshOptimizer(new ExampleMesh());
        Mesh optimizedMesh = optimizer.Optimize(exampleChunk);

        path = Path.Combine("/workspaces/Voxel-Mesh-Optimization-Library/VoxelMeshOptimizerLibrary/examples", "ChunkOptimized" + ".obj");
        ObjExporter.Export(optimizedMesh, path);

    }
}

