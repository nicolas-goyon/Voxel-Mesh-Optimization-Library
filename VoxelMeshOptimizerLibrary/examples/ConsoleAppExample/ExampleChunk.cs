using System;
using System.Collections.Generic;
using VoxelMeshOptimizer.Core;
using System.Numerics;

namespace ConsoleAppExample
{
    public class ExampleChunk : Chunk<ExampleVoxel>
    {
        private readonly ExampleVoxel[,,] _voxels;

        public uint XDepth { get; }
        public uint YDepth { get; }
        public uint ZDepth { get; }

        public ExampleChunk(ushort[,,] voxelArray)
        {
            XDepth = (uint)voxelArray.GetLength(0);
            YDepth = (uint)voxelArray.GetLength(1);
            ZDepth = (uint)voxelArray.GetLength(2);

            _voxels = new ExampleVoxel[XDepth, YDepth, ZDepth];

            // Initialize from the ushort array
            for (uint x = 0; x < XDepth; x++)
            {
                for (uint y = 0; y < YDepth; y++)
                {
                    for (uint z = 0; z < ZDepth; z++)
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
            for (uint x = 0; x < XDepth; x++)
            {
                for (uint y = 0; y < YDepth; y++)
                {
                    for (uint z = 0; z < ZDepth; z++)
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
            if (x >= XDepth || y >= YDepth || z >= ZDepth)
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
            if (x >= XDepth || y >= YDepth || z >= ZDepth)
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
            foreach (var majorVal in BuildRange(GetDepth(majorA), majorAsc))
            {
                foreach (var midVal in BuildRange(GetDepth(middleA), middleAsc))
                {
                    foreach (var minVal in BuildRange(GetDepth(minorA), minorAsc))
                    {
                        uint x = 0, y = 0, z = 0;

                        if (majorA == Axis.X) x = majorVal;
                        else if (majorA == Axis.Y) y = majorVal;
                        else if (majorA == Axis.Z) z = majorVal;

                        if (middleA == Axis.X) x = midVal;
                        else if (middleA == Axis.Y) y = midVal;
                        else if (middleA == Axis.Z) z = midVal;

                        if (minorA == Axis.X) x = minVal;
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

        public bool IsOutOfBound(uint x, uint y, uint z)
        {
            return x < 0 || x >= GetDepth(Axis.X)
                || y < 0 || y >= GetDepth(Axis.Y)
                || z < 0 || z >= GetDepth(Axis.Z);
        }

        public bool AreDifferentAxis(
            Axis major,
            Axis middle,
            Axis minor
        )
        {
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
            var planeWidth = GetDepth(middle);
            var planeHeight = GetDepth(minor);

            return (planeWidth, planeHeight);
        }
        
        
        /// <summary>
        /// Builds a mesh that contains every face for each solid voxel in the chunk.
        /// This is a naïve implementation without any form of optimization.
        /// </summary>
        public Mesh ToMesh()
        {
            var list = new List<MeshQuad>();

            for (uint x = 0; x < XDepth; x++)
            {
                for (uint y = 0; y < YDepth; y++)
                {
                    for (uint z = 0; z < ZDepth; z++)
                    {
                        var voxel = _voxels[x, y, z];
                        if (!voxel.IsSolid)
                            continue;

                        list.AddRange(CreateVoxelQuads(x, y, z, voxel.ID));
                    }
                }
            }
            var mesh = new ExampleMesh(list);

            return mesh;
        }

        private static IEnumerable<MeshQuad> CreateVoxelQuads(uint x, uint y, uint z, ushort voxelId)
        {
            var bx = (float)x;
            var by = (float)y;
            var bz = (float)z;


            yield return new MeshQuad
            {
                Vertex0 = new Vector3(bx, by, bz),
                Vertex1 = new Vector3(bx, by + 1, bz),
                Vertex2 = new Vector3(bx + 1, by + 1, bz),
                Vertex3 = new Vector3(bx + 1, by, bz),
                Normal = new Vector3(0, 0, -1),
                VoxelID = voxelId
            };

            yield return new MeshQuad
            {
                Vertex0 = new Vector3(bx, by, bz + 1),
                Vertex1 = new Vector3(bx + 1, by, bz + 1),
                Vertex2 = new Vector3(bx + 1, by + 1, bz + 1),
                Vertex3 = new Vector3(bx, by + 1, bz + 1),
                Normal = new Vector3(0, 0, 1),
                VoxelID = voxelId
            };

            yield return new MeshQuad
            {
                Vertex0 = new Vector3(bx, by, bz),
                Vertex1 = new Vector3(bx, by, bz + 1),
                Vertex2 = new Vector3(bx, by + 1, bz + 1),
                Vertex3 = new Vector3(bx, by + 1, bz),
                Normal = new Vector3(-1, 0, 0),
                VoxelID = voxelId
            };

            yield return new MeshQuad
            {
                Vertex0 = new Vector3(bx + 1, by, bz + 1),
                Vertex1 = new Vector3(bx + 1, by, bz),
                Vertex2 = new Vector3(bx + 1, by + 1, bz),
                Vertex3 = new Vector3(bx + 1, by + 1, bz + 1),
                Normal = new Vector3(1, 0, 0),
                VoxelID = voxelId
            };

            yield return new MeshQuad
            {
                Vertex0 = new Vector3(bx, by, bz),
                Vertex1 = new Vector3(bx + 1, by, bz),
                Vertex2 = new Vector3(bx + 1, by, bz + 1),
                Vertex3 = new Vector3(bx, by, bz + 1),
                Normal = new Vector3(0, -1, 0),
                VoxelID = voxelId
            };

            yield return new MeshQuad
            {
                Vertex0 = new Vector3(bx, by + 1, bz + 1),
                Vertex1 = new Vector3(bx + 1, by + 1, bz + 1),
                Vertex2 = new Vector3(bx + 1, by + 1, bz),
                Vertex3 = new Vector3(bx, by + 1, bz),
                Normal = new Vector3(0, 1, 0),
                VoxelID = voxelId
            };
        }


    }
}
