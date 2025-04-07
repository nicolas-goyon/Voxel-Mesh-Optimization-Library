using VoxelMeshOptimizer.Core.OptimizationAlgorithms.DisjointSet;
using Xunit;

namespace VoxelMeshOptimizer.Tests.DisjointSetTesting;

public class DisjointSetTests
{
    [Fact]
    public void InitialCount_ShouldBeEqualToNumberOfElements()
    {
        int n = 10;
        var ds = new DisjointSet(n);
        Assert.Equal(n, ds.GetCount());
    }

    [Fact]
    public void Union_ShouldReduceSetCount()
    {
        var ds = new DisjointSet(5);
        ds.Union(0, 1);
        Assert.Equal(4, ds.GetCount());

        ds.Union(1, 2);
        Assert.Equal(3, ds.GetCount());
    }

    [Fact]
    public void Find_ShouldReturnSameRoot_ForConnectedElements()
    {
        var ds = new DisjointSet(4);
        ds.Union(0, 1);
        ds.Union(1, 2);

        Assert.Equal(ds.Find(0), ds.Find(2));
        Assert.Equal(ds.Find(1), ds.Find(2));
    }

    [Fact]
    public void Find_ShouldReturnDifferentRoots_ForDisconnectedElements()
    {
        var ds = new DisjointSet(4);
        ds.Union(0, 1);

        Assert.NotEqual(ds.Find(0), ds.Find(2));
        Assert.NotEqual(ds.Find(1), ds.Find(3));
    }

    [Fact]
    public void IsRoot_ShouldBeTrue_ForInitialSingleElements()
    {
        var ds = new DisjointSet(3);

        Assert.True(ds.IsRoot(0));
        Assert.True(ds.IsRoot(1));
        Assert.True(ds.IsRoot(2));
    }

    [Fact]
    public void Union_WithSameSet_ShouldNotChangeCount()
    {
        var ds = new DisjointSet(3);
        ds.Union(0, 1);
        var initialCount = ds.GetCount();
        ds.Union(0, 1);

        Assert.Equal(initialCount, ds.GetCount());
    }

    [Fact]
    public void Constructor_ShouldThrowException_ForNegativeElements()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new DisjointSet(-1));
    }

    [Fact]
    public void Find_ShouldThrowException_ForInvalidIndex()
    {
        var ds = new DisjointSet(3);

        Assert.Throws<IndexOutOfRangeException>(() => ds.Find(-1));
        Assert.Throws<IndexOutOfRangeException>(() => ds.Find(3));
    }

    [Fact]
    public void Union_ShouldThrowException_ForInvalidIndices()
    {
        var ds = new DisjointSet(3);

        Assert.Throws<IndexOutOfRangeException>(() => ds.Union(-1, 0));
        Assert.Throws<IndexOutOfRangeException>(() => ds.Union(0, 3));
    }



    [Fact]
    public void ZeroElements_ShouldHaveZeroCount()
    {
        var ds = new DisjointSet(0);
        Assert.Equal(0, ds.GetCount());
    }

    [Fact]
    public void Operations_ShouldThrowException_OnZeroSizedSet()
    {
        var ds = new DisjointSet(0);

        Assert.Throws<IndexOutOfRangeException>(() => ds.Union(0, 1));
        Assert.Throws<IndexOutOfRangeException>(() => ds.Find(0));
        Assert.Throws<IndexOutOfRangeException>(() => ds.IsRoot(0));
    }

    [Fact]
    public void MultipleUnions_ShouldCreateSingleSet()
    {
        var ds = new DisjointSet(5);
        ds.Union(0, 1);
        ds.Union(2, 3);
        ds.Union(1, 2);
        ds.Union(3, 4);

        Assert.Equal(1, ds.GetCount());
        Assert.Equal(ds.Find(0), ds.Find(4));
    }

    [Fact]
    public void IsRoot_ShouldBeFalse_ForNonRootElements()
    {
        var ds = new DisjointSet(3);
        ds.Union(0, 1);

        Assert.False(ds.IsRoot(1) && ds.IsRoot(0));
    }
}
