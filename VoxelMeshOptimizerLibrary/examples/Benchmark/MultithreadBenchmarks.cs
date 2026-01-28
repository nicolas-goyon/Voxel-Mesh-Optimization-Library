// using BenchmarkDotNet.Attributes;
// using VoxelMeshOptimizer.Core;
// using VoxelMeshOptimizer.Core.OcclusionAlgorithms;
// using VoxelMeshOptimizer.Core.OptimizationAlgorithms.DisjointSet;
// using VoxelMeshOptimizer.Core.Toolkit;
// using VoxelMeshOptimizer.Toolkit;
//
// [MemoryDiagnoser]
// [MarkdownExporter, AsciiDocExporter, HtmlExporter, CsvExporter, RPlotExporter,JsonExporter]
// public class MultithreadBenchmarks
// {
//     
//
//     [Benchmark(Baseline = true)]
//     public void Default_MeshToString()
//     {
//     }
//
//     [Benchmark]
//     public void Occluder_MeshToString()
//     {
//     }
// }