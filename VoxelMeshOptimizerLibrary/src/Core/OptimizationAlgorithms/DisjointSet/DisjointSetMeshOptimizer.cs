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
            Console.WriteLine($"Axis : {visibleFace.Key.Item1}, AxisOrder : {visibleFace.Key.Item2}");
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
