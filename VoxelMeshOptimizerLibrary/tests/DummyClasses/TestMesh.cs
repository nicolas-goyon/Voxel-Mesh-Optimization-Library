using VoxelMeshOptimizer.Core;

namespace VoxelMeshOptimizer.Tests.DummyClasses;

public class TestMesh : Mesh
{
    public List<MeshQuad> Quads { get; set; }

    public TestMesh(List<MeshQuad> quads)
    {
        Quads = quads;
    }

    public TestMesh()
    {
        Quads = new();
    }
}
