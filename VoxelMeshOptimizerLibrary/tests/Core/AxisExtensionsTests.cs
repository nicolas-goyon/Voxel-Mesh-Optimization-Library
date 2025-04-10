using System;
using Xunit;
using VoxelMeshOptimizer.Core;
using VoxelMeshOptimizer.Tests.DummyClasses;

namespace VoxelMeshOptimizer.Tests.Core;

#region AxisExtensionsTests
public class AxisExtensionsTests
{
    #region GetDepthFromAxis Tests

    [Fact]
    public void GetDepthFromAxis_Ascending_ReturnsCoordinateValue_ForAxisX()
    {
        // Arrange: Create a TestChunk with known dimensions.
        var chunk = new TestChunk(10, 20, 30);
        uint x = 3, y = 5, z = 7;

        // Act: For Axis.X and Ascending order, depth should equal the x coordinate.
        uint depth = AxisExtensions.GetDepthFromAxis(Axis.X, AxisOrder.Ascending, x, y, z, chunk);

        // Assert
        Assert.Equal(x, depth);
    }

    [Fact]
    public void GetDepthFromAxis_Descending_ReturnsFlippedValue_ForAxisX()
    {
        // Arrange: Create a TestChunk.
        var chunk = new TestChunk(10, 20, 30);
        uint x = 2, y = 10, z = 15;
        // For Descending order on Axis.X:
        // Expected depth = (chunk.XDepth - 1) - x = (10 - 1) - 2 = 7.
        uint expectedDepth = (chunk.XDepth - 1) - x;

        // Act
        uint depth = AxisExtensions.GetDepthFromAxis(Axis.X, AxisOrder.Descending, x, y, z, chunk);

        // Assert
        Assert.Equal(expectedDepth, depth);
    }

    [Fact]
    public void GetDepthFromAxis_OnNearFace_ReturnsZeroDepth()
    {
        // Arrange: Create a TestChunk.
        var chunk = new TestChunk(8, 8, 8);
        // For a voxel on the near face:
        uint coordinate = 0;

        // Act & Assert for each axis in Ascending order.
        Assert.Equal(0u, AxisExtensions.GetDepthFromAxis(Axis.X, AxisOrder.Ascending, coordinate, 4, 4, chunk));
        Assert.Equal(0u, AxisExtensions.GetDepthFromAxis(Axis.Y, AxisOrder.Ascending, 4, coordinate, 4, chunk));
        Assert.Equal(0u, AxisExtensions.GetDepthFromAxis(Axis.Z, AxisOrder.Ascending, 4, 4, coordinate, chunk));
    }

    [Fact]
    public void GetDepthFromAxis_OnFarFace_ReturnsMaxDepth_ForDescendingOrder()
    {
        // Arrange
        var chunk = new TestChunk(8, 8, 8);
        // For a voxel on the far face in Descending order:
        uint x = chunk.XDepth - 1, y = chunk.YDepth - 1, z = chunk.ZDepth - 1;
        // For Descending order, depth for x equals (chunk.XDepth - 1) - (chunk.XDepth - 1) = 0.
        // To test far positions, we can compare for one axis:
        uint expectedDepthX = (chunk.XDepth - 1) - x; // should be 0
        uint expectedDepthY = (chunk.YDepth - 1) - y; // should be 0
        uint expectedDepthZ = (chunk.ZDepth - 1) - z; // should be 0

        // Act & Assert
        Assert.Equal(expectedDepthX, AxisExtensions.GetDepthFromAxis(Axis.X, AxisOrder.Descending, x, 3, 3, chunk));
        Assert.Equal(expectedDepthY, AxisExtensions.GetDepthFromAxis(Axis.Y, AxisOrder.Descending, 3, y, 3, chunk));
        Assert.Equal(expectedDepthZ, AxisExtensions.GetDepthFromAxis(Axis.Z, AxisOrder.Descending, 3, 3, z, chunk));
    }

    [Fact]
    public void GetDepthFromAxis_OutOfBoundCoordinate_ThrowsArgumentOutOfRangeException()
    {
        // Arrange
        var chunk = new TestChunk(10, 10, 10);
        uint x = 11, y = 5, z = 5; // x is out-of-bound (since valid indices are 0-9)

        // Act & Assert
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            AxisExtensions.GetDepthFromAxis(Axis.X, AxisOrder.Ascending, x, y, z, chunk));

        // Test other coordinate out-of-bound situations:
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            AxisExtensions.GetDepthFromAxis(Axis.Y, AxisOrder.Ascending, 5, 11, 5, chunk));
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            AxisExtensions.GetDepthFromAxis(Axis.Z, AxisOrder.Ascending, 5, 5, 11, chunk));
    }

    #endregion

    #region DefineIterationOrder Tests

    [Fact]
    public void DefineIterationOrder_ForMajorAxisX_ReturnsExpectedMapping()
    {
        // Arrange:
        // For major axis X with Ascending order, default mapping is: major = X, middle = Y, minor = Z.
        Axis majorAxis = Axis.X;
        AxisOrder order = AxisOrder.Ascending;

        // Act:
        var iterationOrder = AxisExtensions.DefineIterationOrder(majorAxis, order);

        // Assert:
        Assert.Equal(Axis.X, iterationOrder.major);
        Assert.Equal(order, iterationOrder.majorOrder);
        Assert.Equal(Axis.Y, iterationOrder.middle);
        Assert.Equal(order, iterationOrder.middleOrder);
        Assert.Equal(Axis.Z, iterationOrder.minor);
        Assert.Equal(order, iterationOrder.minorOrder);
    }

    [Fact]
    public void DefineIterationOrder_ForMajorAxisY_ReturnsExpectedMapping()
    {
        // Arrange: For major axis Y with Descending order, mapping is:
        // major = Y, middle = Z, minor = X.
        Axis majorAxis = Axis.Y;
        AxisOrder order = AxisOrder.Descending;

        // Act:
        var iterationOrder = AxisExtensions.DefineIterationOrder(majorAxis, order);

        // Assert:
        Assert.Equal(Axis.Y, iterationOrder.major);
        Assert.Equal(order, iterationOrder.majorOrder);
        Assert.Equal(Axis.Z, iterationOrder.middle);
        Assert.Equal(order, iterationOrder.middleOrder);
        Assert.Equal(Axis.X, iterationOrder.minor);
        Assert.Equal(order, iterationOrder.minorOrder);
    }

    [Fact]
    public void DefineIterationOrder_ForMajorAxisZ_ReturnsExpectedMapping()
    {
        // Arrange: For major axis Z with Ascending order, mapping is:
        // major = Z, middle = Y, minor = X.
        Axis majorAxis = Axis.Z;
        AxisOrder order = AxisOrder.Ascending;

        // Act:
        var iterationOrder = AxisExtensions.DefineIterationOrder(majorAxis, order);

        // Assert:
        Assert.Equal(Axis.Z, iterationOrder.major);
        Assert.Equal(order, iterationOrder.majorOrder);
        Assert.Equal(Axis.Y, iterationOrder.middle);
        Assert.Equal(order, iterationOrder.middleOrder);
        Assert.Equal(Axis.X, iterationOrder.minor);
        Assert.Equal(order, iterationOrder.minorOrder);
    }

    [Fact]
    public void DefineIterationOrder_InvalidNonDistinctAxes_ThrowsException()
    {
        // Arrange:
        // Although the default mapping always creates distinct axes,
        // we simulate an error situation by manually invoking ForEachCoordinate (which enforces distinct axes)
        // with invalid repeated axis values.

        var chunk = new TestChunk(5, 5, 5);
        // Here, we purposely call ForEachCoordinate with non-distinct axes to trigger an exception.
        // The exception type from TestChunk.ForEachCoordinate is ArgumentException.
        Assert.Throws<ArgumentException>(() =>
        {
            // For example, using Axis.X for both major and middle.
            chunk.ForEachCoordinate(Axis.X, AxisOrder.Ascending, Axis.X, AxisOrder.Ascending, Axis.Z, AxisOrder.Ascending, (a, b, c) => { });
        });
    }

    #endregion

    #region GetSlicePlanePosition Tests

    [Fact]
    public void GetSlicePlanePosition_Ascending_ReturnsSameAbsoluteCoordinates()
    {
        // Arrange: Create a TestChunk.
        var chunk = new TestChunk(10, 20, 30);
        uint x = 3, y = 5, z = 7;
        // For Ascending order on the in-plane axes, the position is used as-is.
        // We choose:
        // sliceAxis = Z (order irrelevant), planeAxis1 = X, planeAxis2 = Y.
        Axis sliceAxis = Axis.Z;
        AxisOrder sliceOrder = AxisOrder.Ascending;
        Axis planeAxis1 = Axis.X;
        AxisOrder planeAxis1Order = AxisOrder.Ascending;
        Axis planeAxis2 = Axis.Y;
        AxisOrder planeAxis2Order = AxisOrder.Ascending;

        // Act:
        var slicePos = AxisExtensions.GetSlicePlanePosition(
            sliceAxis, sliceOrder,
            planeAxis1, planeAxis1Order,
            planeAxis2, planeAxis2Order,
            x, y, z, chunk);

        // Assert:
        // Since orders are Ascending, the in-plane positions should equal the absolute coordinates
        Assert.Equal(x, slicePos.planeX);
        Assert.Equal(y, slicePos.planeY);
    }

    [Fact]
    public void GetSlicePlanePosition_Descending_ReturnsFlippedCoordinatesForPlaneAxes()
    {
        // Arrange: Create a TestChunk with known dimensions.
        var chunk = new TestChunk(10, 20, 30);
        // Let the absolute coordinates be:
        uint x = 2, y = 3, z = 4;
        // For planeAxis1 (X) descending: flippedX = (chunk.XDepth - 1) - x = (10 - 1) - 2 = 7.
        uint expectedPlaneX = (chunk.XDepth - 1) - x;
        // For planeAxis2 (Y) descending: flippedY = (chunk.YDepth - 1) - y = (20 - 1) - 3 = 16.
        uint expectedPlaneY = (chunk.YDepth - 1) - y;

        // Use distinct slice and plane axes.
        Axis sliceAxis = Axis.Z;
        AxisOrder sliceOrder = AxisOrder.Descending; // Note: sliceOrder is not used in calculations.
        Axis planeAxis1 = Axis.X;
        AxisOrder planeAxis1Order = AxisOrder.Descending;
        Axis planeAxis2 = Axis.Y;
        AxisOrder planeAxis2Order = AxisOrder.Descending;

        // Act:
        var slicePos = AxisExtensions.GetSlicePlanePosition(
            sliceAxis, sliceOrder,
            planeAxis1, planeAxis1Order,
            planeAxis2, planeAxis2Order,
            x, y, z, chunk);

        // Assert:
        Assert.Equal(expectedPlaneX, slicePos.planeX);
        Assert.Equal(expectedPlaneY, slicePos.planeY);
    }

    [Fact]
    public void GetSlicePlanePosition_MixedOrders_SliceOrderDoesNotInfluenceOutput()
    {
        // Arrange: We test that using a different sliceOrder (Ascending vs Descending) does not affect the result.
        var chunk = new TestChunk(10, 20, 30);
        uint x = 4, y = 8, z = 2;

        Axis sliceAxis = Axis.Z;
        // Test with two different sliceOrder values.
        AxisOrder sliceOrderA = AxisOrder.Ascending;
        AxisOrder sliceOrderB = AxisOrder.Descending;
        Axis planeAxis1 = Axis.X;
        AxisOrder planeAxis1Order = AxisOrder.Ascending; // For simplicity, use Ascending for plane axes.
        Axis planeAxis2 = Axis.Y;
        AxisOrder planeAxis2Order = AxisOrder.Ascending;

        // Act:
        var slicePosA = AxisExtensions.GetSlicePlanePosition(
            sliceAxis, sliceOrderA,
            planeAxis1, planeAxis1Order,
            planeAxis2, planeAxis2Order,
            x, y, z, chunk);

        var slicePosB = AxisExtensions.GetSlicePlanePosition(
            sliceAxis, sliceOrderB,
            planeAxis1, planeAxis1Order,
            planeAxis2, planeAxis2Order,
            x, y, z, chunk);

        // Assert: Even though sliceOrder is different, output should remain identical.
        Assert.Equal(slicePosA.planeX, slicePosB.planeX);
        Assert.Equal(slicePosA.planeY, slicePosB.planeY);
    }

    [Fact]
    public void GetSlicePlanePosition_NonDistinctAxes_ThrowsArgumentException()
    {
        // Arrange:
        var chunk = new TestChunk(10, 10, 10);
        uint x = 5, y = 5, z = 5;
        // Use non-distinct axes (e.g., sliceAxis and planeAxis1 are both Axis.X)
        Axis sliceAxis = Axis.X;
        Axis planeAxis1 = Axis.X;  // Not allowed: must be distinct.
        Axis planeAxis2 = Axis.Y;
        AxisOrder order = AxisOrder.Ascending;

        // Act & Assert:
        Assert.Throws<ArgumentException>(() =>
            AxisExtensions.GetSlicePlanePosition(
                sliceAxis, order,
                planeAxis1, order,
                planeAxis2, order,
                x, y, z,
                chunk));
    }

    #endregion

    #region Invalid Enum Value Tests

    [Fact]
    public void GetDepthFromAxis_InvalidEnumValue_CastInteger_ThrowsExceptionOrHandlesGracefully()
    {
        // Arrange:
        // Here we cast an integer value (that isn't defined) to Axis.
        // This test is "nice to have" rather than required.
        var chunk = new TestChunk(10, 10, 10);
        uint x = 2, y = 2, z = 2;
        // Cast integer 100 (an undefined value) to Axis.
        Axis invalidAxis = (Axis)100;

        Assert.False(chunk.IsOutOfBound(x,y,z));

        // Act & Assert:
        // Depending on implementation, this could throw an exception.
        Assert.Throws<ArgumentException>(() =>
            AxisExtensions.GetDepthFromAxis(invalidAxis, AxisOrder.Ascending, x, y, z, chunk));
    }

    #endregion
}
#endregion
