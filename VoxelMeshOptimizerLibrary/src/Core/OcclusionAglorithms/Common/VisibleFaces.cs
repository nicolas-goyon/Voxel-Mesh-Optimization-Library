namespace VoxelMeshOptimizer.Core.OcclusionAlgorithms.Common;


// The result of the occlusion optimization: all visible planes, organized by direction.
public class VisibleFaces
{
    public Dictionary<Direction, List<VisiblePlane>> PlanesByDirection { get; } 
        = new Dictionary<Direction, List<VisiblePlane>>();
}