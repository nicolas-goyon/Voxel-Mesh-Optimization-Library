using VoxelMeshOptimizer.Core;
using VoxelMeshOptimizer.Core.OptimizationAlgorithms.DisjointSet;
using VoxelMeshOptimizer.Tests.DummyClasses;
using Xunit;

namespace VoxelMeshOptimizer.Tests.DisjointSetTesting;

public class DisjointSetMeshOptimizerTests
{
    [Fact]
    public void Optimize_ShouldThrow_NotImplementedException()
    {
        // Arrange
        var optimizer = new DisjointSetMeshOptimizer();
        var chunk = new TestChunk(2, 2, 2);

        // Act & Assert
        Assert.Throws<NotImplementedException>(() => optimizer.Optimize(chunk));
    }

}
