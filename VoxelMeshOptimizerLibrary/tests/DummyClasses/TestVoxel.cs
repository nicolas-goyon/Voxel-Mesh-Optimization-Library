using VoxelMeshOptimizer.Core;


namespace VoxelMeshOptimizer.Tests.DummyClasses;


public class TestVoxel : Voxel
{
    public ushort ID { get; }
    public bool IsSolid { get; }

    public TestVoxel(ushort id, bool isSolid)
    {
        ID = id;
        IsSolid = isSolid;
    }
}
