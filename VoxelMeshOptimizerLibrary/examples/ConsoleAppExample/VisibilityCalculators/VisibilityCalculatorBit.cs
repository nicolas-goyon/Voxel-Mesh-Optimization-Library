using System.Numerics;
using System.Text;

namespace VoxelVisibility;

public static class VisibilityCalculatorBit
{
    
    
    /// <summary>
    /// Given a 3D array of values [x,y,z] and a threshold,
    /// returns a 3D array of bool[6], indicating for each voxel which faces are visible.
    /// </summary>
    public static bool[,,][] GetVisibleFaces(double[,,] voxels, double threshold = 0.5)
    {
        int sizeX = voxels.GetLength(0);
        int sizeY = voxels.GetLength(1);
        int sizeZ = voxels.GetLength(2);

        // Allocate output: at each [x,y,z] a bool[6]
        var visible = new bool[sizeX, sizeY, sizeZ][];
        var bools = ToBools(voxels, threshold);

        // Bottom-Top
        for (int z = 0; z < sizeZ; z++)
            for (int x = 0; x < sizeX; x++)
            {
                var boolLine = new bool[sizeY];
                for (int y = 0; y < sizeY; y++)
                {
                    boolLine[y] = bools[x, y, z];
                }
                var line = BoolArrayToBigInteger(boolLine);
                var allRightFaces = BigIntegerToBoolArray(ShiftLeftInvertAnd(line), totalBits: sizeY);
                var allLeftFaces = BigIntegerToBoolArray(ShiftRightInvertAnd(line), totalBits: sizeY);


                for (int y = 0; y < sizeY; y++)
                {
                    if (visible[x, y, z] == null) visible[x, y, z] = new bool[6];
                    visible[x, y, z][(int)Face.Yneg] = allLeftFaces[y];
                    visible[x, y, z][(int)Face.Ypos] = allRightFaces[y];
                }
            }

        // Left-right
        for (int y = 0; y < sizeY; y++)
        for (int z = 0; z < sizeZ; z++)
        {
            var boolLine = new bool[sizeX];
            for (int x = 0; x < sizeX; x++)
            {
                boolLine[x] = bools[x, y, z];
            }
            var line = BoolArrayToBigInteger(boolLine);
            // face[x] = line;
            var allRightFaces = BigIntegerToBoolArray(ShiftLeftInvertAnd(line), totalBits: sizeX);
            var allLeftFaces = BigIntegerToBoolArray(ShiftRightInvertAnd(line), totalBits: sizeX);


            for (int x = 0; x < sizeX; x++)
            {

                // Left  (x-1)
                visible[x, y, z][(int)Face.Xneg] = allLeftFaces[x];
                // Right (x+1)
                visible[x, y, z][(int)Face.Xpos] = allRightFaces[x];
            }
        }

        // Left-right
        for (int x = 0; x < sizeX; x++)
        for (int y = 0; y < sizeY; y++)
        {
            var boolLine = new bool[sizeZ];
            for (int z = 0; z < sizeZ; z++)
            {
                boolLine[z] = bools[x, y, z];
            }
            var line = BoolArrayToBigInteger(boolLine);
            // face[x] = line;
            var allRightFaces = BigIntegerToBoolArray(ShiftLeftInvertAnd(line), totalBits: sizeZ);
            var allLeftFaces = BigIntegerToBoolArray(ShiftRightInvertAnd(line), totalBits: sizeZ);


            for (int z = 0; z < sizeZ; z++)
            {
                visible[x, y, z][(int)Face.Zneg] = allLeftFaces[z];
                visible[x, y, z][(int)Face.Zpos] = allRightFaces[z];
            }
        }

        return visible;

    }

    public static bool[,,] ToBools(double[,,] voxels, double threshold = 0.5)
    {
        int sizeX = voxels.GetLength(0);
        int sizeY = voxels.GetLength(1);
        int sizeZ = voxels.GetLength(2);
        var visible = new bool[sizeX, sizeY, sizeZ];

        for (int x = 0; x < sizeX; x++)
            for (int y = 0; y < sizeY; y++)
                for (int z = 0; z < sizeZ; z++)
                {
                    visible[x, y, z] = voxels[x, y, z] > threshold;
                }

        return visible;
    }

    /// <summary>
    /// Packs a bool array into a BigInteger.
    /// dots[0] becomes the highest (most‑significant) bit;
    /// dots[^1] becomes the least‑significant bit.
    /// </summary>
    public static BigInteger BoolArrayToBigInteger(bool[] dots)
    {
        if (dots == null) throw new ArgumentNullException(nameof(dots));

        BigInteger value = BigInteger.Zero;

        for (int i = 0; i < dots.Length; i++)
        {
            if (dots[i])
            {
                // Position from the right‑hand side (LSB = 0)
                int bitIndex = dots.Length - 1 - i;
                value |= BigInteger.One << bitIndex;
            }
        }

        return value;
    }

    /// <summary>
    /// Computes  (x  &  ~ (x << 1))  but clamps the NOT to the operand’s bit‑width
    /// so it behaves like a fixed‑width unsigned integer.
    /// </summary>
    /// <remarks>
    /// The method assumes <paramref name="x"/> is non‑negative.  
    /// For negative numbers you first need to decide how wide the
    /// fixed‑width mask should be.
    /// </remarks>
    public static BigInteger ShiftLeftInvertAnd(BigInteger x)
    {
        if (x.Sign < 0)
            throw new ArgumentOutOfRangeException(nameof(x), "Only non‑negative values are supported.");

        // How many *meaningful* bits does x have?  (.NET 7+/8+ API)
        long bitLen = x.GetBitLength();            // e.g. 0‑based MSB index + 1  :contentReference[oaicite:0]{index=0}

        // After left‑shifting we may have one extra bit, so add 1
        int width = checked((int)(bitLen + 1));

        BigInteger shifted = x << 1;

        // Build a mask like  (1 << width) – 1   to limit the NOT operation
        BigInteger mask = (BigInteger.One << width) - 1;

        BigInteger inverted = (~shifted) & mask;

        return x & inverted;
    }
/// <summary>
    /// Computes  (x  &  ~ (x << 1))  but clamps the NOT to the operand’s bit‑width
    /// so it behaves like a fixed‑width unsigned integer.
    /// </summary>
    /// <remarks>
    /// The method assumes <paramref name="x"/> is non‑negative.  
    /// For negative numbers you first need to decide how wide the
    /// fixed‑width mask should be.
    /// </remarks>
    public static BigInteger ShiftRightInvertAnd(BigInteger x)
    {
        if (x.Sign < 0)
            throw new ArgumentOutOfRangeException(nameof(x), "Only non‑negative values are supported.");

        // How many *meaningful* bits does x have?  (.NET 7+/8+ API)
        long bitLen = x.GetBitLength();            // e.g. 0‑based MSB index + 1  :contentReference[oaicite:0]{index=0}

        // After left‑shifting we may have one extra bit, so add 1
        int width = checked((int)(bitLen + 1));

        BigInteger shifted = x >> 1;

        // Build a mask like  (1 << width) – 1   to limit the NOT operation
        BigInteger mask = (BigInteger.One >> width) - 1;

        BigInteger inverted = (~shifted) & mask;

        return x & inverted;
    }


    /// <summary>
    /// Expands <paramref name="value"/> into a bool[] such that
    /// result[0] is the MSB.  
    /// If <paramref name="totalBits"/> is larger than the value’s
    /// natural width, leading zeros are padded on the left.
    /// </summary>
    public static bool[] BigIntegerToBoolArray(BigInteger value, int totalBits = 0)
    {
        if (value.Sign < 0)
            throw new ArgumentOutOfRangeException(nameof(value), "Only non‑negative values are supported.");

        // .NET 7/8+: exact bit length without the sign bit
        int naturalBits = value.IsZero ? 1 : (int)value.GetBitLength();  // MS docs :contentReference[oaicite:0]{index=0}
        int width       = Math.Max(naturalBits, totalBits);

        var bits = new bool[width];

        for (int i = 0; i < width; i++)
        {
            // i counts from the right (LSB = 0) – we mirror into the array
            bits[width - 1 - i] = ((value >> i) & BigInteger.One) != 0;
        }

        return bits;
    }
}
