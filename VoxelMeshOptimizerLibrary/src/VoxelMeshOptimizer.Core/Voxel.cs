namespace VoxelMeshOptimizer.Core;

public interface Voxel
{
    int Color { get; }
    (int x, int y, int z) Position { get; }
}
