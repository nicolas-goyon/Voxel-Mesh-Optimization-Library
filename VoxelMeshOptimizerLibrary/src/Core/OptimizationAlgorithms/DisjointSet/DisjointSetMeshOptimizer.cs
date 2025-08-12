using CommunityToolkit.Diagnostics;
using VoxelMeshOptimizer.Core.OcclusionAlgorithms;

namespace VoxelMeshOptimizer.Core.OptimizationAlgorithms.DisjointSet;

public class DisjointSetMeshOptimizer : MeshOptimizer
{
    private Mesh mesh;

    public DisjointSetMeshOptimizer(Mesh mesh){
        Guard.IsEmpty(mesh.Quads);
        
        this.mesh = mesh;
    }


    public Mesh Optimize(Chunk<Voxel> chunk)
    {
        var occluder = new VoxelOcclusionOptimizer(chunk);
        var visibileFaces = occluder.ComputeVisibleFaces();

        foreach (var visibleFace in visibileFaces.PlanesByAxis){
            foreach (var face in visibleFace.Value)
            {
                var optimizer = new DisjointSetVisiblePlaneOptimizer(face, chunk);
                optimizer.Optimize();
                var quads = optimizer.ToMeshQuads();
                mesh.Quads.AddRange(quads);
            }
        }

        return mesh;
    }
}
