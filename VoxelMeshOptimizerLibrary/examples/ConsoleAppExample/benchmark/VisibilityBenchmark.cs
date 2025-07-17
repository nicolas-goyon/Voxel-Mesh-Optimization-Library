using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using VoxelVisibility;

[MemoryDiagnoser]
[MarkdownExporter, AsciiDocExporter, HtmlExporter, CsvExporter, RPlotExporter]
public class VisibilityBenchmarks
{
    private const double threshold = 0.4;

    [Params(50, 250)]
    public int size;

    public int sizeX => size;

    public int sizeY => size;

    public int sizeZ => size;

    private double[,,] voxels = null!;
    private bool[] packed = null!;

    
    [IterationSetup]
    public void Setup()
    {

        voxels = new double[sizeX, sizeY, sizeZ];
        var rnd = new Random();
        for (int x = 0; x < sizeX; x++)
            for (int y = 0; y < sizeY; y++)
                for (int z = 0; z < sizeZ; z++)
                    voxels[x, y, z] = rnd.NextDouble();

        // Prepare the packed boolean array for the SIMD version
        packed = VisibilityCalculatorSimd.Pack(voxels, threshold);
    }


    // private void Verify(bool[,,][] result, string label)
    // {
    //     for (int x = 0; x < sizeX; x++)
    //         for (int y = 0; y < sizeY; y++)
    //             for (int z = 0; z < sizeZ; z++)
    //                 for (int f = 0; f < 6; f++)
    //                     if (result[x, y, z][f] != baseline[x, y, z][f])
    //                         throw new InvalidOperationException($"{label} mismatch at {x},{y},{z} face {f}");
    // }

    [Benchmark(Baseline = true)]
    public void Naive()
    {
        VisibilityCalculator.GetVisibleFaces(voxels, threshold);
    }

    [Benchmark]
    public void BitOpsOptimized()
    {
        var bools = VisibilityCalculatorBinary.ToBools(voxels, threshold);

        VisibilityCalculatorBinary.GetVisibleFaces(bools);
    }


    [Benchmark]
    public void Simd()
    {
        VisibilityCalculatorSimd.GetVisibleFaces(packed, sizeX, sizeY, sizeZ);
    }


    [Benchmark]
    public void SimdOptimized()
    {
        VisibilityCalculatorSimd.GetVisibleFacesOptimized(packed, sizeX, sizeY, sizeZ);
    }
}
