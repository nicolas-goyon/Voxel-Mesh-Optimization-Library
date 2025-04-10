using VoxelMeshOptimizer.Core;

namespace ConsoleAppExample;

public class ExampleMesh : Mesh
{
    public IReadOnlyList<(float x, float y, float z)> Vertices { get; init; } = new List<(float, float, float)>();
    public IReadOnlyList<int> Triangles { get; init; } = new List<int>();
}
