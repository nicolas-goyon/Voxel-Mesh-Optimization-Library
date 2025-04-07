namespace VoxelMeshOptimizer.Core;

public interface Mesh
{
    IReadOnlyList<(float x, float y, float z)> Vertices { get; }
    IReadOnlyList<int> Triangles { get; }
}
