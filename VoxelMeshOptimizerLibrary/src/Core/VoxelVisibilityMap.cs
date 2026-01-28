namespace VoxelMeshOptimizer.Core;

public class VoxelVisibilityMap
{
    private readonly VoxelFace[,,] visibilityMap;
    private readonly Chunk chunk;

    public VoxelVisibilityMap(Chunk chunk)
    {
        this.chunk = chunk;
        visibilityMap = new VoxelFace[chunk.XDepth, chunk.YDepth, chunk.ZDepth];
        ComputeVisibilityMap();
    }

    private void ComputeVisibilityMap()
    {
        chunk.ForEachCoordinate(
            Axis.X, AxisOrder.Ascending,
            Axis.Y, AxisOrder.Ascending,
            Axis.Z, AxisOrder.Ascending,
        (x, y, z) => {
            Voxel voxel = chunk.Get(x, y, z);
            if (!voxel.IsSolid)
            {
                visibilityMap[x, y, z] = VoxelFace.None;
                return;
            }

            VoxelFace visibleFaces = VoxelFace.None;

            // Check adjacent voxels
            if (IsAdjacentVoxelTransparent(x, y, z + 1)) visibleFaces |= VoxelFace.Zpos;
            if (IsAdjacentVoxelTransparent(x, y, z - 1)) visibleFaces |= VoxelFace.Zneg;
            if (IsAdjacentVoxelTransparent(x - 1, y, z)) visibleFaces |= VoxelFace.Xneg;
            if (IsAdjacentVoxelTransparent(x + 1, y, z)) visibleFaces |= VoxelFace.Xpos;
            if (IsAdjacentVoxelTransparent(x, y + 1, z)) visibleFaces |= VoxelFace.Ypos;
            if (IsAdjacentVoxelTransparent(x, y - 1, z)) visibleFaces |= VoxelFace.Yneg;

            visibilityMap[x, y, z] = visibleFaces;
        });
    }

    private bool IsAdjacentVoxelTransparent(uint x, uint y, uint z)
    {
        if (chunk.IsOutOfBound(x,y,z)) return true;

        Voxel adjacentVoxel = chunk.Get(x, y, z);
        return !adjacentVoxel.IsSolid;
    }

    public VoxelFace GetVisibleFaces(uint x, uint y, uint z)
    {
        if (x >= chunk.XDepth || y >= chunk.YDepth || z >= chunk.ZDepth)
            return VoxelFace.None;
        return visibilityMap[x, y, z];
    }
}