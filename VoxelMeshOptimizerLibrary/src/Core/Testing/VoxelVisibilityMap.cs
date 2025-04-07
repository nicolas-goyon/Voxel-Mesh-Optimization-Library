namespace VoxelMeshOptimizer.Core;

public class VoxelVisibilityMap
{
    private VoxelFace[,,] visibilityMap;
    private Chunk<Voxel> chunk;

    public VoxelVisibilityMap(Chunk<Voxel> chunk)
    {
        this.chunk = chunk;
        visibilityMap = new VoxelFace[chunk.Width, chunk.Height, chunk.Depth];
        ComputeVisibilityMap();
    }

    private void ComputeVisibilityMap()
    {
        for (uint x = 0; x < chunk.Width; x++)
        {
            for (uint y = 0; y < chunk.Height; y++)
            {
                for (uint z = 0; z < chunk.Depth; z++)
                {
                    Voxel voxel = chunk.Get(x, y, z);
                    if (voxel == null || !voxel.IsSolid)
                    {
                        visibilityMap[x, y, z] = VoxelFace.None;
                        continue;
                    }

                    VoxelFace visibleFaces = VoxelFace.None;

                    // Check adjacent voxels
                    if (IsAdjacentVoxelTransparent(x, y, z + 1)) visibleFaces |= VoxelFace.Front;
                    if (IsAdjacentVoxelTransparent(x, y, z - 1)) visibleFaces |= VoxelFace.Back;
                    if (IsAdjacentVoxelTransparent(x - 1, y, z)) visibleFaces |= VoxelFace.Left;
                    if (IsAdjacentVoxelTransparent(x + 1, y, z)) visibleFaces |= VoxelFace.Right;
                    if (IsAdjacentVoxelTransparent(x, y + 1, z)) visibleFaces |= VoxelFace.Top;
                    if (IsAdjacentVoxelTransparent(x, y - 1, z)) visibleFaces |= VoxelFace.Bottom;

                    visibilityMap[x, y, z] = visibleFaces;
                }
            }
        }
    }

    private bool IsAdjacentVoxelTransparent(uint x, uint y, uint z)
    {
        Voxel adjacentVoxel = chunk.Get(x, y, z);
        return adjacentVoxel == null || !adjacentVoxel.IsSolid;
    }

    public VoxelFace GetVisibleFaces(uint x, uint y, uint z)
    {
        if (x < 0 || x >= chunk.Width || y < 0 || y >= chunk.Height || z < 0 || z >= chunk.Depth)
            return VoxelFace.None;
        return visibilityMap[x, y, z];
    }
}
