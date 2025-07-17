using System;

namespace VoxelVisibility;

public static class VisibilityCalculatorBinary
{
    private static void ComputeMasks(ulong[] col, int validBits, ulong[] negMask, ulong[] posMask)
    {
        int len = col.Length;
        ulong carry = 0;
        for (int i = 0; i < len; i++)
        {
            ulong word = col[i];
            ulong shifted = (word << 1) | carry;
            negMask[i] = word & ~shifted;
            carry = word >> 63;
        }
        carry = 0;
        for (int i = len - 1; i >= 0; i--)
        {
            ulong word = col[i];
            ulong shifted = (word >> 1) | (carry << 63);
            posMask[i] = word & ~shifted;
            carry = word & 1UL;
        }

        int rem = validBits & 63;
        if (rem != 0)
        {
            ulong mask = (1UL << rem) - 1UL;
            negMask[len - 1] &= mask;
            posMask[len - 1] &= mask;
        }
    }

    private static bool GetBit(ulong[] arr, int index)
    {
        return (arr[index / 64] & (1UL << (index % 64))) != 0;
    }

    private static void SetBit(ulong[] arr, int index)
    {
        arr[index / 64] |= 1UL << (index % 64);
    }

    public static bool[,,][] GetVisibleFaces(bool[,,] voxels)
    {
        int sizeX = voxels.GetLength(0);
        int sizeY = voxels.GetLength(1);
        int sizeZ = voxels.GetLength(2);

        var visible = new bool[sizeX, sizeY, sizeZ][];

        int segY = (sizeY + 63) / 64;
        var colY = new ulong[segY];
        var negY = new ulong[segY];
        var posY = new ulong[segY];

        for (int z = 0; z < sizeZ; z++)
            for (int x = 0; x < sizeX; x++)
            {
                Array.Clear(colY);
                for (int y = 0; y < sizeY; y++)
                    if (voxels[x, y, z])
                        SetBit(colY, y);

                ComputeMasks(colY, sizeY, negY, posY);

                for (int y = 0; y < sizeY; y++)
                {
                    visible[x, y, z] ??= new bool[6];
                    visible[x, y, z][(int)Face.Yneg] = GetBit(negY, y);
                    visible[x, y, z][(int)Face.Ypos] = GetBit(posY, y);
                }
            }

        int segX = (sizeX + 63) / 64;
        var colX = new ulong[segX];
        var negX = new ulong[segX];
        var posX = new ulong[segX];

        for (int y = 0; y < sizeY; y++)
            for (int z = 0; z < sizeZ; z++)
            {
                Array.Clear(colX);
                for (int x = 0; x < sizeX; x++)
                    if (voxels[x, y, z])
                        SetBit(colX, x);

                ComputeMasks(colX, sizeX, negX, posX);

                for (int x = 0; x < sizeX; x++)
                {
                    visible[x, y, z] ??= new bool[6];
                    visible[x, y, z][(int)Face.Xneg] = GetBit(negX, x);
                    visible[x, y, z][(int)Face.Xpos] = GetBit(posX, x);
                }
            }

        int segZ = (sizeZ + 63) / 64;
        var colZ = new ulong[segZ];
        var negZ = new ulong[segZ];
        var posZ = new ulong[segZ];

        for (int x = 0; x < sizeX; x++)
            for (int y = 0; y < sizeY; y++)
            {
                Array.Clear(colZ);
                for (int z = 0; z < sizeZ; z++)
                    if (voxels[x, y, z])
                        SetBit(colZ, z);

                ComputeMasks(colZ, sizeZ, negZ, posZ);

                for (int z = 0; z < sizeZ; z++)
                {
                    visible[x, y, z] ??= new bool[6];
                    visible[x, y, z][(int)Face.Zneg] = GetBit(negZ, z);
                    visible[x, y, z][(int)Face.Zpos] = GetBit(posZ, z);
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
}