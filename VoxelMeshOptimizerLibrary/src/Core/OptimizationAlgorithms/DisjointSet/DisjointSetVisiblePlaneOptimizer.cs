using System.Numerics;
using CommunityToolkit.Diagnostics;
using VoxelMeshOptimizer.Core.OcclusionAlgorithms.Common;

namespace VoxelMeshOptimizer.Core.OptimizationAlgorithms.DisjointSet;

/// <summary>
/// Optimizes a 2D VisiblePlane by merging contiguous regions of solid voxels with the same ID.
/// </summary>
public class DisjointSetVisiblePlaneOptimizer
{
    private readonly DisjointSet disjointSet;
    private readonly VisiblePlane plane;
    private readonly Voxel?[,] voxels;
    private readonly int width;
    private readonly int height;

    public DisjointSetVisiblePlaneOptimizer(VisiblePlane plane)
    {
        Guard.IsNotNull(plane, nameof(plane));
        Guard.IsNotNull(plane.Voxels, nameof(plane.Voxels));
        this.plane = plane;
        voxels = plane.Voxels;
        width = voxels.GetLength(0);
        height = voxels.GetLength(1);

        Guard.IsGreaterThan(width, 0, nameof(width));
        Guard.IsGreaterThan(height, 0, nameof(height));

        disjointSet = new DisjointSet(width * height);
    }

    public void Optimize()
    {
        CreateSets();
    }

    private void CreateSets()
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
        Guard.IsInRange(x, 0, width, nameof(x));
        Guard.IsInRange(y, 0, height, nameof(y));

        var rootVoxel = voxels[x, y];
        if (rootVoxel == null) return;

        int currentWidth = 1;
        int currentHeight = 1;

        // Expand to the right
        while (x + currentWidth < width &&
               voxels[x + currentWidth, y]?.ID == rootVoxel.ID &&
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
                var v = voxels[x + dx, y + currentHeight];
                if (v?.ID != rootVoxel.ID || IsNotAlone(x + dx, y + currentHeight))
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
        Guard.IsInRange(x, 0, width, nameof(x));
        Guard.IsInRange(y, 0, height, nameof(y));

        var voxel = voxels[x, y];
        if (voxel == null) return true;

        int root = disjointSet.Find(ToIndex(x, y));
        if (root != ToIndex(x, y)) return true;

        return (x > 0     && AreSame(x, y, x - 1, y)) ||
               (x < width - 1 && AreSame(x, y, x + 1, y)) ||
               (y > 0     && AreSame(x, y, x, y - 1)) ||
               (y < height - 1 && AreSame(x, y, x, y + 1));
    }

    private bool AreSame(int x1, int y1, int x2, int y2)
    {
        var v1 = voxels[x1, y1];
        var v2 = voxels[x2, y2];
        if (v1 == null || v2 == null) return false;
        return v1.ID == v2.ID &&
               disjointSet.Find(ToIndex(x2, y2)) == disjointSet.Find(ToIndex(x1, y1));
    }

    private int ToIndex(int x, int y) => y * width + x;


    public List<MeshQuad> ToMeshQuads()
    {
        var groups = new Dictionary<int, List<(int x, int y)>>();

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                if (voxels[x, y] == null) continue;

                int root = disjointSet.Find(ToIndex(x, y));
                if (!groups.ContainsKey(root))
                {
                    groups[root] = [];
                }
                groups[root].Add((x, y));
            }
        }

        var quads = new List<MeshQuad>();
        foreach (var group in groups.Values)
        {
            var minX = group.Min(p => p.x);
            var maxX = group.Max(p => p.x);
            var minY = group.Min(p => p.y);
            var maxY = group.Max(p => p.y);

            int width = maxX - minX + 1;
            int height = maxY - minY + 1;

            var origin = new Vector3();
            origin.SetAxis(plane.MajorAxis, plane.MajorAxisOrder == AxisOrder.Ascending ? minX : -minX);
            origin.SetAxis(plane.MiddleAxis, plane.MiddleAxisOrder == AxisOrder.Ascending ? minY : -minY);
            origin.SetAxis(plane.MinorAxis, plane.MinorAxisOrder == AxisOrder.Ascending ? (float)plane.SliceIndex : -(float)plane.SliceIndex);

            var right = AxisExtensions.Direction(plane.MajorAxis, plane.MajorAxisOrder);
            var up    = AxisExtensions.Direction(plane.MiddleAxis, plane.MiddleAxisOrder);
            var normal = AxisExtensions.Direction(plane.MinorAxis, plane.MinorAxisOrder);

            // Construct corners (clockwise)
            var v0 = origin;
            var v1 = origin + right * width;
            var v2 = origin + right * width + up * height;
            var v3 = origin + up * height;

            int voxelId = voxels[minX, minY]!.ID;

            quads.Add(new MeshQuad
            {
                Vertex0 = v0,
                Vertex1 = v1,
                Vertex2 = v2,
                Vertex3 = v3,
                Normal = normal,
                VoxelID = voxelId
            });
        }

        return quads;
    }

}
