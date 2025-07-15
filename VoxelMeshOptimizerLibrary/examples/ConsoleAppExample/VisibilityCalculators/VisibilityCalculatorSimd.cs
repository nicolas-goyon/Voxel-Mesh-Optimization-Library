using System;
using System.Numerics;

namespace VoxelVisibility;

public static class VisibilityCalculatorSimd
{
    /// <summary>
    /// Convert a standard 3D voxel array into a vectorized representation along the X axis.
    /// Padding is added so that the X dimension becomes a multiple of <see cref="Vector{Double}.Count"/>.
    /// </summary>
    public static Vector<double>[,,] Pack(double[,,] voxels)
    {
        int sizeX = voxels.GetLength(0);
        int sizeY = voxels.GetLength(1);
        int sizeZ = voxels.GetLength(2);

        int vecSize = Vector<double>.Count;
        int sizeXVec = (sizeX + vecSize - 1) / vecSize;
        var result = new Vector<double>[sizeXVec, sizeY, sizeZ];
        double[] buffer = new double[vecSize];

        for (int y = 0; y < sizeY; y++)
        for (int z = 0; z < sizeZ; z++)
        {
            for (int vx = 0; vx < sizeXVec; vx++)
            {
                int baseX = vx * vecSize;
                for (int lane = 0; lane < vecSize; lane++)
                {
                    int xi = baseX + lane;
                    buffer[lane] = xi < sizeX ? voxels[xi, y, z] : 0.0;
                }
                result[vx, y, z] = new Vector<double>(buffer);
            }
        }

        return result;
    }

    /// <summary>
    /// Given a vectorized voxel array and the original X dimension, compute the visible faces.
    /// </summary>
    public static bool[,,][] GetVisibleFaces(Vector<double>[,,] voxels, int sizeX, double threshold = 0.5)
    {
        int vecSize = Vector<double>.Count;
        int sizeXVec = voxels.GetLength(0);
        int sizeY = voxels.GetLength(1);
        int sizeZ = voxels.GetLength(2);

        var visible = new bool[sizeX, sizeY, sizeZ][];
        var threshVec = new Vector<double>(threshold);

        double[] scratch = new double[vecSize];

        for (int y = 0; y < sizeY; y++)
        for (int z = 0; z < sizeZ; z++)
        {
            for (int vx = 0; vx < sizeXVec; vx++)
            {
                Vector<double> curr = voxels[vx, y, z];
                Vector<double> prev = vx > 0 ? voxels[vx - 1, y, z] : Vector<double>.Zero;
                Vector<double> next = vx < sizeXVec - 1 ? voxels[vx + 1, y, z] : Vector<double>.Zero;

                // Shift to obtain neighbours in the X direction
                Vector<double> leftVec = ShiftRightOne(curr, prev, scratch);
                Vector<double> rightVec = ShiftLeftOne(curr, next, scratch);

                var mInside = Vector.GreaterThan(curr, threshVec);
                var mLeftFace = Vector.BitwiseAnd(mInside, Vector.LessThan(leftVec, threshVec));
                var mRightFace = Vector.BitwiseAnd(mInside, Vector.LessThan(rightVec, threshVec));

                for (int lane = 0; lane < vecSize; lane++)
                {
                    int xi = vx * vecSize + lane;
                    if (xi >= sizeX)
                        break;

                    bool inside = mInside[lane] != 0.0;
                    bool left = mLeftFace[lane] != 0.0;
                    bool right = mRightFace[lane] != 0.0;

                    bool bottom = inside && GetValue(voxels, xi, y - 1, z, sizeX, sizeY, sizeZ) < threshold;
                    bool top    = inside && GetValue(voxels, xi, y + 1, z, sizeX, sizeY, sizeZ) < threshold;
                    bool back   = inside && GetValue(voxels, xi, y, z - 1, sizeX, sizeY, sizeZ) < threshold;
                    bool front  = inside && GetValue(voxels, xi, y, z + 1, sizeX, sizeY, sizeZ) < threshold;

                    var faces = new bool[6];
                    faces[(int)Face.Left] = left;
                    faces[(int)Face.Right] = right;
                    faces[(int)Face.Bottom] = bottom;
                    faces[(int)Face.Top] = top;
                    faces[(int)Face.Back] = back;
                    faces[(int)Face.Front] = front;

                    visible[xi, y, z] = faces;
                }
            }
        }

        return visible;
    }

    private static Vector<double> ShiftLeftOne(Vector<double> current, Vector<double> next, double[] scratch)
    {
        current.CopyTo(scratch);
        for (int i = 0; i < scratch.Length - 1; i++)
            scratch[i] = scratch[i + 1];
        scratch[scratch.Length - 1] = next[0];
        return new Vector<double>(scratch);
    }

    private static Vector<double> ShiftRightOne(Vector<double> current, Vector<double> prev, double[] scratch)
    {
        current.CopyTo(scratch);
        for (int i = scratch.Length - 1; i > 0; i--)
            scratch[i] = scratch[i - 1];
        scratch[0] = prev[scratch.Length - 1];
        return new Vector<double>(scratch);
    }

    private static double GetValue(Vector<double>[,,] voxels, int x, int y, int z, int sizeX, int sizeY, int sizeZ)
    {
        if (x < 0 || x >= sizeX || y < 0 || y >= sizeY || z < 0 || z >= sizeZ)
            return 0.0;

        int vecSize = Vector<double>.Count;
        int vx = x / vecSize;
        int lane = x % vecSize;
        return voxels[vx, y, z][lane];
    }
}