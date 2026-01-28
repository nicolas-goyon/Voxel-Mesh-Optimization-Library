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


    /**
     * Generate a full chunk with id 1 voxels 
     * 
     */
    public static Chunk GenerateBasicChunk_full(int x = 16, int y = 16, int z = 16)
    {
        Guard.IsGreaterThan(x, 0, "sizeX");
        Guard.IsGreaterThan(y, 0, "sizeY" );
        Guard.IsGreaterThan(z, 0, "sizeZ");
        
        ushort[,,] voxels = new ushort[x, y, z];
        for (int i = 0; i < x; i++)
        {
            for (int j = 0; j < y; j++)
            {
                for (int k = 0; k < z; k++)
                {
                    voxels[i, j, k] = 1;
                }
            }
        }
        return new Chunk(voxels);
    }
    
    
    /**
     * Generate a partially full chunk with id 1 voxels (only voxels with y <= limit)
     *
     */
    public static Chunk GenerateBasicChunk_partiallyFull(int x = 16, int y = 16, int z = 16, int limit = 8)
    {
        Guard.IsGreaterThan(x, 0, "sizeX");
        Guard.IsGreaterThan(y, 0, "sizeY" );
        Guard.IsGreaterThan(z, 0, "sizeZ");
        
        ushort[,,] voxels = new ushort[x, y, z];
        for (int i = 0; i < x; i++)
        {
            for (int j = 0; j < y; j++)
            {
                for (int k = 0; k < z; k++)
                {
                    voxels[i, j, k] = j > limit ? (ushort)0 : (ushort)1;
                }
            }
        }
        return new Chunk(voxels);
    }



    public static ushort[,,] GenerateBasicVoxelArray_PartiallyFull(int x = 16, int y = 16, int z = 16, int limit = 8)
    {
        
        Guard.IsGreaterThan(x, 0, "sizeX");
        Guard.IsGreaterThan(y, 0, "sizeY" );
        Guard.IsGreaterThan(z, 0, "sizeZ");
        
        ushort[,,] voxels = new ushort[x, y, z];
        for (int i = 0; i < x; i++)
        {
            for (int j = 0; j < y; j++)
            {
                for (int k = 0; k < z; k++)
                {
                    voxels[i, j, k] = j > limit ? (ushort)0 : (ushort)1;
                }
            }
        }

        return voxels;
    }
}