using VoxelMeshOptimizer.Core;

namespace ConsoleAppExample;

public class ExampleMesh : Mesh
{
    public List<MeshQuad> Quads { get; set; }

    public ExampleMesh(List<MeshQuad> quads)
    {
        Quads = quads;
    }

    public ExampleMesh()
    {
        Quads = new();
    }
}
