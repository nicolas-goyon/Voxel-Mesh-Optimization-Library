using System.Text;

namespace VoxelMeshOptimizer.Core;

public class Mesh
{
    public List<MeshQuad> Quads { get; set; }

    public Mesh(List<MeshQuad> quads)
    {
        Quads = quads;

    }

    public Mesh()
    {
        Quads = [];
    }

    public string Describe()
    {
        StringBuilder sb = new();
        sb.AppendLine($"Mesh of {Quads.Count} quads :\n");
        foreach (MeshQuad meshQuad in Quads)
        {
            sb.Append(meshQuad.Describe());
            sb.AppendLine();
        }
        return sb.ToString();
    }
}