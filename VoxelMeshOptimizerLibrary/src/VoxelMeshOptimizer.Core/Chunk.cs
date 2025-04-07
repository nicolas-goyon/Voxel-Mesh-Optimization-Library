namespace VoxelMeshOptimizer.Core;

public interface Chunk<out T> where T : Voxel
{
    uint Width {get;}
    uint Height {get;}
    uint Depth {get;}

    IEnumerable<T> GetVoxels();

    T Get(uint x, uint y, uint z);
}
