using CommunityToolkit.Diagnostics;
using VoxelMeshOptimizer.Core.OcclusionAlgorithms;
using VoxelMeshOptimizer.Core.OcclusionAlgorithms.Common;

namespace VoxelMeshOptimizer.Core.OptimizationAlgorithms.DisjointSet;

public class DisjointSetMeshOptimizer
{
    private Mesh mesh;

    public DisjointSetMeshOptimizer(Mesh mesh){
        Guard.IsEmpty(mesh.Quads);
        
        this.mesh = mesh;
    }


    public Mesh Optimize(Chunk chunk)
    {
        VoxelOcclusionOptimizer occluder = new(chunk);
        VisibleFaces visibileFaces = occluder.ComputeVisibleFaces();

        foreach (DisjointSetVisiblePlaneOptimizer? optimizer in from visibleFace in visibileFaces.PlanesByAxis from face in visibleFace.Value select new DisjointSetVisiblePlaneOptimizer(face, chunk))
        {
            optimizer.Optimize();
            List<MeshQuad> quads = optimizer.ToMeshQuads();
            mesh.Quads.AddRange(quads);
        }

        return mesh;
    }
}
