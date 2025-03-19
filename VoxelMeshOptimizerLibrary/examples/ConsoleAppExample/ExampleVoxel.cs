using VoxelMeshOptimizer.Core;

namespace ConsoleAppExample;

public class ExampleVoxel : Voxel
{
    public int Color { get; }
    public (int x, int y, int z) Position { get; }

    public ExampleVoxel(int color, (int, int, int) position)
    {
        Color = color;
        Position = position;
    }
}
