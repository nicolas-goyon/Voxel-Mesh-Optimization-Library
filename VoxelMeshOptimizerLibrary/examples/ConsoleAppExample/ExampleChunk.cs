using System;
using System.Collections.Generic;
using VoxelMeshOptimizer.Core;

namespace ConsoleAppExample
{
    public class ExampleChunk : Chunk<ExampleVoxel>
    {
        private readonly ExampleVoxel[,,] _voxels;

        public uint Width { get; }
        public uint Height { get; }
        public uint Depth { get; }

        public ExampleChunk(ushort[,,] voxelArray)
        {
            Width = (uint)voxelArray.GetLength(0);
            Height = (uint)voxelArray.GetLength(1);
            Depth = (uint)voxelArray.GetLength(2);

            _voxels = new ExampleVoxel[Width, Height, Depth];

            // Initialize from the ushort array
            for (uint x = 0; x < Width; x++)
            {
                for (uint y = 0; y < Height; y++)
                {
                    for (uint z = 0; z < Depth; z++)
                    {
                        ushort value = voxelArray[x, y, z];
                        _voxels[x, y, z] = new ExampleVoxel(value);
                    }
                }
            }
        }

        /// <summary>
        /// Returns all voxels in this chunk in no guaranteed order.
        /// </summary>
        public IEnumerable<ExampleVoxel> GetVoxels()
        {
            for (uint x = 0; x < Width; x++)
            {
                for (uint y = 0; y < Height; y++)
                {
                    for (uint z = 0; z < Depth; z++)
                    {
                        yield return _voxels[x, y, z];
                    }
                }
            }
        }

        /// <summary>
        /// Retrieves the voxel at the given position (X,Y,Z), ignoring the axis fields for now.
        /// Throws if out of range, or you could choose to return null.
        /// </summary>
        public ExampleVoxel Get(uint x, uint y, uint z)
        {
            if (x >= Width || y >= Height || z >= Depth)
            {
                throw new ArgumentOutOfRangeException("Requested voxel coordinates are out of bounds.");
            }

            return _voxels[x, y, z];
        }

        /// <summary>
        /// Sets a voxel at the given position (X,Y,Z). 
        /// </summary>
        public void Set(uint x, uint y, uint z, ExampleVoxel voxel)
        {
            if (x >= Width || y >= Height || z >= Depth)
            {
                throw new ArgumentOutOfRangeException("Requested voxel coordinates are out of bounds.");
            }

            _voxels[x, y, z] = voxel;
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
            foreach (var majorVal in BuildRange(GetDimension(majorA), majorAsc))
            {
                foreach (var midVal in BuildRange(GetDimension(middleA), middleAsc))
                {
                    foreach (var minVal in BuildRange(GetDimension(minorA), minorAsc))
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
        public uint GetDimension(Axis axis)
        {
            return axis switch
            {
                Axis.X => Width,
                Axis.Y => Height,
                Axis.Z => Depth,
                _ => throw new ArgumentOutOfRangeException(nameof(axis), axis, "Unknown axis.")
            };
        }

    }
}
