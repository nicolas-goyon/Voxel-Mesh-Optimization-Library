using System;
using System.Numerics;

namespace VoxelVisibility
{
    public static class VisibilityCalculatorSimd
    {
        /// <summary>
        /// Given a 3D array of values [x,y,z] and a threshold,
        /// returns a 3D array of bool[6], indicating for each voxel which faces are visible.
        /// This implementation processes X in SIMD lanes of Vector<double>.Count.
        /// </summary>
        public static bool[,,][] GetVisibleFaces(double[,,] voxels, double threshold = 0.5)
        {
            int sizeX = voxels.GetLength(0);
            int sizeY = voxels.GetLength(1);
            int sizeZ = voxels.GetLength(2);

            var visible = new bool[sizeX, sizeY, sizeZ][];            
            int vecSize = Vector<double>.Count;
            var threshVec = new Vector<double>(threshold);

            // Allocate temporary buffers for neighbor loads
            double[] currArr = new double[vecSize];
            double[] leftArr = new double[vecSize];
            double[] rightArr = new double[vecSize];
            
            for (int y = 0; y < sizeY; y++)
            for (int z = 0; z < sizeZ; z++)
            {
                // Precompute scalar neighbor rows for y±1, z±1
                for (int x = 0; x < sizeX; x++)
                    visible[x, y, z] = new bool[6];

                for (int x = 0; x < sizeX; x += vecSize)
                {
                    int chunk = Math.Min(vecSize, sizeX - x);

                    // Load current voxels into currArr
                    for (int i = 0; i < chunk; i++)
                        currArr[i] = voxels[x + i, y, z];
                    // For out‐of‐bounds neighbors, fill with 0.0
                    for (int i = chunk; i < vecSize; i++)
                        currArr[i] = 0.0;

                    // Load left neighbor
                    for (int i = 0; i < chunk; i++)
                    {
                        int xi = x + i - 1;
                        leftArr[i] = (xi >= 0) ? voxels[xi, y, z] : 0.0;
                    }
                    for (int i = chunk; i < vecSize; i++)
                        leftArr[i] = 0.0;

                    // Load right neighbor
                    for (int i = 0; i < chunk; i++)
                    {
                        int xi = x + i + 1;
                        rightArr[i] = (xi < sizeX) ? voxels[xi, y, z] : 0.0;
                    }
                    for (int i = chunk; i < vecSize; i++)
                        rightArr[i] = 0.0;

                    // Create vectors
                    var vCurr  = new Vector<double>(currArr);
                    var vLeft  = new Vector<double>(leftArr);
                    var vRight = new Vector<double>(rightArr);

                    // Masks: inside = curr > threshold
                    var mInside = Vector.GreaterThan(vCurr, threshVec);

                    // Along X:
                    var mLeftFace  = Vector.BitwiseAnd(mInside, Vector.LessThan(vLeft, threshVec));
                    var mRightFace = Vector.BitwiseAnd(mInside, Vector.LessThan(vRight, threshVec));

                    // Along Y and Z: scalar comparison per lane
                    for (int i = 0; i < chunk; i++)
                    {
                        int xi = x + i;
                        bool inside = currArr[i] > threshold;

                        // Bottom (y-1) and Top (y+1)
                        bool bottom = inside && GetValue(voxels, xi, y - 1, z, sizeX, sizeY, sizeZ) < threshold;
                        bool top    = inside && GetValue(voxels, xi, y + 1, z, sizeX, sizeY, sizeZ) < threshold;

                        // Back (z-1) and Front (z+1)
                        bool back  = inside && GetValue(voxels, xi, y, z - 1, sizeX, sizeY, sizeZ) < threshold;
                        bool front = inside && GetValue(voxels, xi, y, z + 1, sizeX, sizeY, sizeZ) < threshold;

                        // Pack vector lanes for left & right
                        bool left  = mLeftFace[i] != 0.0;
                        bool right = mRightFace[i] != 0.0;

                        var faces = new bool[6];
                        faces[(int)Face.Left]   = left;
                        faces[(int)Face.Right]  = right;
                        faces[(int)Face.Bottom] = bottom;
                        faces[(int)Face.Top]    = top;
                        faces[(int)Face.Back]   = back;
                        faces[(int)Face.Front]  = front;

                        visible[xi, y, z] = faces;
                    }
                }
            }

            return visible;
        }

        /// <summary>
        /// Safely get a voxel value, returning 0 if out of bounds.
        /// </summary>
        private static double GetValue(double[,,] voxels, int x, int y, int z, int sizeX, int sizeY, int sizeZ)
        {
            if (x < 0 || x >= sizeX ||
                y < 0 || y >= sizeY ||
                z < 0 || z >= sizeZ)
                return 0.0; 

            return voxels[x, y, z];
        }
    }
}
