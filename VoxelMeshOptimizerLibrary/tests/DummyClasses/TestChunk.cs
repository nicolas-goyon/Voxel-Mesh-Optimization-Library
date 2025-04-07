using VoxelMeshOptimizer.Core;

public class TestChunk : Chunk<Voxel>
{
    private readonly Voxel[,,] data;

    public uint Width { get; }
    public uint Height { get; }
    public uint Depth { get; }

    public TestChunk(uint width, uint height, uint depth)
    {
        Width = width;
        Height = height;
        Depth = depth;
        data = new Voxel[width, height, depth];
    }

    public IEnumerable<Voxel> GetVoxels()
    {
        for (uint x = 0; x < Width; x++)
        {
            for (uint y = 0; y < Height; y++)
            {
                for (uint z = 0; z < Depth; z++)
                {
                    var v = data[x, y, z];
                    if (v != null) yield return v;
                }
            }
        }
    }

    public Voxel Get(uint x, uint y, uint z)
    {
        if (x >= Width || y >= Height || z >= Depth)
            return null; // or throw, depending on your design
        return data[x, y, z];
    }

    public void SetVoxel(uint x, uint y, uint z, Voxel voxel)
    {
        if (x < Width && y < Height && z < Depth)
        {
            data[x, y, z] = voxel;
        }
    }
}
