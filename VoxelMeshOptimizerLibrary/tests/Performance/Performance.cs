using VoxelMeshOptimizer.Core;
using VoxelMeshOptimizer.Core.OcclusionAlgorithms;
using VoxelMeshOptimizer.Core.OptimizationAlgorithms.DisjointSet;
using VoxelMeshOptimizer.Tests.DummyClasses;
using VoxelMeshOptimizer.Toolkit;
using Xunit;

namespace VoxelMeshOptimizer.Tests.Performance;

public class Performance
{
    public static TestChunk Setup()
    {
        var size = 50;
        var voxelsShort = PerlinNoiseChunkGen.CreatePerlinLandscape(size, 123);
        return new TestChunk(voxelsShort);
    }

    private static void ValidateTriangles(Mesh mesh, int expectedTriangles, bool exact)
    {
        var triangleCount = mesh.Quads.Count * 2;
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
        var chunk = Setup();
        var baseMesh = chunk.ToMesh();
        ObjExporter.MeshToObjString(baseMesh);
        ValidateTriangles(baseMesh, 542160, true);

    }

    [Fact]
    public void Occlusion()
    {

        var chunk = Setup();
        var occluder = new VoxelOcclusionOptimizer(chunk);
        var visibileFaces = occluder.ComputeVisibleFaces();
        var occludedQuads = VisibleFacesMesher.Build(visibileFaces, chunk);
        var occludedMesh = new TestMesh(occludedQuads);
        ObjExporter.MeshToObjString(occludedMesh);
        ValidateTriangles(occludedMesh, 25000, false);


    }
    [Fact]
    public void Optimization()
    {
        var chunk = Setup();
        var mesh = new TestMesh();
        var optimizer = new DisjointSetMeshOptimizer(mesh);
        Mesh optimizedMesh = optimizer.Optimize(chunk);
        ObjExporter.MeshToObjString(optimizedMesh);
        ValidateTriangles(optimizedMesh, 5000, false);

    }
}