using VoxelMeshOptimizer.Core;

namespace ConsoleAppExample;

public class ExampleVoxel : Voxel
{
    public ushort ID { get; }
    public bool IsTransparent => ID == 0;

    public ExampleVoxel(ushort id)
    {
        ID = id;
    }
}
