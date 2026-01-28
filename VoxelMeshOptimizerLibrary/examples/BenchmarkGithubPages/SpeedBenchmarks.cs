using BenchmarkDotNet.Attributes;
using VoxelMeshOptimizer.Core;
using VoxelMeshOptimizer.Core.OcclusionAlgorithms;
using VoxelMeshOptimizer.Core.OcclusionAlgorithms.Common;
using VoxelMeshOptimizer.Core.OptimizationAlgorithms.DisjointSet;
using VoxelMeshOptimizer.Core.Toolkit;
using VoxelMeshOptimizer.Toolkit;

namespace BenchmarkGithubPages;

[JsonExporterAttribute.Full]
[JsonExporterAttribute.FullCompressed]
public class SpeedBenchmarks
{
    private Chunk exampleChunk;


    [IterationSetup]
    public void Setup()
    {
        exampleChunk = new Chunk(PerlinNoiseChunkGen.CreatePerlinLandscape(50, 123));
    }


    [Benchmark]
    public void Occluder()
    {
        VoxelOcclusionOptimizer occluder = new VoxelOcclusionOptimizer(exampleChunk);
        VisibleFaces visibileFaces = occluder.ComputeVisibleFaces();
        VisibleFacesMesher.Build(visibileFaces, exampleChunk);
    }


    [Benchmark]
    public void Optimize()
    {
        DisjointSetMeshOptimizer optimizer = new DisjointSetMeshOptimizer(new Mesh());
        optimizer.Optimize(exampleChunk);
    }
    


    [Benchmark(Baseline = true)]
    public void Default_MeshToString()
    {
        Mesh baseMesh = exampleChunk.ToMesh();
        ObjExporter.MeshToObjString(baseMesh);
    }

    [Benchmark]
    public void Occluder_MeshToString()
    {
        VoxelOcclusionOptimizer occluder = new VoxelOcclusionOptimizer(exampleChunk);
        VisibleFaces visibileFaces = occluder.ComputeVisibleFaces();
        List<MeshQuad> occludedQuads = VisibleFacesMesher.Build(visibileFaces, exampleChunk);
        Mesh occludedMesh = new Mesh(occludedQuads);
        ObjExporter.MeshToObjString(occludedMesh);
    }


    [Benchmark]
    public void Optimize_MeshToString()
    {
        DisjointSetMeshOptimizer optimizer = new DisjointSetMeshOptimizer(new Mesh());
        Mesh optimizedMesh = optimizer.Optimize(exampleChunk);
        ObjExporter.MeshToObjString(optimizedMesh);
    }
}