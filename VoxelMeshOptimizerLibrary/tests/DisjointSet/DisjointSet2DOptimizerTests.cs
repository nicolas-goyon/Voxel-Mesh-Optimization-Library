using Xunit;
using VoxelMeshOptimizer.Core.OptimizationAlgorithms.DisjointSet;
using VoxelMeshOptimizer.Core.OcclusionAlgorithms.Common;
using VoxelMeshOptimizer.Core;
using VoxelMeshOptimizer.Tests.DummyClasses;
using System.Numerics;

namespace VoxelMeshOptimizer.Tests.DisjointSetTesting;

public class DisjointSet2DOptimizerTests
{

    private static VisiblePlane CreatePlaneFromIds(ushort?[,] ids)
    {
        int width = ids.GetLength(0);
        int height = ids.GetLength(1);
        var plane = new VisiblePlane(
            Axis.X, AxisOrder.Ascending,
            Axis.Y, AxisOrder.Ascending,
            Axis.Z, AxisOrder.Ascending,
            0, (uint)width, (uint)height
        );

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                var id = ids[x, y];
                if (id != null && id.HasValue)
                {
                    plane.Voxels[x, y] = new TestVoxel (id: id.Value,isSolid: true);
                }
            }
        }

        return plane;
    }


    [Fact]
    public void Optimize_ShouldMergeIntoOneRectangle_WhenAllPixelsHaveSameValue()
    {
        ushort?[,] ids = {
            {1, 1},
            {1, 1}
        };

        var plane = CreatePlaneFromIds(ids);
        var optimizer = new DisjointSetVisiblePlaneOptimizer(plane);
        optimizer.Optimize();

        var expected = new List<List<(int x, int y)>>
        {
            new List<(int x, int y)> { (0,0), (1,0), (0,1), (1,1) }
        };

        Assert.Equal(expected, optimizer.ToResult());
    }

    [Fact]
    public void Optimize_ShouldNotMerge_WhenPixelsHaveDifferentValues()
    {
        ushort?[,] ids = {
            {1, 2},
            {3, 4}
        };

        var plane = CreatePlaneFromIds(ids);
        var optimizer = new DisjointSetVisiblePlaneOptimizer(plane);
        optimizer.Optimize();

        var expected = new List<List<(int x, int y)>>
        {
            new List<(int x, int y)> { (0,0) },
            new List<(int x, int y)> { (1,0) },
            new List<(int x, int y)> { (0,1) },
            new List<(int x, int y)> { (1,1) }
        };

        Assert.Equal(expected, optimizer.ToResult());
    }

    [Fact]
    public void Optimize_ShouldCreateMultipleRectangles_ForMultipleGroups()
    {
        ushort?[,] ids = {
            {1, 1, 2, 2},
            {1, 1, 2, 2},
            {3, 3, 4, 4},
            {3, 3, 4, 4}
        };

        var plane = CreatePlaneFromIds(ids);
        var optimizer = new DisjointSetVisiblePlaneOptimizer(plane);
        optimizer.Optimize();

        var expected = new List<List<(int x, int y)>>
        {
            new List<(int x, int y)> { (0,0), (1,0), (0,1), (1,1) },
            new List<(int x, int y)> { (2,0), (3,0), (2,1), (3,1) },
            new List<(int x, int y)> { (0,2), (1,2), (0,3), (1,3) },
            new List<(int x, int y)> { (2,2), (3,2), (2,3), (3,3) }
        };

        Assert.Equal(expected, optimizer.ToResult());
    }

    [Fact]
    public void Optimize_ShouldHandleSinglePixelGroups()
    {
        ushort?[,] ids = {
            {1, 2},
            {1, 3}
        };

        var plane = CreatePlaneFromIds(ids);
        var optimizer = new DisjointSetVisiblePlaneOptimizer(plane);
        optimizer.Optimize();

        var expected = new List<List<(int x, int y)>>
        {
            new List<(int x, int y)> { (0,0), (1,0) },
            new List<(int x, int y)> { (0,1) },
            new List<(int x, int y)> { (1,1) }
        };

        Assert.Equal(expected, optimizer.ToResult());
    }

    [Fact]
    public void Optimize_ShouldNotMergeIntoNonRectangleShapes()
    {
        ushort?[,] ids = {
            {1, 1, 2},
            {1, 2, 2},
            {2, 2, 2}
        };

        var plane = CreatePlaneFromIds(ids);
        var optimizer = new DisjointSetVisiblePlaneOptimizer(plane);
        optimizer.Optimize();

        var expected = new List<List<(int x, int y)>>
        {
            new List<(int x, int y)> { (0,0), (1,0)},
            new List<(int x, int y)> { (2,0), (2,1), (2,2)},
            new List<(int x, int y)> { (0,1)},
            new List<(int x, int y)> { (1,1), (1,2)},
            new List<(int x, int y)> { (0,2)},

        };

        Assert.Equal(expected, optimizer.ToResult());
    }



    [Fact]
    public void Constructor_NullPixels_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => new DisjointSetVisiblePlaneOptimizer(null));
    }

    [Fact]
    public void Constructor_EmptyPixels_ThrowsArgumentException()
    {
        ushort?[,] ids = new ushort?[0, 0];
        var plane = CreatePlaneFromIds(ids);
        Assert.Throws<ArgumentOutOfRangeException>(() => new DisjointSetVisiblePlaneOptimizer(plane));
    }

    [Fact]
    public void Optimize_SinglePixel_NoUnionPerformed()
    {
        ushort?[,] ids = { { 1 } };
        var plane = CreatePlaneFromIds(ids);
        var optimizer = new DisjointSetVisiblePlaneOptimizer(plane);
        optimizer.Optimize();

        var result = optimizer.ToResult();
        Assert.Single(result);
        Assert.Single(result[0]);
        Assert.Equal((0, 0), result[0][0]);
    }

    [Fact]
    public void Optimize_TwoDifferentPixels_TwoSetsCreated()
    {
        ushort?[,] ids = { { 1, 2 } };
        var plane = CreatePlaneFromIds(ids);
        var optimizer = new DisjointSetVisiblePlaneOptimizer(plane);
        optimizer.Optimize();

        var result = optimizer.ToResult();
        Assert.Equal(2, result.Count);
    }

    [Fact]
    public void Optimize_TwoSamePixels_OneSetCreated()
    {
        ushort?[,] ids = { { 1, 1 } };
        var plane = CreatePlaneFromIds(ids);
        var optimizer = new DisjointSetVisiblePlaneOptimizer(plane);
        optimizer.Optimize();

        var result = optimizer.ToResult();
        Assert.Single(result);
        Assert.Equal(2, result[0].Count);
    }

    [Fact]
    public void Optimize_RectangleOfSameColor_CreatesSingleSet()
    {
        ushort?[,] ids = {
            { 1, 1 },
            { 1, 1 }
        };
        var plane = CreatePlaneFromIds(ids);
        var optimizer = new DisjointSetVisiblePlaneOptimizer(plane);
        optimizer.Optimize();

        var result = optimizer.ToResult();
        Assert.Single(result);
        Assert.Equal(4, result[0].Count);
    }

    [Fact]
    public void Optimize_MixedColors_CreatesCorrectSets()
    {
        ushort?[,] ids = {
            { 1, 1, 2 },
            { 1, 2, 2 },
            { 3, 3, 2 }
        };
        var plane = CreatePlaneFromIds(ids);
        var optimizer = new DisjointSetVisiblePlaneOptimizer(plane);
        optimizer.Optimize();

        var result = optimizer.ToResult();
        
        var expected = new List<List<(int x, int y)>>
        {
            new List<(int x, int y)> { (0,0), (1,0)},
            new List<(int x, int y)> { (2,0), (2,1)},
            new List<(int x, int y)> { (0,1)},
            new List<(int x, int y)> { (1,1), (1,2)},
            new List<(int x, int y)> { (0,2)},
            new List<(int x, int y)> { (2,2)},

        };
        Assert.Equal(expected, result);
    }
    [Fact]
    public void ToMeshQuads_ShouldReturnOneQuad_WithCorrectVerticesAndNormal()
    {
        ushort?[,] ids = {
            {1, 1},
            {1, 1}
        };

        var plane = CreatePlaneFromIds(ids);
        var optimizer = new DisjointSetVisiblePlaneOptimizer(plane);
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

        var optimizer = new DisjointSetVisiblePlaneOptimizer(plane);
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
        var optimizer = new DisjointSetVisiblePlaneOptimizer(plane);
        optimizer.Optimize();

        var quads = optimizer.ToMeshQuads();

        Assert.Equal(2, quads.Count);

        Assert.Contains(quads, q => q.VoxelID == 1);
        Assert.Contains(quads, q => q.VoxelID == 2);
    }
}
