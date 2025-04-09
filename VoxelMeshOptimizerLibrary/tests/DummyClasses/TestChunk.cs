using VoxelMeshOptimizer.Core;
using VoxelMeshOptimizer.Tests.DummyClasses;

public class TestChunk : Chunk<TestVoxel>
{
    private readonly TestVoxel[,,] data;

    public uint XDepth { get; }
    public uint YDepth { get; }
    public uint ZDepth { get; }

    public TestChunk(uint xDepth, uint yDepth, uint zDepth)
    {
        XDepth = xDepth;
        YDepth = yDepth;
        ZDepth = zDepth;
        data = new TestVoxel[xDepth, yDepth, zDepth];
    }
    
    /// <summary>
    /// Retrieves the voxel at the given position (X,Y,Z), ignoring the axis fields for now.
    /// Throws if out of range, or you could choose to return null.
    /// </summary>
    public TestVoxel Get(uint x, uint y, uint z)
    {
        if (x >= XDepth || y >= YDepth || z >= ZDepth)
        {
            throw new ArgumentOutOfRangeException("Requested voxel coordinates are out of bounds.");
        }

        return data[x, y, z];
    }

    /// <summary>
    /// Sets a voxel at the given position (X,Y,Z). 
    /// </summary>
    public void Set(uint x, uint y, uint z, TestVoxel voxel)
    {
        if (x >= XDepth || y >= YDepth || z >= ZDepth)
        {
            throw new ArgumentOutOfRangeException("Requested voxel coordinates are out of bounds.");
        }

        data[x, y, z] = voxel;
    }

    /// <summary>
    /// Iterates over every (X,Y,Z) in the chunk in the order of three distinct axes 
    /// (Major, Middle, Minor). The callback receives a VoxelPosition that includes 
    /// the coordinate plus the iteration axes for debugging or advanced logic.
    /// </summary>
    public void ForEachCoordinate(
        Axis majorA, AxisOrder majorAsc,
        Axis middleA, AxisOrder middleAsc,
        Axis minorA, AxisOrder minorAsc,
        Action<uint, uint, uint> action
    )
    {
        // 2) Ensure all axes are distinct
        if (majorA == middleA || middleA == minorA || majorA == minorA)
        {
            throw new ArgumentException("All three HumanAxis values must target different axes (X/Y/Z).");
        }

        // 4) Triple-nested loop in the order: major -> middle -> minor
        //    We find which axis is major, middle, minor, then nest them accordingly.
        foreach (var majorVal in BuildRange(GetDepth(majorA), majorAsc))
        {
            foreach (var midVal in BuildRange(GetDepth(middleA), middleAsc))
            {
                foreach (var minVal in BuildRange(GetDepth(minorA), minorAsc))
                {
                    uint x = 0, y = 0, z = 0;

                    if      (majorA == Axis.X) x = majorVal;
                    else if (majorA == Axis.Y) y = majorVal;
                    else if (majorA == Axis.Z) z = majorVal;

                    if      (middleA == Axis.X) x = midVal;
                    else if (middleA == Axis.Y) y = midVal;
                    else if (middleA == Axis.Z) z = midVal;

                    if      (minorA == Axis.X) x = minVal;
                    else if (minorA == Axis.Y) y = minVal;
                    else if (minorA == Axis.Z) z = minVal;

                    action(x, y, z);
                }
            }
        }
    }


    private IEnumerable<uint> BuildRange(uint size, AxisOrder order)
    {
        if (order == AxisOrder.Ascending)
        {
            for (uint i = 0; i < size; i++)
                yield return i;
        }
        else
        {
            for (int i = (int)size - 1; i >= 0; i--)
                yield return (uint)i;
        }
    }
    
    /// <summary>
    /// Simple helper to pick the chunk’s dimension (depth) by axis.
    /// </summary>
    public uint GetDepth(Axis axis)
    {
        return axis switch
        {
            Axis.X => XDepth,
            Axis.Y => YDepth,
            Axis.Z => ZDepth,
            _ => throw new ArgumentOutOfRangeException(nameof(axis), axis, "Unknown axis.")
        };
    }

    public bool IsOutOfBound(uint x, uint y, uint z){
        return x < 0 || x >= GetDepth(Axis.X) 
            || y < 0 || y >= GetDepth(Axis.Y)
            || z < 0 || z >= GetDepth(Axis.Z);
    }

    public bool AreDifferentAxis(
        Axis major,
        Axis middle,
        Axis minor
    ){
        return major != middle && middle != minor && minor != major;
    }

    public (uint planeWidth, uint planeHeight) GetPlaneDimensions(
        Axis major,
        Axis middle,
        Axis minor
    )
    {
        // "Plane dimensions" = minor dimension (x-axis of the plane),
        //                      middle dimension (y-axis of the plane).
        var planeWidth  = GetDepth(middle);
        var planeHeight = GetDepth(minor);

        return (planeWidth, planeHeight);
    }
}