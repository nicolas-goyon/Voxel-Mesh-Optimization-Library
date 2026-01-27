using CommunityToolkit.Diagnostics;
using VoxelMeshOptimizer.Core;

namespace VoxelMeshOptimizer.Tests.Helpers;

public static class ChunkGen
{
    public static Chunk GenerateChunk(int sizeX, int sizeY, int sizeZ)
    {
        Guard.IsGreaterThan(sizeX, 0, "sizeX");
        Guard.IsGreaterThan(sizeY, 0, "sizeY" );
        Guard.IsGreaterThan(sizeZ, 0, "sizeZ");

        ushort[,,] voxels = new ushort[sizeX, sizeY, sizeZ];
        return new Chunk(voxels);
    }
    
    
    public static Chunk GenerateChunk(uint sizeX, uint sizeY, uint sizeZ)
    {
        Guard.IsGreaterThan(sizeX, 0, "sizeX");
        Guard.IsGreaterThan(sizeY, 0, "sizeY" );
        Guard.IsGreaterThan(sizeZ, 0, "sizeZ");

        ushort[,,] voxels = new ushort[sizeX, sizeY, sizeZ];
        return new Chunk(voxels);
    }
}