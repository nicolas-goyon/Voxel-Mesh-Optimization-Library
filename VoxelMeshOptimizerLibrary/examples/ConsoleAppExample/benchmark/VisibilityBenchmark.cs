using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using VoxelVisibility;

[MemoryDiagnoser]
public class VisibilityBenchmarks
{
    private const double threshold = 0.4;
    private const int sizeX = 254, sizeY = 254, sizeZ = 254;
    private double[,,] voxels = null!;
    private bool[] packed = null!;

    [GlobalSetup]
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

    [Benchmark]
    public void Naive() => VisibilityCalculator.GetVisibleFaces(voxels, threshold);

    [Benchmark]
    public void BitOps() => VisibilityCalculatorBit.GetVisibleFaces(voxels, threshold);

    
    [Benchmark]
    public void BitOpsOptimized()
    {
        var bools = VisibilityCalculatorBit.ToBools(voxels, threshold);
        VisibilityCalculatorBinaryOptimized.GetVisibleFaces(bools);
    }


    [Benchmark]
    public void Simd() => VisibilityCalculatorSimd.GetVisibleFaces(packed, sizeX, sizeY, sizeZ);
}
