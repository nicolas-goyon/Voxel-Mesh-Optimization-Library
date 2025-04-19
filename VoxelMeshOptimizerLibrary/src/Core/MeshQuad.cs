namespace VoxelMeshOptimizer.Core;
using System.Numerics;

public record MeshQuad
{
    public Vector3 Vertex0 { get; init; } // bottom-left
    public Vector3 Vertex1 { get; init; } // bottom-right
    public Vector3 Vertex2 { get; init; } // top-right
    public Vector3 Vertex3 { get; init; } // top-left

    public Vector3 Normal { get; init; }
    public int VoxelID { get; init; }

}

