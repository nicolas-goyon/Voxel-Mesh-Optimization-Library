using System.Diagnostics;
using System.Numerics;

namespace ConsoleAppExample;

public class SimdExample
{
    [Flags]
    private enum VoxelFace
    {
        None = 0,
        Xneg = 1,
        Xpos = 2,
        Yneg = 4,
        Ypos = 8,
        Zneg = 16,
        Zpos = 32
    }

    public static void Run()
    {
        const int sizeX = 640;
        const int sizeY = 640;
        const int sizeZ = 640;
        const float airThreshold = 0.5f;

        int volume = sizeX * sizeY * sizeZ;
        var rand = new Random(42);

        var voxels = new float[volume];
        for (int i = 0; i < volume; i++)
            voxels[i] = (float)rand.NextDouble();

        var swScalar = Stopwatch.StartNew();
        var scalar = ComputeVisibilityScalar(voxels, sizeX, sizeY, sizeZ, airThreshold);
        swScalar.Stop();

        var swSimd = Stopwatch.StartNew();
        var simd = ComputeVisibilitySimd(voxels, sizeX, sizeY, sizeZ, airThreshold);
        swSimd.Stop();

        bool equal = scalar.SequenceEqual(simd);
        Console.WriteLine($"Results equal: {equal}");
        Console.WriteLine($"Scalar Time: {swScalar.ElapsedMilliseconds} ms");
        Console.WriteLine($"SIMD Time:   {swSimd.ElapsedMilliseconds} ms");
    }

    private static VoxelFace[] ComputeVisibilityScalar(float[] values, int sx, int sy, int sz, float threshold)
    {
        int strideY = sx;
        int strideZ = sx * sy;
        var result = new VoxelFace[values.Length];

        for (int z = 0; z < sz; z++)
        {
            for (int y = 0; y < sy; y++)
            {
                for (int x = 0; x < sx; x++)
                {
                    int index = x + y * strideY + z * strideZ;
                    float current = values[index];
                    if (current <= threshold) continue;

                    if (x == 0 || values[index - 1] <= threshold)
                        result[index] |= VoxelFace.Xneg;
                    if (x == sx - 1 || values[index + 1] <= threshold)
                        result[index] |= VoxelFace.Xpos;

                    if (y == 0 || values[index - strideY] <= threshold)
                        result[index] |= VoxelFace.Yneg;
                    if (y == sy - 1 || values[index + strideY] <= threshold)
                        result[index] |= VoxelFace.Ypos;

                    if (z == 0 || values[index - strideZ] <= threshold)
                        result[index] |= VoxelFace.Zneg;
                    if (z == sz - 1 || values[index + strideZ] <= threshold)
                        result[index] |= VoxelFace.Zpos;
                }
            }
        }

        return result;
    }

    private static VoxelFace[] ComputeVisibilitySimd(float[] values, int sx, int sy, int sz, float threshold)
    {
        int strideY = sx;
        int strideZ = sx * sy;
        var result = new VoxelFace[values.Length];

        var threshVec = new Vector<float>(threshold);
        int width = Vector<float>.Count;
        int[] buffer = new int[width];

        // X positive faces
        for (int z = 0; z < sz; z++)
        {
            for (int y = 0; y < sy; y++)
            {
                int baseIndex = (z * sy + y) * sx;
                int x = 0;
                for (; x <= sx - 1 - width; x += width)
                {
                    int idx = baseIndex + x;
                    var current = new Vector<float>(values, idx);
                    var next = new Vector<float>(values, idx + 1);
                    var mask = Vector.BitwiseAnd(Vector.GreaterThan(current, threshVec), Vector.LessThan(next, threshVec));
                    mask.CopyTo(buffer);
                    for (int k = 0; k < width; k++)
                        if (buffer[k] != 0) result[idx + k] |= VoxelFace.Xpos;
                }
                for (; x < sx - 1; x++)
                {
                    int idx = baseIndex + x;
                    if (values[idx] > threshold && values[idx + 1] <= threshold)
                        result[idx] |= VoxelFace.Xpos;
                }
                if (values[baseIndex + sx - 1] > threshold)
                    result[baseIndex + sx - 1] |= VoxelFace.Xpos;
            }
        }

        // X negative faces
        for (int z = 0; z < sz; z++)
        {
            for (int y = 0; y < sy; y++)
            {
                int baseIndex = (z * sy + y) * sx;
                int x = 1;
                for (; x <= sx - width; x += width)
                {
                    int idx = baseIndex + x;
                    var current = new Vector<float>(values, idx);
                    var prev = new Vector<float>(values, idx - 1);
                    var mask = Vector.BitwiseAnd(Vector.GreaterThan(current, threshVec), Vector.LessThan(prev, threshVec));
                    mask.CopyTo(buffer);
                    for (int k = 0; k < width; k++)
                        if (buffer[k] != 0) result[idx + k] |= VoxelFace.Xneg;
                }
                for (; x < sx; x++)
                {
                    int idx = baseIndex + x;
                    if (values[idx] > threshold && values[idx - 1] <= threshold)
                        result[idx] |= VoxelFace.Xneg;
                }
                if (values[baseIndex] > threshold)
                    result[baseIndex] |= VoxelFace.Xneg;
            }
        }

        // Y positive faces
        for (int z = 0; z < sz; z++)
        {
            int slice = z * strideZ;
            for (int y = 0; y < sy - 1; y++)
            {
                int row = slice + y * sx;
                int nextRow = row + sx;
                int x = 0;
                for (; x <= sx - width; x += width)
                {
                    int idx = row + x;
                    var current = new Vector<float>(values, idx);
                    var next = new Vector<float>(values, nextRow + x);
                    var mask = Vector.BitwiseAnd(Vector.GreaterThan(current, threshVec), Vector.LessThan(next, threshVec));
                    mask.CopyTo(buffer);
                    for (int k = 0; k < width; k++)
                        if (buffer[k] != 0) result[idx + k] |= VoxelFace.Ypos;
                }
                for (; x < sx; x++)
                {
                    int idx = row + x;
                    if (values[idx] > threshold && values[nextRow + x] <= threshold)
                        result[idx] |= VoxelFace.Ypos;
                }
            }

            int lastRow = slice + (sy - 1) * sx;
            for (int x = 0; x < sx; x++)
                if (values[lastRow + x] > threshold)
                    result[lastRow + x] |= VoxelFace.Ypos;
        }

        // Y negative faces
        for (int z = 0; z < sz; z++)
        {
            int slice = z * strideZ;
            for (int y = 1; y < sy; y++)
            {
                int row = slice + y * sx;
                int prevRow = row - sx;
                int x = 0;
                for (; x <= sx - width; x += width)
                {
                    int idx = row + x;
                    var current = new Vector<float>(values, idx);
                    var prev = new Vector<float>(values, prevRow + x);
                    var mask = Vector.BitwiseAnd(Vector.GreaterThan(current, threshVec), Vector.LessThan(prev, threshVec));
                    mask.CopyTo(buffer);
                    for (int k = 0; k < width; k++)
                        if (buffer[k] != 0) result[idx + k] |= VoxelFace.Yneg;
                }
                for (; x < sx; x++)
                {
                    int idx = row + x;
                    if (values[idx] > threshold && values[prevRow + x] <= threshold)
                        result[idx] |= VoxelFace.Yneg;
                }
            }

            int firstRow = slice;
            for (int x = 0; x < sx; x++)
                if (values[firstRow + x] > threshold)
                    result[firstRow + x] |= VoxelFace.Yneg;
        }

        // Z positive faces
        for (int z = 0; z < sz - 1; z++)
        {
            int slice = z * strideZ;
            int nextSlice = slice + strideZ;
            for (int y = 0; y < sy; y++)
            {
                int row = slice + y * sx;
                int nextRow = nextSlice + y * sx;
                int x = 0;
                for (; x <= sx - width; x += width)
                {
                    int idx = row + x;
                    var current = new Vector<float>(values, idx);
                    var next = new Vector<float>(values, nextRow + x);
                    var mask = Vector.BitwiseAnd(Vector.GreaterThan(current, threshVec), Vector.LessThan(next, threshVec));
                    mask.CopyTo(buffer);
                    for (int k = 0; k < width; k++)
                        if (buffer[k] != 0) result[idx + k] |= VoxelFace.Zpos;
                }
                for (; x < sx; x++)
                {
                    int idx = row + x;
                    if (values[idx] > threshold && values[nextRow + x] <= threshold)
                        result[idx] |= VoxelFace.Zpos;
                }
            }
        }

        int lastSlice = (sz - 1) * strideZ;
        for (int y = 0; y < sy; y++)
            for (int x = 0; x < sx; x++)
                if (values[lastSlice + y * sx + x] > threshold)
                    result[lastSlice + y * sx + x] |= VoxelFace.Zpos;

        // Z negative faces
        for (int z = 1; z < sz; z++)
        {
            int slice = z * strideZ;
            int prevSlice = slice - strideZ;
            for (int y = 0; y < sy; y++)
            {
                int row = slice + y * sx;
                int prevRow = prevSlice + y * sx;
                int x = 0;
                for (; x <= sx - width; x += width)
                {
                    int idx = row + x;
                    var current = new Vector<float>(values, idx);
                    var prev = new Vector<float>(values, prevRow + x);
                    var mask = Vector.BitwiseAnd(Vector.GreaterThan(current, threshVec), Vector.LessThan(prev, threshVec));
                    mask.CopyTo(buffer);
                    for (int k = 0; k < width; k++)
                        if (buffer[k] != 0) result[idx + k] |= VoxelFace.Zneg;
                }
                for (; x < sx; x++)
                {
                    int idx = row + x;
                    if (values[idx] > threshold && values[prevRow + x] <= threshold)
                        result[idx] |= VoxelFace.Zneg;
                }
            }
        }

        for (int y = 0; y < sy; y++)
            for (int x = 0; x < sx; x++)
                if (values[y * sx + x] > threshold)
                    result[y * sx + x] |= VoxelFace.Zneg;

        return result;
    }
}