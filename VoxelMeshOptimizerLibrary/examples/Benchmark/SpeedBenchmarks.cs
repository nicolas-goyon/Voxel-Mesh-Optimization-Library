using BenchmarkDotNet.Attributes;
using ConsoleAppExample;
using VoxelMeshOptimizer.Core;
using VoxelMeshOptimizer.Core.OcclusionAlgorithms;
using VoxelMeshOptimizer.Core.OptimizationAlgorithms.DisjointSet;
using VoxelMeshOptimizer.Toolkit;

[MemoryDiagnoser]
[MarkdownExporter, AsciiDocExporter, HtmlExporter, CsvExporter, RPlotExporter,JsonExporter]
public class SpeedBenchmarks
{
    private ExampleChunk exampleChunk;


    [IterationSetup]
    public void Setup()
    {
        exampleChunk = new ExampleChunk(PerlinNoiseChunkGen.CreatePerlinLandscape(50, 123));
    }


    [Benchmark]
    public void Occluder()
    {
        var occluder = new VoxelOcclusionOptimizer(exampleChunk);
        var visibileFaces = occluder.ComputeVisibleFaces();
        VisibleFacesMesher.Build(visibileFaces, exampleChunk);
    }


    [Benchmark]
    public void Optimize()
    {
        var optimizer = new DisjointSetMeshOptimizer(new ExampleMesh());
        optimizer.Optimize(exampleChunk);
    }
    


    [Benchmark(Baseline = true)]
    public void Default_MeshToString()
    {
        var baseMesh = exampleChunk.ToMesh();
        ObjExporter.MeshToObjString(baseMesh);
    }

    [Benchmark]
    public void Occluder_MeshToString()
    {
        var occluder = new VoxelOcclusionOptimizer(exampleChunk);
        var visibileFaces = occluder.ComputeVisibleFaces();
        var occludedQuads = VisibleFacesMesher.Build(visibileFaces, exampleChunk);
        var occludedMesh = new ExampleMesh(occludedQuads);
        ObjExporter.MeshToObjString(occludedMesh);
    }


    [Benchmark]
    public void Optimize_MeshToString()
    {
        var optimizer = new DisjointSetMeshOptimizer(new ExampleMesh());
        Mesh optimizedMesh = optimizer.Optimize(exampleChunk);
        ObjExporter.MeshToObjString(optimizedMesh);
    }
}
