namespace VoxelMeshOptimizer.Core.OcclusionAlgorithms.Common;

// Each 2D slice in a given direction, containing references to visible voxels.
public class VisiblePlane
{
    public Direction Direction { get; }
    public uint SliceIndex { get; }
    public Voxel[,] Voxels { get; }

    public VisiblePlane(Direction direction, uint sliceIndex, uint width, uint height)
    {
        Direction = direction;
        SliceIndex = sliceIndex;
        Voxels = new Voxel[width, height];
    }
}