using Xunit;
using VoxelMeshOptimizer.Core.OcclusionAlgorithms.Common;
using VoxelMeshOptimizer.Core;
using VoxelMeshOptimizer.Tests.DummyClasses;

namespace VoxelMeshOptimizer.Tests.OcclusionAlgorithms.Common;

public class VisiblePlaneTests
{
    [Fact]
    public void Constructor_ShouldInitializePropertiesCorrectly()
    {
        // Arrange
        var plane = new VisiblePlane(Axis.X, AxisOrder.Ascending, Axis.Y, AxisOrder.Ascending, Axis.Z, AxisOrder.Ascending, 2, 3, 4);


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
        var plane = new VisiblePlane(Axis.X, AxisOrder.Ascending, Axis.Y, AxisOrder.Ascending, Axis.Z, AxisOrder.Ascending, 0, 2, 2);

        // Act & Assert
        Assert.True(plane.IsPlaneEmpty);
    }

    [Fact]
    public void IsPlaneEmpty_ShouldReturnFalse_WhenAtLeastOneVoxelIsSet()
    {
        // Arrange
        var plane = new VisiblePlane(Axis.X, AxisOrder.Ascending, Axis.Y, AxisOrder.Ascending, Axis.Z, AxisOrder.Ascending, 0, 2, 2);
        plane.Voxels[0, 0] = new TestVoxel(1, true);

        // Act & Assert
        Assert.False(plane.IsPlaneEmpty);
    }

    [Fact]
    public void ToString_ShouldIncludeAxisOrderSigns()
    {
        var plane = new VisiblePlane(
            Axis.X, AxisOrder.Ascending,
            Axis.Y, AxisOrder.Descending,
            Axis.Z, AxisOrder.Ascending,
            1, 2, 2
        );

        string expected = "Plane(Major=+X, Middle=-Y, Minor=+Z, SliceIndex=1)";
        Assert.Equal(expected, plane.ToString());
    }

    [Fact]
    public void Describe_ShouldIncludeAxisOrderSignsAndVoxelData()
    {
        var plane = new VisiblePlane(
            Axis.X, AxisOrder.Ascending,
            Axis.Y, AxisOrder.Descending,
            Axis.Z, AxisOrder.Ascending,
            0, 2, 2
        );

        plane.Voxels[0, 0] = new TestVoxel(42, true);
        plane.Voxels[1, 1] = new TestVoxel(84, true);

        string expected = 
            "Plane(Major=+X, Middle=-Y, Minor=+Z, SliceIndex=0)\n" +
            "Voxels (each cell shows 'ID' or '.' if null):\n" +
            "Row 0: 42 . \n" +
            "Row 1: . 84 \n";

        Assert.Equal(expected, plane.Describe());
    }

    [Fact]
    public void Constructor_ShouldHandleZeroSizedPlane()
    {
        // Arrange
        var plane = new VisiblePlane(Axis.X, AxisOrder.Ascending, Axis.Y, AxisOrder.Ascending, Axis.Z, AxisOrder.Ascending, 0, 0, 0);

        // Assert
        Assert.Empty(plane.Voxels);
        Assert.True(plane.IsPlaneEmpty);
    }

    [Fact]
    public void Voxels_SetVoxel_ShouldBeAccessibleCorrectly()
    {
        // Arrange
        var voxel = new TestVoxel(99, true);
        var plane = new VisiblePlane(Axis.X, AxisOrder.Ascending, Axis.Y, AxisOrder.Ascending, Axis.Z, AxisOrder.Ascending, 0, 1, 1);
        plane.Voxels[0, 0] = voxel;

        // Assert
        Assert.Same(voxel, plane.Voxels[0, 0]);
    }
}
