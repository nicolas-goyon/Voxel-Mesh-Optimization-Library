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
    Zpos = 1 << 0,  // Front
    Zneg = 1 << 1,   // Back
    Xneg = 1 << 2,   // Right
    Xpos = 1 << 3,  // Left
    Ypos = 1 << 4,    // Bottom
    Yneg = 1 << 5  // Top
}