using System.Diagnostics;
using System.Numerics;
using System.Linq;
using VoxelVisibility;

namespace ConsoleAppExample;

public class SimdExample
{
    public static void Run()
    {
        // Dimensions
        const double threshold = 0.5;
        const int sizeX = 200, sizeY = 200, sizeZ = 200;

        // Generate random voxels
        var rnd = new Random();
        var voxels = new double[sizeX, sizeY, sizeZ];
        for (int x = 0; x < sizeX; x++)
            for (int y = 0; y < sizeY; y++)
                for (int z = 0; z < sizeZ; z++)
                    voxels[x, y, z] = rnd.NextDouble();

        // Time the visibility computation
        var sw = Stopwatch.StartNew();
        var visibleFaces = VisibilityCalculator.GetVisibleFaces(voxels, threshold);
        sw.Stop();

        Console.WriteLine($"Computed visible faces for {sizeX}×{sizeY}×{sizeZ} voxels in {sw.Elapsed.TotalMilliseconds:N2} ms.");


        var packed = VisibilityCalculatorSimd.Pack(voxels, threshold);
        sw = Stopwatch.StartNew();
        var visible = VisibilityCalculatorSimd.GetVisibleFaces(packed, sizeX, sizeY, sizeZ);
        sw.Stop();
        Console.WriteLine($"Computed visible faces for SIMD : {sizeX}×{sizeY}×{sizeZ} voxels in {sw.Elapsed.TotalMilliseconds:N2} ms.");

        // (Optional) Count total visible faces:
        long count = 0;
        bool areEquals = true;
        for (int x = 0; x < sizeX; x++)
            for (int y = 0; y < sizeY; y++)
                for (int z = 0; z < sizeZ; z++)
                    for (int f = 0; f < 6; f++){
                        if (visibleFaces[x, y, z][f]) count++;
                        if (visibleFaces[x, y, z][f] != visible[x, y, z][f]) areEquals = false; 
                    }

        Console.WriteLine($"Total visible faces: {count}");
        Console.WriteLine($"Are equals : {areEquals}");
    }







}
