namespace VoxelMeshOptimizer.Core.OcclusionAlgorithms.Common;
// Directions used when building planes.
public enum Direction
{
    Front,   // Looking along +Z axis
    Back,    // Looking along -Z axis
    Left,    // Looking along -X axis
    Right,   // Looking along +X axis
    Top,     // Looking along +Y axis
    Bottom   // Looking along -Y axis
}