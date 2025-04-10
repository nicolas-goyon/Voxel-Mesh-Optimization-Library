namespace VoxelMeshOptimizer.Core.OcclusionAlgorithms.Common;


// The result of the occlusion optimization: all visible planes, organized by direction.
public class VisibleFaces
{
    public Dictionary<(Axis, AxisOrder), List<VisiblePlane>> PlanesByAxis { get; }
        = new Dictionary<(Axis, AxisOrder), List<VisiblePlane>>();
}