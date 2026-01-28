using System.Numerics;
using CommunityToolkit.Diagnostics;
using VoxelMeshOptimizer.Core.OcclusionAlgorithms.Common;

namespace VoxelMeshOptimizer.Core.OptimizationAlgorithms.DisjointSet;

/// <summary>
/// Optimizes a 2D VisiblePlane by merging contiguous regions of solid voxels with the same ID.
/// </summary>
public class DisjointSetVisiblePlaneOptimizer
{
    private DisjointSet disjointSet;
    private readonly VisiblePlane plane;
    private readonly Voxel?[,] voxels;
    private readonly int width;
    private readonly int height;
    private readonly Chunk chunk;

    public DisjointSetVisiblePlaneOptimizer(VisiblePlane plane, Chunk chunk)
    {
        Guard.IsNotNull(plane);
        Guard.IsNotNull(plane.Voxels, nameof(plane.Voxels));
        this.plane = plane;
        voxels = plane.Voxels;
        this.chunk = chunk;

        width = voxels.GetLength(0);
        height = voxels.GetLength(1);

        Guard.IsGreaterThan(width, 0);
        Guard.IsGreaterThan(height, 0);

        disjointSet = new DisjointSet(width * height);
    }

    public void Optimize()
    {
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                if (voxels[x, y] == null || IsNotAlone(x, y))
                    continue;

                CreateOneSet(x, y);
            }
        }
    }

    private void CreateOneSet(int x, int y)
    {
        Guard.IsInRange(x, 0, width);
        Guard.IsInRange(y, 0, height);

        Voxel? rootVoxel = voxels[x, y];
        if (rootVoxel == null) return;

        int currentWidth = 1;
        int currentHeight = 1;

        // Expand to the right
        while (x + currentWidth < width &&
               voxels[x + currentWidth, y]?.ID == rootVoxel?.ID &&
               !IsNotAlone(x + currentWidth, y))
        {
            currentWidth++;
        }

        // Expand downward
        while (y + currentHeight < height)
        {
            bool canExpand = true;
            for (int dx = 0; dx < currentWidth; dx++)
            {
                Voxel? v = voxels[x + dx, y + currentHeight];
                if (v?.ID != rootVoxel?.ID || IsNotAlone(x + dx, y + currentHeight))
                {
                    canExpand = false;
                    break;
                }
            }
            if (!canExpand) break;
            currentHeight++;
        }

        // Union the whole block
        int rootIndex = ToIndex(x, y);
        for (int dy = 0; dy < currentHeight; dy++)
        {
            for (int dx = 0; dx < currentWidth; dx++)
            {
                disjointSet.Union(rootIndex, ToIndex(x + dx, y + dy));
            }
        }
    }

    private bool IsNotAlone(int x, int y)
    {
        Guard.IsInRange(x, 0, width);
        Guard.IsInRange(y, 0, height);

        Voxel? voxel = voxels[x, y];
        if (voxel == null) return true;

        int root = disjointSet.Find(ToIndex(x, y));
        if (root != ToIndex(x, y)) return true;

        return (x > 0 && AreSame(x, y, x - 1, y)) ||
               (x < width - 1 && AreSame(x, y, x + 1, y)) ||
               (y > 0 && AreSame(x, y, x, y - 1)) ||
               (y < height - 1 && AreSame(x, y, x, y + 1));
    }

    private bool AreSame(int x1, int y1, int x2, int y2)
    {
        Voxel? v1 = voxels[x1, y1];
        Voxel? v2 = voxels[x2, y2];
        if (!v1.HasValue || !v2.HasValue) return false;
        return v1.Value.ID == v2.Value.ID &&
               disjointSet.Find(ToIndex(x2, y2)) == disjointSet.Find(ToIndex(x1, y1));
    }

    private int ToIndex(int x, int y) => y * width + x;


    public List<MeshQuad> ToMeshQuads()
    {
        Dictionary<int, List<(int x, int y)>> groups = new();


        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                if (voxels[x, y] == null) continue;

                int root = disjointSet.Find(ToIndex(x, y));
                if (!groups.TryGetValue(root, out List<(int x, int y)>? value))
                {
                    value = [];
                    groups[root] = value;
                }

                value.Add((x, y));
            }
        }




        List<MeshQuad> quads = [];
        MeshQuad? quad; // Keep outside of loop, it keeps being re-created
        foreach (KeyValuePair<int, List<(int x, int y)>> group in groups)
        {

            List<(int x, int y)> groupVoxels = group.Value;

            int minX = groupVoxels.Min(p => p.x);
            int maxX = groupVoxels.Max(p => p.x);
            int minY = groupVoxels.Min(p => p.y);
            int maxY = groupVoxels.Max(p => p.y);


            int voxelId = voxels[minX, minY]!.Value.ID;


            switch (plane.MajorAxis, plane.MajorAxisOrder)
            {
                case (Axis.X, AxisOrder.Descending):
                    {
                        uint x = chunk.XDepth - plane.SliceIndex;
                        long y1 = chunk.YDepth - minX;
                        long y2 = chunk.YDepth - maxX - 1;
                        long z1 = chunk.ZDepth - minY;
                        long z2 = chunk.ZDepth - maxY - 1;
                        quad = new MeshQuad
                        {
                            Vertex0 = new Vector3(x, y1, z1),
                            Vertex1 = new Vector3(x, y2, z1),
                            Vertex2 = new Vector3(x, y2, z2),
                            Vertex3 = new Vector3(x, y1, z2),
                            Normal = new Vector3(1, 0, 0),
                            VoxelId = voxelId
                        };
                        break;
                    }
                case (Axis.X, AxisOrder.Ascending):
                    {
                        
                        uint x = plane.SliceIndex;
                        int y1 = minX;
                        int y2 = maxX + 1;
                        int z1 = minY;
                        int z2 = maxY + 1;
                        quad = new MeshQuad
                        {
                            Vertex1 = new Vector3(x, y1, z1),
                            Vertex0 = new Vector3(x, y2, z1),
                            Vertex3 = new Vector3(x, y2, z2),
                            Vertex2 = new Vector3(x, y1, z2),
                            Normal = new Vector3(1, 0, 0),
                            VoxelId = voxelId
                        };
                        break;
                    }
                case (Axis.Y, AxisOrder.Descending):
                {
                        long x1 = chunk.XDepth - minY;
                        long x2 = chunk.XDepth - maxY - 1;
                        uint y = chunk.YDepth - plane.SliceIndex;
                        long z1 = chunk.ZDepth - minX;
                        long z2 = chunk.ZDepth - maxX - 1;
                        quad = new MeshQuad
                        {
                            Vertex0 = new Vector3(x1, y, z1),
                            Vertex1 = new Vector3(x1, y, z2),
                            Vertex2 = new Vector3(x2, y, z2),
                            Vertex3 = new Vector3(x2, y, z1),
                            Normal = new Vector3(1, 0, 0),
                            VoxelId = voxelId
                        };
                        break;
                    }
                case (Axis.Y, AxisOrder.Ascending):
                {
                        int x1 = minY;
                        int x2 = maxY + 1;
                        uint y = plane.SliceIndex;
                        int z1 = minX;
                        int z2 = maxX + 1;
                        quad = new MeshQuad
                        {
                            Vertex1 = new Vector3(x1, y, z1),
                            Vertex0 = new Vector3(x1, y, z2),
                            Vertex3 = new Vector3(x2, y, z2),
                            Vertex2 = new Vector3(x2, y, z1),
                            Normal = new Vector3(1, 0, 0),
                            VoxelId = voxelId
                        };
                        break;
                    }
                case (Axis.Z, AxisOrder.Descending):
                {
                        long x1 = chunk.XDepth - minY;
                        long x2 = chunk.XDepth - maxY - 1;
                        long y1 = chunk.YDepth - minX;
                        long y2 = chunk.YDepth - maxX - 1;
                        uint z = chunk.ZDepth - plane.SliceIndex;
                        quad = new MeshQuad
                        {
                            Vertex1 = new Vector3(x1, y1, z),
                            Vertex0 = new Vector3(x1, y2, z),
                            Vertex3 = new Vector3(x2, y2, z),
                            Vertex2 = new Vector3(x2, y1, z),
                            Normal = new Vector3(1, 0, 0),
                            VoxelId = voxelId
                        };
                        break;
                    }
                case (Axis.Z, AxisOrder.Ascending):
                    {
                        int x1 = minY;
                        int x2 = maxY + 1;
                        int y1 = minX;
                        int y2 = maxX + 1;
                        uint z = plane.SliceIndex;
                        quad = new MeshQuad
                        {
                            Vertex0 = new Vector3(x1, y1, z),
                            Vertex1 = new Vector3(x1, y2, z),
                            Vertex2 = new Vector3(x2, y2, z),
                            Vertex3 = new Vector3(x2, y1, z),
                            Normal = new Vector3(1, 0, 0),
                            VoxelId = voxelId
                        };
                        break;
                    }
                default: throw new ArgumentOutOfRangeException();
            }


            if (!quad.HasValue) throw new Exception("Unexpected null value");

            quads.Add(quad.Value);
        }

        return quads;
    }

}
