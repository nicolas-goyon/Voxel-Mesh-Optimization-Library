namespace VoxelMeshOptimizer.Core;
using System;


public interface Voxel
{
    public ushort ID { get;}
    public bool IsSolid {get;}
}


[Flags]
public enum VoxelFace
{
    None = 0,
    Front = 1 << 0,  // Positive Z
    Back = 1 << 1,   // Negative Z
    Left = 1 << 2,   // Negative X
    Right = 1 << 3,  // Positive X
    Top = 1 << 4,    // Positive Y
    Bottom = 1 << 5  // Negative Y
}