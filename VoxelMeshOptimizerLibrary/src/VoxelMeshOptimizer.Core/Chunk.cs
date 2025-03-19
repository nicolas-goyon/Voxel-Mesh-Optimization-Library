namespace VoxelMeshOptimizer.Core;

public interface Chunk
{
    IEnumerable<Voxel> GetVoxels();
}
