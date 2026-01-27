namespace VoxelMeshOptimizer.Core;

public interface MeshOptimizer
{
    public Mesh Optimize(Chunk chunk);
}
