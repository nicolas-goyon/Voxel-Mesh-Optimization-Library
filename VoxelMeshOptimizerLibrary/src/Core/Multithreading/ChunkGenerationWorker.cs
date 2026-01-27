
using VoxelMeshOptimizer.Core.OptimizationAlgorithms.DisjointSet;

namespace VoxelMeshOptimizer.Core.Multithreading
{
    public readonly struct ChunkMeshGenerationWorker
    {
        private readonly Chunk chunk;

        public ChunkMeshGenerationWorker(Chunk chunk)
        {
            this.chunk = chunk;
        }

        public (Chunk chunk, Mesh) Execute()
        {
            DisjointSetMeshOptimizer optimizer = new(new Mesh([]));
            return (chunk, optimizer.Optimize(chunk));
        }
    }
}