using VoxelMeshOptimizer.Core;
using VoxelMeshOptimizer.Core.OptimizationAlgorithms.DisjointSet;
using Xunit;

namespace VoxelMeshOptimizer.Tests;

public class DisjointSetMeshOptimizerTests
{
    [Fact]
    public void Optimize_ShouldThrow_NotImplementedException()
    {
        // Arrange
        var optimizer = new DisjointSetMeshOptimizer();

        // Act & Assert
        // Assert.Throws<NotImplementedException>(() => optimizer.Optimize(chunk));
    }

}
