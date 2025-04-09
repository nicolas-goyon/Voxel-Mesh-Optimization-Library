using VoxelMeshOptimizer.Core;
using VoxelMeshOptimizer.Tests.DummyClasses;
using Xunit;

namespace VoxelMeshOptimizer.Tests.Core;
public class ChunkInterfaceTests
{

    
    [Fact]
    public void ChunkDimensionsAndPlaneDimensions(){
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

        Assert.Equal<uint>(2, chunk.GetDepth(Axis.X));
        Assert.Equal<uint>(2, chunk.GetDepth(Axis.Y));
        Assert.Equal<uint>(1, chunk.GetDepth(Axis.Z));

        Assert.Equal<(uint, uint)>((2, 1), chunk.GetPlaneDimensions(Axis.X, Axis.Y, Axis.Z));
        Assert.Equal<(uint, uint)>((2, 1), chunk.GetPlaneDimensions(Axis.X, Axis.Y, Axis.Z));
        Assert.Equal<(uint, uint)>((2, 1), chunk.GetPlaneDimensions(Axis.X, Axis.Y, Axis.Z));

    }

    [Fact]
    public void AreDifferentAxis(){
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

        Assert.True(chunk.AreDifferentAxis(Axis.X,Axis.Y,Axis.Z));
        Assert.False(chunk.AreDifferentAxis(Axis.X,Axis.X,Axis.Z));
        Assert.False(chunk.AreDifferentAxis(Axis.X,Axis.Y,Axis.X));
        Assert.False(chunk.AreDifferentAxis(Axis.Y,Axis.Y,Axis.Z));
        Assert.False(chunk.AreDifferentAxis(Axis.X,Axis.Y,Axis.Y));
        Assert.False(chunk.AreDifferentAxis(Axis.Z,Axis.Y,Axis.Z));
        Assert.False(chunk.AreDifferentAxis(Axis.X,Axis.Z,Axis.Z));
        Assert.False(chunk.AreDifferentAxis(Axis.X,Axis.X,Axis.X));
    }

    [Fact]
    public void GetDimension_ForEachCoordinate_CorrectOrder(){
        // Arrange
        var chunk = new TestChunk(2, 2, 2);

        var expectedOrder = new (uint, uint, uint)[]{
            (0,0,0),
            (0,0,1),
            (0,1,0),
            (0,1,1),
            (1,0,0),
            (1,0,1),
            (1,1,0),
            (1,1,1),
        };
        int index = 0;

        // Only one voxel => set it to solid
        chunk.ForEachCoordinate(
            Axis.X, AxisOrder.Ascending,
            Axis.Y, AxisOrder.Ascending,
            Axis.Z, AxisOrder.Ascending,
            (x,y,z) => 
            {
                Assert.Equal<(uint, uint, uint)>(expectedOrder[index], (x,y,z));
                index++;
            }
        );

        
        expectedOrder = new (uint, uint, uint)[]{
            (1,0,0),
            (1,0,1),
            (1,1,0),
            (1,1,1),
            (0,0,0),
            (0,0,1),
            (0,1,0),
            (0,1,1),
        };
        index = 0;

        // Only one voxel => set it to solid
        chunk.ForEachCoordinate(
            Axis.X, AxisOrder.Descending,
            Axis.Y, AxisOrder.Ascending,
            Axis.Z, AxisOrder.Ascending,
            (x,y,z) => 
            {
                Assert.Equal<(uint, uint, uint)>(expectedOrder[index], (x,y,z));
                index++;
            }
        );

        
        expectedOrder = new (uint, uint, uint)[]{
            (0,1,0),
            (0,1,1),
            (0,0,0),
            (0,0,1),
            (1,1,0),
            (1,1,1),
            (1,0,0),
            (1,0,1),
        };
        index = 0;

        // Only one voxel => set it to solid
        chunk.ForEachCoordinate(
            Axis.X, AxisOrder.Ascending,
            Axis.Y, AxisOrder.Descending,
            Axis.Z, AxisOrder.Ascending,
            (x,y,z) => 
            {
                Assert.Equal<(uint, uint, uint)>(expectedOrder[index], (x,y,z));
                index++;
            }
        );

        
        expectedOrder = new (uint, uint, uint)[]{
            (0,0,1),
            (0,0,0),
            (0,1,1),
            (0,1,0),
            (1,0,1),
            (1,0,0),
            (1,1,1),
            (1,1,0),
        };
        index = 0;

        // Only one voxel => set it to solid
        chunk.ForEachCoordinate(
            Axis.X, AxisOrder.Ascending,
            Axis.Y, AxisOrder.Ascending,
            Axis.Z, AxisOrder.Descending,
            (x,y,z) => 
            {
                Assert.Equal<(uint, uint, uint)>(expectedOrder[index], (x,y,z));
                index++;
            }
        );


        
        
        expectedOrder = new (uint, uint, uint)[]{
            (1,1,1),
            (1,1,0),
            (1,0,1),
            (1,0,0),
            (0,1,1),
            (0,1,0),
            (0,0,1),
            (0,0,0),
        };
        index = 0;

        // Only one voxel => set it to solid
        chunk.ForEachCoordinate(
            Axis.X, AxisOrder.Descending,
            Axis.Y, AxisOrder.Descending,
            Axis.Z, AxisOrder.Descending,
            (x,y,z) => 
            {
                Assert.Equal<(uint, uint, uint)>(expectedOrder[index], (x,y,z));
                index++;
            }
        );


        
        
        expectedOrder = new (uint, uint, uint)[]{
            (0,0,0),
            (1,0,0),
            (0,0,1),
            (1,0,1),
            (0,1,0),
            (1,1,0),
            (0,1,1),
            (1,1,1),
        };
        index = 0;

        // Only one voxel => set it to solid
        chunk.ForEachCoordinate(
            Axis.Y, AxisOrder.Ascending,
            Axis.Z, AxisOrder.Ascending,
            Axis.X, AxisOrder.Ascending,
            (x,y,z) => 
            {
                Assert.Equal<(uint, uint, uint)>(expectedOrder[index], (x,y,z));
                index++;
            }
        );
        
        expectedOrder = new (uint, uint, uint)[]{
            (1,1,1),
            (0,1,1),
            (1,1,0),
            (0,1,0),
            (1,0,1),
            (0,0,1),
            (1,0,0),
            (0,0,0),
        };
        index = 0;

        // Only one voxel => set it to solid
        chunk.ForEachCoordinate(
            Axis.Y, AxisOrder.Descending,
            Axis.Z, AxisOrder.Descending,
            Axis.X, AxisOrder.Descending,
            (x,y,z) => 
            {
                Assert.Equal<(uint, uint, uint)>(expectedOrder[index], (x,y,z));
                index++;
            }
        );



        
        expectedOrder = new (uint, uint, uint)[]{
            (0,0,0),
            (0,1,0),

            (1,0,0),
            (1,1,0),

            (0,0,1),
            (0,1,1),

            (1,0,1),
            (1,1,1),
        };
        index = 0;

        // Only one voxel => set it to solid
        chunk.ForEachCoordinate(
            Axis.Z, AxisOrder.Ascending,
            Axis.X, AxisOrder.Ascending,
            Axis.Y, AxisOrder.Ascending,
            (x,y,z) => 
            {
                Assert.Equal<(uint, uint, uint)>(expectedOrder[index], (x,y,z));
                index++;
            }
        );
    }
    
    
    [Fact]
    public void GetDimension_ForEachCoordinate_Throws_WhenAxisEquals(){
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

        // Only one voxel => set it to solid
        Assert.Throws<ArgumentException>(() => chunk.ForEachCoordinate(
            Axis.X, AxisOrder.Ascending,
            Axis.X, AxisOrder.Ascending,
            Axis.Z, AxisOrder.Ascending,
            (x,y,z) => {}
        ));
        Assert.Throws<ArgumentException>(() => chunk.ForEachCoordinate(
            Axis.X, AxisOrder.Ascending,
            Axis.Y, AxisOrder.Ascending,
            Axis.Y, AxisOrder.Ascending,
            (x,y,z) => {}
        ));
        Assert.Throws<ArgumentException>(() => chunk.ForEachCoordinate(
            Axis.Z, AxisOrder.Ascending,
            Axis.X, AxisOrder.Ascending,
            Axis.Z, AxisOrder.Ascending,
            (x,y,z) => {}
        ));
        Assert.Throws<ArgumentException>(() => chunk.ForEachCoordinate(
            Axis.Z, AxisOrder.Ascending,
            Axis.Z, AxisOrder.Ascending,
            Axis.Z, AxisOrder.Ascending,
            (x,y,z) => {}
        ));
    }


    [Fact]
    public void SetGet_ThrowsError_WhenOutOfBound(){
        var chunk = new TestChunk(2, 2, 1);
        Assert.Throws<ArgumentOutOfRangeException>(()=>chunk.Set(2,0,0, new TestVoxel(id: 1, isSolid: true)));
        Assert.Throws<ArgumentOutOfRangeException>(()=>chunk.Set(0,2,0, new TestVoxel(id: 1, isSolid: true)));
        Assert.Throws<ArgumentOutOfRangeException>(()=>chunk.Set(0,0,2, new TestVoxel(id: 1, isSolid: true)));

        
        Assert.Throws<ArgumentOutOfRangeException>(()=>chunk.Get(2,0,0));
        Assert.Throws<ArgumentOutOfRangeException>(()=>chunk.Get(0,2,0));
        Assert.Throws<ArgumentOutOfRangeException>(()=>chunk.Get(0,0,2));
    }
}