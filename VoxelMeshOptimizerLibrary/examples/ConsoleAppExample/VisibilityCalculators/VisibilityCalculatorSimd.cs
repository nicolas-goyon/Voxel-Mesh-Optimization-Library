using System;
using System.Numerics;

namespace VoxelVisibility;

public static class VisibilityCalculatorSimd
{
    /// <summary>
    /// Pack a 3D array of doubles into a 1D boolean array where each element
    /// indicates whether the corresponding voxel value is above the given threshold.
    /// The returned array is indexed using <c>x + y * sizeX + z * sizeX * sizeY</c>.
    /// </summary>
    public static bool[] Pack(double[,,] voxels, double threshold = 0.5)
    {
        int sizeX = voxels.GetLength(0);
        int sizeY = voxels.GetLength(1);
        int sizeZ = voxels.GetLength(2);
        long total = (long)sizeX * sizeY * sizeZ;
        if (total > int.MaxValue)
            throw new ArgumentException("Voxel grid too large", nameof(voxels));

        var result = new bool[total];
        int idx = 0;
        for (int z = 0; z < sizeZ; z++)
            for (int y = 0; y < sizeY; y++)
                for (int x = 0; x < sizeX; x++)
                    result[idx++] = voxels[x, y, z] > threshold;
        return result;
    }

    /// <summary>
    /// Compute visible faces using a packed boolean voxel array. The X, Y and Z
    /// dimensions must match those used to pack the array.
    /// </summary>
    public static bool[,,][] GetVisibleFaces(bool[] voxels, int sizeX, int sizeY, int sizeZ)
    {
        long total = (long)sizeX * sizeY * sizeZ;
        if (voxels.Length < total)
            throw new ArgumentException("Voxel array is smaller than expected", nameof(voxels));

        var visible = new bool[sizeX, sizeY, sizeZ][];
        int vecSize = Vector<byte>.Count;
        byte[] currArr = new byte[vecSize];
        byte[] leftArr = new byte[vecSize];
        byte[] rightArr = new byte[vecSize];

        for (int index = 0; index < voxels.Length; index += vecSize)
        {
            int chunk = Math.Min(vecSize, voxels.Length - index);
            for (int i = 0; i < chunk; i++)
            {
                int idx = index + i;
                int x = idx % sizeX;
                int y = (idx / sizeX) % sizeY;
                int z = idx / (sizeX * sizeY);
                currArr[i] = voxels[idx] ? (byte)1 : (byte)0;
                leftArr[i] = GetValue(voxels, x - 1, y, z, sizeX, sizeY, sizeZ) ? (byte)1 : (byte)0;
                rightArr[i] = GetValue(voxels, x + 1, y, z, sizeX, sizeY, sizeZ) ? (byte)1 : (byte)0;
            }
            for (int i = chunk; i < vecSize; i++)
            {
                currArr[i] = 0;
                leftArr[i] = 0;
                rightArr[i] = 0;
            }

            var vCurr = new Vector<byte>(currArr);
            var vLeft = new Vector<byte>(leftArr);
            var vRight = new Vector<byte>(rightArr);

            var mLeftFace = Vector.BitwiseAnd(vCurr, Vector.OnesComplement(vLeft));
            var mRightFace = Vector.BitwiseAnd(vCurr, Vector.OnesComplement(vRight));

            for (int i = 0; i < chunk; i++)
            {
                int idx = index + i;
                int x = idx % sizeX;
                int y = (idx / sizeX) % sizeY;
                int z = idx / (sizeX * sizeY);
                bool inside = currArr[i] != 0;
                bool left = mLeftFace[i] != 0;
                bool right = mRightFace[i] != 0;

                bool bottom = inside && !GetValue(voxels, x, y - 1, z, sizeX, sizeY, sizeZ);
                bool top    = inside && !GetValue(voxels, x, y + 1, z, sizeX, sizeY, sizeZ);
                bool back   = inside && !GetValue(voxels, x, y, z - 1, sizeX, sizeY, sizeZ);
                bool front  = inside && !GetValue(voxels, x, y, z + 1, sizeX, sizeY, sizeZ);

                var faces = new bool[6];
                faces[(int)Face.Left] = left;
                faces[(int)Face.Right] = right;
                faces[(int)Face.Bottom] = bottom;
                faces[(int)Face.Top] = top;
                faces[(int)Face.Back] = back;
                faces[(int)Face.Front] = front;

                visible[x, y, z] = faces;
            }
        }

        return visible;
    }

    private static bool GetValue(bool[] voxels, int x, int y, int z, int sizeX, int sizeY, int sizeZ)
    {
        if (x < 0 || x >= sizeX || y < 0 || y >= sizeY || z < 0 || z >= sizeZ)
            return false;
        long index = (long)x + (long)y * sizeX + (long)z * sizeX * sizeY;
        return voxels[index];
    }
}