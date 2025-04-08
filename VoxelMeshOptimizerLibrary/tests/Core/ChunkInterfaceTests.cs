using VoxelMeshOptimizer.Core;
using VoxelMeshOptimizer.Tests.DummyClasses;
using Xunit;

namespace VoxelMeshOptimizer.Tests.Core;
public class ChunkInterfaceTests
{

    
    [Fact]
    public void OcclusionNotImplemented(){
        // Arrange
        var chunk = new TestChunk(2, 2, 1);

        // Only one voxel => set it to solid
        chunk.ForEachCoordinate(
            Axis.X, AxisOrder.Ascending,
            Axis.Y, AxisOrder.Ascending,
            Axis.Z, AxisOrder.Ascending,
            (x,y,z) => 
            {
                // pos.X, pos.Y, pos.Z are all zero
                chunk.Set(x,y,z, new TestVoxel(id: 1, isSolid: true));
            }
        );

        Assert.Equal<uint>(2, chunk.GetDimension(Axis.X));
        Assert.Equal<uint>(2, chunk.GetDimension(Axis.Y));
        Assert.Equal<uint>(1, chunk.GetDimension(Axis.Z));

        Assert.Equal<(uint, uint)>((2, 1), chunk.GetPlaneDimensions(Axis.X, Axis.Y, Axis.Z));
        Assert.Equal<(uint, uint)>((2, 1), chunk.GetPlaneDimensions(Axis.X, Axis.Y, Axis.Z));
        Assert.Equal<(uint, uint)>((2, 1), chunk.GetPlaneDimensions(Axis.X, Axis.Y, Axis.Z));

    }
}