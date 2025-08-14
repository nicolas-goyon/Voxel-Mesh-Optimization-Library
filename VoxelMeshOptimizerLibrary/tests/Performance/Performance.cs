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


    [Fact]
    public void Baseline()
    {
        var chunk = Setup();
        var baseMesh = chunk.ToMesh();
        ObjExporter.MeshToObjString(baseMesh);

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


    }
    [Fact]
    public void Optimization()
    {
        var chunk = Setup();
        var mesh = new TestMesh();
        var optimizer = new DisjointSetMeshOptimizer(mesh);
        Mesh optimizedMesh = optimizer.Optimize(chunk);
        ObjExporter.MeshToObjString(optimizedMesh);

    }
}