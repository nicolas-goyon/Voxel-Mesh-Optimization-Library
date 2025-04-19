using VoxelMeshOptimizer.Core.OcclusionAlgorithms.Common;

namespace VoxelMeshOptimizer.Core.OcclusionAlgorithms;


public interface Occluder
{
    /// <summary>
    /// Computes a collection of 2D planes (slices) in each direction,
    /// representing all visible faces in the chunk. Each plane references the voxels
    /// that have a face visible in that slice and direction.
    /// </summary>
    public VisibleFaces ComputeVisibleFaces();
}
