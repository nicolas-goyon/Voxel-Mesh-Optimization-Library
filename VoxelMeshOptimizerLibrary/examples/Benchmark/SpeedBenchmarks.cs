using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using ConsoleAppExample;
using VoxelMeshOptimizer.Core;
using VoxelMeshOptimizer.Core.OcclusionAlgorithms;
using VoxelMeshOptimizer.Core.OptimizationAlgorithms.DisjointSet;
using VoxelMeshOptimizer.Toolkit;

[MemoryDiagnoser]
[MarkdownExporter, AsciiDocExporter, HtmlExporter, CsvExporter, RPlotExporter]
public class SpeedBenchmarks
{
    private ExampleChunk exampleChunk;

    
    [IterationSetup]
    public void Setup()
    {

        exampleChunk = new ExampleChunk(PerlinNoiseChunkGen.CreatePerlinLandscape(50, 123));
    }


    [Benchmark(Baseline = true)]
    public void Default()
    {
        var baseMesh = exampleChunk.ToMesh();
        ObjExporter.MeshToObjString(baseMesh);
    }

    [Benchmark]
    public void Occluded()
    {
        var occluder = new VoxelOcclusionOptimizer(exampleChunk);
        var visibileFaces = occluder.ComputeVisibleFaces();
        var occludedQuads = VisibleFacesMesher.Build(visibileFaces, exampleChunk);
        var occludedMesh = new ExampleMesh(occludedQuads);
        ObjExporter.MeshToObjString(occludedMesh);
    }


    [Benchmark]
    public void Optimized()
    {
        var optimizer = new DisjointSetMeshOptimizer(new ExampleMesh());
        Mesh optimizedMesh = optimizer.Optimize(exampleChunk);
        ObjExporter.MeshToObjString(optimizedMesh);
    }
}
