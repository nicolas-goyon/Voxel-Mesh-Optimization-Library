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
        Chunk chunk = new DummyChunk();

        // Act & Assert
        Assert.Throws<NotImplementedException>(() => optimizer.Optimize(chunk));
    }

    private class DummyChunk : Chunk
    {
        public IEnumerable<Voxel> GetVoxels() => new List<Voxel>();
    }
}
