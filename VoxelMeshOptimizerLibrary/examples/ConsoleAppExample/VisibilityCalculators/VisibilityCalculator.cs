namespace VoxelVisibility;

public static class VisibilityCalculator
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

        for (int x = 0; x < sizeX; x++)
            for (int y = 0; y < sizeY; y++)
                for (int z = 0; z < sizeZ; z++)
                {
                    bool isBlock = voxels[x, y, z] > threshold;
                    var faces = new bool[6];

                    // Left  (x-1)
                    faces[(int)Face.Xneg] = isBlock && GetValue(voxels, x - 1, y, z, sizeX, sizeY, sizeZ) < threshold;
                    // Right (x+1)
                    faces[(int)Face.Xpos] = isBlock && GetValue(voxels, x + 1, y, z, sizeX, sizeY, sizeZ) < threshold;
                    // Bottom (y-1)
                    faces[(int)Face.Yneg] = isBlock && GetValue(voxels, x, y - 1, z, sizeX, sizeY, sizeZ) < threshold;
                    // Top    (y+1)
                    faces[(int)Face.Ypos] = isBlock && GetValue(voxels, x, y + 1, z, sizeX, sizeY, sizeZ) < threshold;
                    // Back   (z-1)
                    faces[(int)Face.Zneg] = isBlock && GetValue(voxels, x, y, z - 1, sizeX, sizeY, sizeZ) < threshold;
                    // Front  (z+1)
                    faces[(int)Face.Zpos] = isBlock && GetValue(voxels, x, y, z + 1, sizeX, sizeY, sizeZ) < threshold;

                    visible[x, y, z] = faces;
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
        {
            return 0.0; // outside treated as empty
        }

        return voxels[x, y, z];
    }
}