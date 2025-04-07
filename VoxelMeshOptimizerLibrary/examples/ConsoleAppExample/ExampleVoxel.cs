using VoxelMeshOptimizer.Core;

namespace ConsoleAppExample;

public class ExampleVoxel : VoxelMeshOptimizer
{
    public ushort ID { get; }
    public bool IsSolid => ID != 0;

    public ExampleVoxel(ushort id)
    {
        ID = id;
    }
}
