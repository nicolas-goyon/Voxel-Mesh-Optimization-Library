using VoxelMeshOptimizer.Core;

namespace ConsoleAppExample;

public class ExampleChunk : Chunk
{
    public IEnumerable<Voxel> GetVoxels()
    {
        // Example data here
        return new List<Voxel>
        {
            new ExampleVoxel(1, (0,0,0)),
            new ExampleVoxel(1, (1,0,0)),
            // Add more example voxels as necessary
        };
    }
}
