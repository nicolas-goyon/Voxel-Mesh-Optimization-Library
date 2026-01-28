using VoxelMeshOptimizer.Core;
using VoxelMeshOptimizer.Core.OcclusionAlgorithms.Common;

namespace VoxelMeshOptimizer.Tests.Occlusion.Common;

public class VisiblePlaneTests
{
    [Fact]
    public void Constructor_ShouldInitializePropertiesCorrectly()
    {
        // Arrange
        VisiblePlane plane = new(Axis.X, AxisOrder.Ascending, Axis.Y, AxisOrder.Ascending, Axis.Z, AxisOrder.Ascending, 2, 3, 4);


        // Assert
        Assert.Equal(Axis.X, plane.MajorAxis);
        Assert.Equal(AxisOrder.Ascending, plane.MajorAxisOrder);

        Assert.Equal(Axis.Y, plane.MiddleAxis);
        Assert.Equal(AxisOrder.Ascending, plane.MiddleAxisOrder);

        Assert.Equal(Axis.Z, plane.MinorAxis);
        Assert.Equal(AxisOrder.Ascending, plane.MinorAxisOrder);

        Assert.Equal((uint)2, plane.SliceIndex);
        Assert.Equal(3, plane.Voxels.GetLength(0));
        Assert.Equal(4, plane.Voxels.GetLength(1));
    }

    [Fact]
    public void IsPlaneEmpty_ShouldReturnTrue_WhenNoVoxelsAreSet()
    {
        // Arrange
        VisiblePlane plane = new(Axis.X, AxisOrder.Ascending, Axis.Y, AxisOrder.Ascending, Axis.Z, AxisOrder.Ascending, 0, 2, 2);

        // Act & Assert
        Assert.True(plane.IsPlaneEmpty);
    }

    [Fact]
    public void IsPlaneEmpty_ShouldReturnFalse_WhenAtLeastOneVoxelIsSet()
    {
        // Arrange
        VisiblePlane plane = new(Axis.X, AxisOrder.Ascending, Axis.Y, AxisOrder.Ascending, Axis.Z, AxisOrder.Ascending, 0, 2, 2)
            {
                Voxels =
                {
                    [0, 0] = new Voxel(1)
                }
            };

        // Act & Assert
        Assert.False(plane.IsPlaneEmpty);
    }


    [Fact]
    public void Constructor_ShouldHandleZeroSizedPlane()
    {
        // Arrange
        VisiblePlane plane = new VisiblePlane(Axis.X, AxisOrder.Ascending, Axis.Y, AxisOrder.Ascending, Axis.Z, AxisOrder.Ascending, 0, 0, 0);

        // Assert
        Assert.Empty(plane.Voxels);
        Assert.True(plane.IsPlaneEmpty);
    }

    [Fact]
    public void Voxels_SetVoxel_ShouldBeAccessibleCorrectly()
    {
        // Arrange
        Voxel voxel = new(99);
        VisiblePlane plane = new(Axis.X, AxisOrder.Ascending, Axis.Y, AxisOrder.Ascending, Axis.Z, AxisOrder.Ascending, 0, 1, 1)
            {
                Voxels =
                {
                    [0, 0] = voxel
                }
            };

        // Assert
        Assert.Equal(voxel, plane.Voxels[0, 0]);
    }


    [Fact]
    public void ConvertToPixelArray_AllVoxelsNonNull_ReturnsCorrectIDs()
    {
        // Arrange: Create a 2x2 VisiblePlane with all voxels.
        Voxel?[,] voxels = new Voxel?[,]
        {
            { new Voxel(10), new Voxel(20) },
            { new Voxel(30), new Voxel(40) }
        };
        
        VisiblePlane plane = new VisiblePlane(
            majorAxis: Axis.X, majorAxisOrder: AxisOrder.Ascending,
            middleAxis: Axis.Y, middleAxisOrder: AxisOrder.Ascending,
            minorAxis: Axis.Z, minorAxisOrder: AxisOrder.Ascending,
            sliceIndex: 0,
            width: 2,
            height: 2)
        {
            Voxels = voxels
        };

        // Act
        int[,] result = plane.ConvertToPixelArray();

        // Assert: Verify that each element has the expected ID.
        Assert.Equal(10, result[0, 0]);
        Assert.Equal(20, result[0, 1]);
        Assert.Equal(30, result[1, 0]);
        Assert.Equal(40, result[1, 1]);
    }

    [Fact]
    public void ConvertToPixelArray_WithNullVoxels_ReturnsMinusOneForNulls()
    {
        // Arrange: Create a 3x2 VisiblePlane with some null voxels.
        Voxel?[,] voxels = new Voxel?[,]
        {
            { new Voxel(5), null },
            { null, new Voxel(15) },
            { new Voxel(25), new Voxel(35) }
        };

        VisiblePlane plane = new VisiblePlane(
            majorAxis: Axis.X, majorAxisOrder: AxisOrder.Ascending,
            middleAxis: Axis.Y, middleAxisOrder: AxisOrder.Ascending,
            minorAxis: Axis.Z, minorAxisOrder: AxisOrder.Ascending,
            sliceIndex: 1,
            width: 3,
            height: 2)
        {
            Voxels = voxels
        };

        // Act
        int[,] result = plane.ConvertToPixelArray();

        // Assert: Verify the proper mapping of non-null and null voxels.
        Assert.Equal(5, result[0, 0]);
        Assert.Equal(-1, result[0, 1]);
        Assert.Equal(-1, result[1, 0]);
        Assert.Equal(15, result[1, 1]);
        Assert.Equal(25, result[2, 0]);
        Assert.Equal(35, result[2, 1]);
    }


}
