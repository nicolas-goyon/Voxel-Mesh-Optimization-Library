using VoxelMeshOptimizer.Core;
using VoxelMeshOptimizer.Core.OptimizationAlgorithms.DisjointSet;
using VoxelMeshOptimizer.Core.Toolkit;

namespace ConsoleAppExample;
using System.Numerics;
using VoxelMeshOptimizer.Core.OcclusionAlgorithms;
using VoxelMeshOptimizer.Core.OcclusionAlgorithms.Common;
using VoxelMeshOptimizer.Toolkit;

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



        // var exampleChunk = new ExampleChunk("/workspaces/Voxel-Mesh-Optimization-Library/VoxelMeshOptimizerLibrary/examples/ConsoleAppExample/Resources/TestChunkPerlinNoiseGen.chk");
        // exampleChunk.Save("/workspaces/Voxel-Mesh-Optimization-Library/VoxelMeshOptimizerLibrary/examples/ConsoleAppExample/Resources/TestChunkPerlinNoiseGen.chk");

        // // var mesh = exampleChunk.ToMesh();
        // var optimizer = new DisjointSetMeshOptimizer(new ExampleMesh());
        // Mesh optimizedMesh = optimizer.Optimize(exampleChunk);

        // var path = Path.Combine("/workspaces/Voxel-Mesh-Optimization-Library/VoxelMeshOptimizerLibrary/examples/ConsoleAppExample/Resources", "Test2" + ".obj");

        // // Act
        // ObjExporter.Export(optimizedMesh, path);

        Chunk exampleChunk = new Chunk(PerlinNoiseChunkGen.CreatePerlinLandscape(50, 123));

        Mesh baseMesh = exampleChunk.ToMesh();
        string filePath = Path.Combine("/workspaces/Voxel-Mesh-Optimization-Library/VoxelMeshOptimizerLibrary/examples/ConsoleAppExample/Resources", "ChunkBase" + ".obj");
        File.WriteAllText(filePath, ObjExporter.MeshToObjString(baseMesh));
        Console.WriteLine(filePath);

        
        VoxelOcclusionOptimizer occluder = new VoxelOcclusionOptimizer(exampleChunk);
        VisibleFaces visibileFaces = occluder.ComputeVisibleFaces();
        List<MeshQuad> occludedQuads = VisibleFacesMesher.Build(visibileFaces, exampleChunk);
        Mesh occludedMesh = new Mesh(occludedQuads);
        filePath = Path.Combine("/workspaces/Voxel-Mesh-Optimization-Library/VoxelMeshOptimizerLibrary/examples/ConsoleAppExample/Resources", "ChunkBaseOccluded" + ".obj");
        File.WriteAllText(filePath, ObjExporter.MeshToObjString(occludedMesh));
        Console.WriteLine(filePath);


        // var mesh = exampleChunk.ToMesh();
        DisjointSetMeshOptimizer optimizer = new DisjointSetMeshOptimizer(new Mesh());
        Mesh optimizedMesh = optimizer.Optimize(exampleChunk);
        filePath = Path.Combine("/workspaces/Voxel-Mesh-Optimization-Library/VoxelMeshOptimizerLibrary/examples/ConsoleAppExample/Resources", "ChunkOptimized" + ".obj");
        File.WriteAllText(filePath, ObjExporter.MeshToObjString(optimizedMesh));
        Console.WriteLine(filePath);

    }
}

