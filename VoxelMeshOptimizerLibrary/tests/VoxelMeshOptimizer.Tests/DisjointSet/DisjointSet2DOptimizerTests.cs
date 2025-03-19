using Xunit;
using VoxelMeshOptimizer.Core.OptimizationAlgorithms.DisjointSet;

namespace VoxelMeshOptimizer.Tests;

public class DisjointSet2DOptimizerTests
{
    [Fact]
    public void Optimize_ShouldMergeIntoOneRectangle_WhenAllPixelsHaveSameValue()
    {
        int[,] pixels = {
            {1, 1},
            {1, 1}
        };

        var optimizer = new DisjointSet2DOptimizer(pixels);
        optimizer.Optimize();

        var expected = new List<List<(int x, int y)>>
        {
            new List<(int x, int y)> { (0,0), (0,1), (1,0), (1,1) }
        };

        Assert.Equal(expected, optimizer.ToResult());
    }

    [Fact]
    public void Optimize_ShouldNotMerge_WhenPixelsHaveDifferentValues()
    {
        int[,] pixels = {
            {1, 2},
            {3, 4}
        };

        var optimizer = new DisjointSet2DOptimizer(pixels);
        optimizer.Optimize();

        var expected = new List<List<(int x, int y)>>
        {
            new List<(int x, int y)> { (0,0) },
            new List<(int x, int y)> { (0,1) },
            new List<(int x, int y)> { (1,0) },
            new List<(int x, int y)> { (1,1) }
        };

        Assert.Equal(expected, optimizer.ToResult());
    }

    [Fact]
    public void Optimize_ShouldCreateMultipleRectangles_ForMultipleGroups()
    {
        int[,] pixels = {
            {1, 1, 2, 2},
            {1, 1, 2, 2},
            {3, 3, 4, 4},
            {3, 3, 4, 4}
        };

        var optimizer = new DisjointSet2DOptimizer(pixels);
        optimizer.Optimize();

        var expected = new List<List<(int x, int y)>>
        {
            new List<(int x, int y)> { (0,0), (0,1), (1,0), (1,1) },
            new List<(int x, int y)> { (0,2), (0,3), (1,2), (1,3) },
            new List<(int x, int y)> { (2,0), (2,1), (3,0), (3,1) },
            new List<(int x, int y)> { (2,2), (2,3), (3,2), (3,3) }
        };

        Assert.Equal(expected, optimizer.ToResult());
    }

    [Fact]
    public void Optimize_ShouldHandleSinglePixelGroups()
    {
        int[,] pixels = {
            {1, 2},
            {1, 3}
        };

        var optimizer = new DisjointSet2DOptimizer(pixels);
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
        int[,] pixels = {
            {1, 1, 2},
            {1, 2, 2},
            {2, 2, 2}
        };

        var optimizer = new DisjointSet2DOptimizer(pixels);
        optimizer.Optimize();

        var expected = new List<List<(int x, int y)>>
        {
            new List<(int x, int y)> { (0,0), (0,1)},
            new List<(int x, int y)> { (0,2), (1,2), (2,2)},
            new List<(int x, int y)> { (1,0)},
            new List<(int x, int y)> { (1,1), (2,1)},
            new List<(int x, int y)> { (2,0)},

        };

        Assert.Equal(expected, optimizer.ToResult());
    }



    [Fact]
    public void Constructor_NullPixels_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => new DisjointSet2DOptimizer(null));
    }

    [Fact]
    public void Constructor_EmptyPixels_ThrowsArgumentException()
    {
        int[,] emptyPixels = new int[0, 0];
        Assert.Throws<ArgumentOutOfRangeException>(() => new DisjointSet2DOptimizer(emptyPixels));
    }

    [Fact]
    public void Optimize_SinglePixel_NoUnionPerformed()
    {
        int[,] pixels = { { 1 } };
        var optimizer = new DisjointSet2DOptimizer(pixels);
        optimizer.Optimize();

        var result = optimizer.ToResult();
        Assert.Single(result);
        Assert.Single(result[0]);
        Assert.Equal((0, 0), result[0][0]);
    }

    [Fact]
    public void Optimize_TwoDifferentPixels_TwoSetsCreated()
    {
        int[,] pixels = { { 1, 2 } };
        var optimizer = new DisjointSet2DOptimizer(pixels);
        optimizer.Optimize();

        var result = optimizer.ToResult();
        Assert.Equal(2, result.Count);
    }

    [Fact]
    public void Optimize_TwoSamePixels_OneSetCreated()
    {
        int[,] pixels = { { 1, 1 } };
        var optimizer = new DisjointSet2DOptimizer(pixels);
        optimizer.Optimize();

        var result = optimizer.ToResult();
        Assert.Single(result);
        Assert.Equal(2, result[0].Count);
    }

    [Fact]
    public void Optimize_RectangleOfSameColor_CreatesSingleSet()
    {
        int[,] pixels = {
            { 1, 1 },
            { 1, 1 }
        };
        var optimizer = new DisjointSet2DOptimizer(pixels);
        optimizer.Optimize();

        var result = optimizer.ToResult();
        Assert.Single(result);
        Assert.Equal(4, result[0].Count);
    }

    [Fact]
    public void Optimize_MixedColors_CreatesCorrectSets()
    {
        int[,] pixels = {
            { 1, 1, 2 },
            { 1, 2, 2 },
            { 3, 3, 2 }
        };
        var optimizer = new DisjointSet2DOptimizer(pixels);
        optimizer.Optimize();

        var result = optimizer.ToResult();
        Assert.Equal(5, result.Count);
    }
    
}
