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
        // ExampleChunk implements Chunk<Voxel>
        var chunk = new Chunk(PerlinNoiseChunkGen.CreatePerlinLandscape(50, 123));
        var optimizer = new DisjointSetMeshOptimizer(new Mesh());
        Mesh mesh = optimizer.Optimize(chunk);

        // Option 1 : Export to Wavefront OBJ
        File.WriteAllText("chunk.obj", ObjExporter.MeshToObjString(mesh));
        
        // Option 2 : Get all the Quads for a in-engine usage (1 Quad = 2 triangles)
        List<MeshQuad> quads = mesh.Quads;
    }
}