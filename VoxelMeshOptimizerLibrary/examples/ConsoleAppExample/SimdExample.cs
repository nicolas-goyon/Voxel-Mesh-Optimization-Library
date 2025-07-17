using System.Diagnostics;
using System.Numerics;
using System.Linq;
using VoxelVisibility;
using System.Text;

namespace ConsoleAppExample;

public class SimdExample
{
    public static void Run()
    {
        #region init
        // Dimensions
        const double threshold = 0.4;
        const int sizeX = 200, sizeY = 200, sizeZ = 200;

        // Generate random voxels
        var rnd = new Random();
        var voxels = new double[sizeX, sizeY, sizeZ];

        for (int x = 0; x < sizeX; x++)
            for (int y = 0; y < sizeY; y++)
                for (int z = 0; z < sizeZ; z++)
                    voxels[x, y, z] = rnd.NextDouble();


        // StringBuilder sb = new();
        // for (int x = 0; x < sizeX; x++)
        // {
        //     for (int y = 0; y < sizeY; y++)
        //     {
        //         for (int z = 0; z < sizeZ; z++)
        //         {
        //             sb.Append(voxels[x, y, z] > threshold ? "\u23F9" : " ");
        //         }
        //         sb.Append("\n");
        //     }

        //     sb.Append("\n");
        // }

        // Console.Write(sb.ToString());
        #endregion

        #region Test each way
        var sw = Stopwatch.StartNew();
        var visibleFaces = VisibilityCalculator.GetVisibleFaces(voxels, threshold);
        sw.Stop();

        Console.WriteLine($"Computed visible faces for {sizeX}×{sizeY}×{sizeZ} voxels in {sw.Elapsed.TotalMilliseconds:N2} ms.");


        var packed = VisibilityCalculatorSimd.Pack(voxels, threshold);
        sw = Stopwatch.StartNew();
        var visibleFacesSimd = VisibilityCalculatorSimd.GetVisibleFaces(packed, sizeX, sizeY, sizeZ);
        sw.Stop();
        Console.WriteLine($"Computed visible faces for SIMD : {sizeX}×{sizeY}×{sizeZ} voxels in {sw.Elapsed.TotalMilliseconds:N2} ms.");


        sw = Stopwatch.StartNew();
        var bools = VisibilityCalculatorBinary.ToBools(voxels, threshold);
        var visibleFacesBitOpti = VisibilityCalculatorBinary.GetVisibleFaces(bools);
        sw.Stop();
        Console.WriteLine($"Computed visible faces for Binary optimized in {sw.Elapsed.TotalMilliseconds:N2} ms.");
        #endregion

        #region Checks for validity
        // I base myself on the naive calculator is working fine
        long count = 0;
        bool areEquals = true;
        for (int x = 0; x < sizeX; x++)
            for (int y = 0; y < sizeY; y++)
                for (int z = 0; z < sizeZ; z++)
                    for (int f = 0; f < 6; f++)
                    {
                        if (visibleFaces[x, y, z][f]) count++;
                        if (visibleFaces[x, y, z][f] != visibleFacesBitOpti[x, y, z][f]) areEquals = false;
                    }

        // Console.WriteLine(
        //     $"""
        //     Base :  0 0 0
        //     |   | - | + |
        //     | X | {visibleFaces[0, 0, 0][(int)Face.Xneg]} | {visibleFaces[0, 0, 0][(int)Face.Xpos]} |
        //     | Y | {visibleFaces[0, 0, 0][(int)Face.Yneg]} | {visibleFaces[0, 0, 0][(int)Face.Ypos]} |
        //     | Z | {visibleFaces[0, 0, 0][(int)Face.Zneg]} | {visibleFaces[0, 0, 0][(int)Face.Zpos]} |
        //     """
        // );

        // Console.WriteLine(
        //     $"""
        //     Bits :  0 0 0
        //     |   | - | + |
        //     | X | {visibleFacesBit[0, 0, 0][(int)Face.Xneg]} | {visibleFacesBit[0, 0, 0][(int)Face.Xpos]} |
        //     | Y | {visibleFacesBit[0, 0, 0][(int)Face.Yneg]} | {visibleFacesBit[0, 0, 0][(int)Face.Ypos]} |
        //     | Z | {visibleFacesBit[0, 0, 0][(int)Face.Zneg]} | {visibleFacesBit[0, 0, 0][(int)Face.Zpos]} |
        //     """
        // );


        Console.WriteLine($"Total visible faces: {count}");
        Console.WriteLine($"Are equals : {areEquals}");
        #endregion
    }







}
