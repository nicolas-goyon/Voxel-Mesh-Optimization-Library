namespace VoxelMeshOptimizer.Core;
using System.Numerics;

public struct MeshQuad
{
    public Vector3 Vertex0 { get; init; } // bottom-left
    public Vector3 Vertex1 { get; init; } // bottom-right
    public Vector3 Vertex2 { get; init; } // top-right
    public Vector3 Vertex3 { get; init; } // top-left

    public Vector3 Normal { get; init; }
    public int VoxelId { get; init; }



    public string Describe()
    {
        return $"MeshQuad : V0 {Vertex0} ; V1 {Vertex1}; V2 {Vertex2}; V3 {Vertex3}; N {Normal}; ID {VoxelId}";
    }

}

