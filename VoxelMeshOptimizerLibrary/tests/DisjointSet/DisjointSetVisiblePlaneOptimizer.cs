using Xunit;
using VoxelMeshOptimizer.Core.OptimizationAlgorithms.DisjointSet;
using VoxelMeshOptimizer.Core.OcclusionAlgorithms.Common;
using VoxelMeshOptimizer.Core;
using VoxelMeshOptimizer.Tests.DummyClasses;
using System.Numerics;

namespace VoxelMeshOptimizer.Tests.DisjointSetTesting;

public partial class DisjointSetVisiblePlaneOptimizerTests
{
    private static VisiblePlane CreatePlaneFromIds(ushort?[,] ids)
    {
        int w = ids.GetLength(0), h = ids.GetLength(1);
        var plane = new VisiblePlane(
            Axis.X, AxisOrder.Ascending,
            Axis.Y, AxisOrder.Ascending,
            Axis.Z, AxisOrder.Ascending,
            0, (uint)w, (uint)h
        );
        for (int x = 0; x < w; x++)
            for (int y = 0; y < h; y++)
                if (ids[x, y].HasValue)
                    plane.Voxels[x, y] = new TestVoxel(ids[x, y].Value, true);
        return plane;
    }

    #region ToMeshQuads (Refactored from ToResult)

    [Fact]
    public void ToMeshQuads_ShouldCreateSingleQuad_ForUniformBlock()
    {
        ushort?[,] ids = {
        {1, 1},
        {1, 1}
    };

        var plane = CreatePlaneFromIds(ids);
        var optimizer = new VoxelMeshOptimizer.Core.OptimizationAlgorithms.DisjointSet.DisjointSetVisiblePlaneOptimizer(plane);
        optimizer.Optimize();

        var quads = optimizer.ToMeshQuads();

        Assert.Single(quads);
        Assert.Equal(1, quads[0].VoxelID);
    }

    [Fact]
    public void ToMeshQuads_ShouldCreateOneQuadPerUniqueVoxel()
    {
        ushort?[,] ids = {
        {1, 2},
        {3, 4}
    };

        var plane = CreatePlaneFromIds(ids);
        var optimizer = new VoxelMeshOptimizer.Core.OptimizationAlgorithms.DisjointSet.DisjointSetVisiblePlaneOptimizer(plane);
        optimizer.Optimize();

        var quads = optimizer.ToMeshQuads();
        Assert.Equal(4, quads.Count);
        Assert.Contains(quads, q => q.VoxelID == 1);
        Assert.Contains(quads, q => q.VoxelID == 2);
        Assert.Contains(quads, q => q.VoxelID == 3);
        Assert.Contains(quads, q => q.VoxelID == 4);
    }

    [Fact]
    public void ToMeshQuads_ShouldCreateMultipleQuads_ForGroupedRegions()
    {
        ushort?[,] ids = {
        {1, 1, 2, 2},
        {1, 1, 2, 2},
        {3, 3, 4, 4},
        {3, 3, 4, 4}
    };

        var plane = CreatePlaneFromIds(ids);
        var optimizer = new VoxelMeshOptimizer.Core.OptimizationAlgorithms.DisjointSet.DisjointSetVisiblePlaneOptimizer(plane);
        optimizer.Optimize();

        var quads = optimizer.ToMeshQuads();
        Assert.Equal(4, quads.Count);

        Assert.Equal(1, quads.Count(q => q.VoxelID == 1));
        Assert.Equal(1, quads.Count(q => q.VoxelID == 2));
        Assert.Equal(1, quads.Count(q => q.VoxelID == 3));
        Assert.Equal(1, quads.Count(q => q.VoxelID == 4));
    }

    [Fact]
    public void ToMeshQuads_ShouldHandleSinglePixelGroups_AsIndividualQuads()
    {
        ushort?[,] ids = {
        {1, 2},
        {1, 3}
    };

        var plane = CreatePlaneFromIds(ids);
        var optimizer = new VoxelMeshOptimizer.Core.OptimizationAlgorithms.DisjointSet.DisjointSetVisiblePlaneOptimizer(plane);
        optimizer.Optimize();

        var quads = optimizer.ToMeshQuads();

        Assert.Equal(3, quads.Count);
        Assert.Equal(1, quads.Count(q => q.VoxelID == 1)); // (0,0) and (0,1)
        Assert.Equal(1, quads.Count(q => q.VoxelID == 2));
        Assert.Equal(1, quads.Count(q => q.VoxelID == 3));
    }

    [Fact]
    public void ToMeshQuads_ShouldAvoidIncorrectMerging_WhenShapeIsNotRectangle()
    {
        ushort?[,] ids = {
        {1, 1, 2},
        {1, 2, 2},
        {2, 2, 2}
    };

        var plane = CreatePlaneFromIds(ids);
        var optimizer = new VoxelMeshOptimizer.Core.OptimizationAlgorithms.DisjointSet.DisjointSetVisiblePlaneOptimizer(plane);
        optimizer.Optimize();

        var quads = optimizer.ToMeshQuads();

        // We expect this complex layout to break into several regions
        Assert.True(quads.Count >= 4 && quads.Count <= 6); // Approximate depending on merges
        Assert.Contains(quads, q => q.VoxelID == 1);
        Assert.Contains(quads, q => q.VoxelID == 2);
    }

    [Fact]
    public void ToMeshQuads_SinglePixel_ReturnsSingleQuad()
    {
        ushort?[,] ids = { { 1 } };
        var plane = CreatePlaneFromIds(ids);
        var optimizer = new VoxelMeshOptimizer.Core.OptimizationAlgorithms.DisjointSet.DisjointSetVisiblePlaneOptimizer(plane);
        optimizer.Optimize();

        var quads = optimizer.ToMeshQuads();
        Assert.Single(quads);
        Assert.Equal(1, quads[0].VoxelID);
    }

    [Fact]
    public void ToMeshQuads_TwoDifferentPixels_CreatesTwoQuads()
    {
        ushort?[,] ids = { { 1, 2 } };
        var plane = CreatePlaneFromIds(ids);
        var optimizer = new VoxelMeshOptimizer.Core.OptimizationAlgorithms.DisjointSet.DisjointSetVisiblePlaneOptimizer(plane);
        optimizer.Optimize();

        var quads = optimizer.ToMeshQuads();
        Assert.Equal(2, quads.Count);
    }

    [Fact]
    public void ToMeshQuads_TwoSamePixels_CreatesOneQuad()
    {
        ushort?[,] ids = { { 1, 1 } };
        var plane = CreatePlaneFromIds(ids);
        var optimizer = new VoxelMeshOptimizer.Core.OptimizationAlgorithms.DisjointSet.DisjointSetVisiblePlaneOptimizer(plane);
        optimizer.Optimize();

        var quads = optimizer.ToMeshQuads();
        Assert.Single(quads);
        Assert.Equal(1, quads[0].VoxelID);
    }

    [Fact]
    public void ToMeshQuads_RectangleOfSameColor_CreatesSingleQuad()
    {
        ushort?[,] ids = {
        { 1, 1 },
        { 1, 1 }
    };
        var plane = CreatePlaneFromIds(ids);
        var optimizer = new VoxelMeshOptimizer.Core.OptimizationAlgorithms.DisjointSet.DisjointSetVisiblePlaneOptimizer(plane);
        optimizer.Optimize();

        var quads = optimizer.ToMeshQuads();
        Assert.Single(quads);
        Assert.Equal(4, GetQuadArea(quads[0]));
    }

    [Fact]
    public void ToMeshQuads_MixedColors_ProducesMultipleDistinctQuads()
    {
        ushort?[,] ids = {
        { 1, 1, 2 },
        { 1, 2, 2 },
        { 3, 3, 2 }
    };
        var plane = CreatePlaneFromIds(ids);
        var optimizer = new VoxelMeshOptimizer.Core.OptimizationAlgorithms.DisjointSet.DisjointSetVisiblePlaneOptimizer(plane);
        optimizer.Optimize();

        var quads = optimizer.ToMeshQuads();

        Assert.Contains(quads, q => q.VoxelID == 1);
        Assert.Contains(quads, q => q.VoxelID == 2);
        Assert.Contains(quads, q => q.VoxelID == 3);
        Assert.True(quads.Count >= 4 && quads.Count <= 6);
    }

    // Helper method to estimate quad area (assumes axis-aligned)
    private static int GetQuadArea(MeshQuad quad)
    {
        var width = Vector3.Distance(quad.Vertex0, quad.Vertex1);
        var height = Vector3.Distance(quad.Vertex1, quad.Vertex2);
        return (int)(width * height);
    }

    #endregion


    #region Constructor

    [Fact]
    public void Constructor_NullPixels_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => new VoxelMeshOptimizer.Core.OptimizationAlgorithms.DisjointSet.DisjointSetVisiblePlaneOptimizer(null));
    }

    [Fact]
    public void Constructor_EmptyPixels_ThrowsArgumentException()
    {
        ushort?[,] ids = new ushort?[0, 0];
        var plane = CreatePlaneFromIds(ids);
        Assert.Throws<ArgumentOutOfRangeException>(() => new VoxelMeshOptimizer.Core.OptimizationAlgorithms.DisjointSet.DisjointSetVisiblePlaneOptimizer(plane));
    }

    #endregion

    #region ToMeshQuads
    [Fact]
    public void ToMeshQuads_ShouldReturnOneQuad_WithCorrectVerticesAndNormal()
    {
        ushort?[,] ids = {
            {1, 1},
            {1, 1}
        };

        var plane = CreatePlaneFromIds(ids);
        var optimizer = new VoxelMeshOptimizer.Core.OptimizationAlgorithms.DisjointSet.DisjointSetVisiblePlaneOptimizer(plane);
        optimizer.Optimize();

        var quads = optimizer.ToMeshQuads();

        Assert.Single(quads);

        var quad = quads[0];

        // Check normal
        Assert.Equal(new Vector3(0, 0, 1), quad.Normal);

        // Vertices should form a square (1x1 at origin)
        Assert.Equal(new Vector3(0, 0, 0), quad.Vertex0);
        Assert.Equal(new Vector3(2, 0, 0), quad.Vertex1);
        Assert.Equal(new Vector3(2, 2, 0), quad.Vertex2);
        Assert.Equal(new Vector3(0, 2, 0), quad.Vertex3);

        Assert.Equal(1, quad.VoxelID);
    }

    [Fact]
    public void ToMeshQuads_ShouldCreateCorrectNormals_ForDescendingMinorAxis()
    {
        ushort?[,] ids = {
            {1, 1},
            {1, 1}
        };

        var plane = new VisiblePlane(
            Axis.X, AxisOrder.Ascending,
            Axis.Y, AxisOrder.Ascending,
            Axis.Z, AxisOrder.Descending,
            0, 2, 2
        );

        for (int x = 0; x < 2; x++)
        {
            for (int y = 0; y < 2; y++)
            {
                plane.Voxels[x, y] = new TestVoxel(id: 1, isSolid: true);
            }
        }

        var optimizer = new VoxelMeshOptimizer.Core.OptimizationAlgorithms.DisjointSet.DisjointSetVisiblePlaneOptimizer(plane);
        optimizer.Optimize();
        var quads = optimizer.ToMeshQuads();

        Assert.Single(quads);

        var quad = quads[0];
        Assert.Equal(new Vector3(0, 0, -1), quad.Normal);
    }

    [Fact]
    public void ToMeshQuads_ShouldCreateMultipleQuads_WhenVoxelGroupsAreSeparated()
    {
        ushort?[,] ids = {
            {1, null},
            {null, 2}
        };

        var plane = CreatePlaneFromIds(ids);
        var optimizer = new VoxelMeshOptimizer.Core.OptimizationAlgorithms.DisjointSet.DisjointSetVisiblePlaneOptimizer(plane);
        optimizer.Optimize();

        var quads = optimizer.ToMeshQuads();

        Assert.Equal(2, quads.Count);

        Assert.Contains(quads, q => q.VoxelID == 1);
        Assert.Contains(quads, q => q.VoxelID == 2);
    }
    #endregion
}
