using VoxelMeshOptimizer.Core.OcclusionAlgorithms.Common;

namespace VoxelMeshOptimizer.Core.OcclusionAlgorithms;


public interface Occluder
{
    /// <summary>
    /// Computes and returns a visibility map for the entire chunk,
    /// where each (x,y,z) has a bitmask indicating which faces are visible.
    /// </summary>
    public VoxelFace[,,] ComputeVisibilityMap();

    /// <summary>
    /// Computes a collection of 2D planes (slices) in each direction,
    /// representing all visible faces in the chunk. Each plane references the voxels
    /// that have a face visible in that slice and direction.
    /// </summary>
    public VisibleFaces ComputeVisiblePlanes();
}
