using VoxelMeshOptimizer.Core;

namespace ConsoleAppExample;
public class ExampleChunk : Chunk<ExampleVoxel>
{
    private readonly ExampleVoxel[,,] _voxels;

    public uint Width { get; }
    public uint Height { get; }
    public uint Depth { get; }

    public ExampleChunk(ushort[,,] voxelArray)
    {
        Width = (uint)voxelArray.GetLength(0);
        Height = (uint)voxelArray.GetLength(1);
        Depth = (uint)voxelArray.GetLength(2);

        _voxels = new ExampleVoxel[Width, Height, Depth];

        for (uint x = 0; x < Width; x++)
        {
            for (uint y = 0; y < Height; y++)
            {
                for (uint z = 0; z < Depth; z++)
                {
                    ushort value = voxelArray[x, y, z];
                    _voxels[x, y, z] = new ExampleVoxel(value);
                }
            }
        }
    }

    public IEnumerable<ExampleVoxel> GetVoxels()
    {
        for (uint x = 0; x < Width; x++)
        {
            for (uint y = 0; y < Height; y++)
            {
                for (uint z = 0; z < Depth; z++)
                {
                    yield return _voxels[x, y, z];
                }
            }
        }
    }

    public ExampleVoxel Get(uint x, uint y, uint z)
    {
        if (x >= Width || y >= Height || z >= Depth)
        {
            throw new ArgumentOutOfRangeException("Requested voxel coordinates are out of bounds.");
        }

        return _voxels[x, y, z];
    }

}
