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
    /// Compute visible faces using a packed boolean voxel array with reduced index computations.
    /// This version processes voxels in X-major order and uses SIMD for all six neighbor comparisons.
    /// </summary>
    public static bool[,,][] GetVisibleFacesOptimized(bool[] voxels, int sizeX, int sizeY, int sizeZ)
    {
        long total = (long)sizeX * sizeY * sizeZ;
        if (voxels.Length < total)
            throw new ArgumentException("Voxel array is smaller than expected", nameof(voxels));

        var visible = new bool[sizeX, sizeY, sizeZ][];

        int vecSize = Vector<byte>.Count;
        byte[] curr = new byte[vecSize];
        byte[] left = new byte[vecSize];
        byte[] right = new byte[vecSize];
        byte[] bottom = new byte[vecSize];
        byte[] top = new byte[vecSize];
        byte[] back = new byte[vecSize];
        byte[] front = new byte[vecSize];

        int rowStride = sizeX;
        int sliceStride = sizeX * sizeY;

        for (int z = 0; z < sizeZ; z++)
        {
            for (int y = 0; y < sizeY; y++)
            {
                int baseIndex = z * sliceStride + y * rowStride;
                for (int x = 0; x < sizeX; x += vecSize)
                {
                    int chunk = Math.Min(vecSize, sizeX - x);
                    for (int i = 0; i < chunk; i++)
                    {
                        int idx = baseIndex + x + i;
                        curr[i] = voxels[idx] ? (byte)1 : (byte)0;
                        left[i] = x + i > 0 ? (voxels[idx - 1] ? (byte)1 : (byte)0) : (byte)0;
                        right[i] = x + i < sizeX - 1 ? (voxels[idx + 1] ? (byte)1 : (byte)0) : (byte)0;
                        bottom[i] = y > 0 ? (voxels[idx - rowStride] ? (byte)1 : (byte)0) : (byte)0;
                        top[i] = y < sizeY - 1 ? (voxels[idx + rowStride] ? (byte)1 : (byte)0) : (byte)0;
                        back[i] = z > 0 ? (voxels[idx - sliceStride] ? (byte)1 : (byte)0) : (byte)0;
                        front[i] = z < sizeZ - 1 ? (voxels[idx + sliceStride] ? (byte)1 : (byte)0) : (byte)0;
                    }
                    for (int i = chunk; i < vecSize; i++)
                    {
                        curr[i] = left[i] = right[i] = bottom[i] = top[i] = back[i] = front[i] = 0;
                    }

                    var vCurr = new Vector<byte>(curr);
                    var vLeft = new Vector<byte>(left);
                    var vRight = new Vector<byte>(right);
                    var vBottom = new Vector<byte>(bottom);
                    var vTop = new Vector<byte>(top);
                    var vBack = new Vector<byte>(back);
                    var vFront = new Vector<byte>(front);

                    var vXneg = Vector.BitwiseAnd(vCurr, Vector.OnesComplement(vLeft));
                    var vXpos = Vector.BitwiseAnd(vCurr, Vector.OnesComplement(vRight));
                    var vYneg = Vector.BitwiseAnd(vCurr, Vector.OnesComplement(vBottom));
                    var vYpos = Vector.BitwiseAnd(vCurr, Vector.OnesComplement(vTop));
                    var vZneg = Vector.BitwiseAnd(vCurr, Vector.OnesComplement(vBack));
                    var vZpos = Vector.BitwiseAnd(vCurr, Vector.OnesComplement(vFront));

                    for (int i = 0; i < chunk; i++)
                    {
                        var faces = new bool[6];
                        faces[(int)Face.Xneg] = vXneg[i] != 0;
                        faces[(int)Face.Xpos] = vXpos[i] != 0;
                        faces[(int)Face.Yneg] = vYneg[i] != 0;
                        faces[(int)Face.Ypos] = vYpos[i] != 0;
                        faces[(int)Face.Zneg] = vZneg[i] != 0;
                        faces[(int)Face.Zpos] = vZpos[i] != 0;
                        visible[x + i, y, z] = faces;
                    }
                }
            }
        }

        return visible;
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
                bool top = inside && !GetValue(voxels, x, y + 1, z, sizeX, sizeY, sizeZ);
                bool back = inside && !GetValue(voxels, x, y, z - 1, sizeX, sizeY, sizeZ);
                bool front = inside && !GetValue(voxels, x, y, z + 1, sizeX, sizeY, sizeZ);

                var faces = new bool[6];
                faces[(int)Face.Xneg] = left;
                faces[(int)Face.Xpos] = right;
                faces[(int)Face.Yneg] = bottom;
                faces[(int)Face.Ypos] = top;
                faces[(int)Face.Zneg] = back;
                faces[(int)Face.Zpos] = front;

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