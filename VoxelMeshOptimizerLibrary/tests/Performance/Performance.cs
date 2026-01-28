using VoxelMeshOptimizer.Core;
using VoxelMeshOptimizer.Core.OcclusionAlgorithms;
using VoxelMeshOptimizer.Core.OcclusionAlgorithms.Common;
using VoxelMeshOptimizer.Core.OptimizationAlgorithms.DisjointSet;
using VoxelMeshOptimizer.Core.Toolkit;
using VoxelMeshOptimizer.Toolkit;
using Xunit;

namespace VoxelMeshOptimizer.Tests.Performance;

public class Performance
{
    public static Chunk Setup()
    {
        int size = 50;
        ushort[,,] voxelsShort = PerlinNoiseChunkGen.CreatePerlinLandscape(size, 123);
        return new Chunk(voxelsShort);
    }

    private static void ValidateTriangles(Mesh mesh, int expectedTriangles, bool exact)
    {
        int triangleCount = mesh.Quads.Count * 2;
        if (exact)
        {
            Assert.Equal(expectedTriangles, triangleCount);
        }
        else
        {
            Assert.True(triangleCount < expectedTriangles,
                $"Expected less than {expectedTriangles} triangles but got {triangleCount}.");
        }
    }


    [Fact]
    public void Baseline()
    {
        Chunk chunk = Setup();
        Mesh baseMesh = chunk.ToMesh();
        ObjExporter.MeshToObjString(baseMesh);
        ValidateTriangles(baseMesh, 542160, true);

    }

    [Fact]
    public void Occlusion()
    {

        Chunk chunk = Setup();
        VoxelOcclusionOptimizer occluder = new VoxelOcclusionOptimizer(chunk);
        VisibleFaces visibileFaces = occluder.ComputeVisibleFaces();
        List<MeshQuad> occludedQuads = VisibleFacesMesher.Build(visibileFaces, chunk);
        Mesh occludedMesh = new Mesh(occludedQuads);
        ObjExporter.MeshToObjString(occludedMesh);
        ValidateTriangles(occludedMesh, 25000, false);


    }
    [Fact]
    public void Optimization()
    {
        Chunk chunk = Setup();
        Mesh mesh = new();
        DisjointSetMeshOptimizer optimizer = new(mesh);
        Mesh optimizedMesh = optimizer.Optimize(chunk);
        ObjExporter.MeshToObjString(optimizedMesh);
        ValidateTriangles(optimizedMesh, 5000, false);

    }
}