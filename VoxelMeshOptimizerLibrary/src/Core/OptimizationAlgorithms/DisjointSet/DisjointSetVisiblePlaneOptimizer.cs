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
    private readonly Chunk<Voxel> chunk;

    public DisjointSetVisiblePlaneOptimizer(VisiblePlane plane, Chunk<Voxel> chunk)
    {
        Guard.IsNotNull(plane, nameof(plane));
        Guard.IsNotNull(plane.Voxels, nameof(plane.Voxels));
        this.plane = plane;
        voxels = plane.Voxels;
        this.chunk = chunk;

        width = voxels.GetLength(0);
        height = voxels.GetLength(1);

        Guard.IsGreaterThan(width, 0, nameof(width));
        Guard.IsGreaterThan(height, 0, nameof(height));

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

        return (x > 0 && AreSame(x, y, x - 1, y)) ||
               (x < width - 1 && AreSame(x, y, x + 1, y)) ||
               (y > 0 && AreSame(x, y, x, y - 1)) ||
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
        foreach (var group in groups)
        {

            var groupVoxels = group.Value;

            var minX = groupVoxels.Min(p => p.x);
            var maxX = groupVoxels.Max(p => p.x);
            var minY = groupVoxels.Min(p => p.y);
            var maxY = groupVoxels.Max(p => p.y);


            int width = maxX - minX + 1;
            int height = maxY - minY + 1;



            MeshQuad quad = null;
            int voxelId = voxels[minX, minY]!.ID;
            
            
            switch (plane.MajorAxis, plane.MajorAxisOrder)
            {
                case (Axis.X, AxisOrder.Descending):
                    {
                        #region DebugVoxels
                            Console.WriteLine("\t\t==========================");
                            Console.WriteLine($"== FaceIndex :  {plane.SliceIndex}");
                            Console.WriteLine($"\tgroup : {group.Key}");
                            foreach (var val in group.Value)
                            {
                                Console.WriteLine($"\t\tx:{val.x} - y:{val.y}");
                            }
                            
                            Console.WriteLine("\tVoxels");
                            for (int testX = 0; testX < voxels.GetLength(0); testX++)
                            {
                                Console.Write("\t| ");
                                for (int testY = 0; testY < voxels.GetLength(1); testY++)
                                {
                                    if (group.Value.Contains((testX, testY))) Console.Write((voxels[testX, testY] == null ? "E" : voxels[testX, testY]!.ID) + " ");
                                    else Console.Write((voxels[testX, testY] == null ? " " : ".") + " ");
                                }
                                Console.WriteLine("|");
                            }
                            Console.WriteLine($"\t\tMinX:{minX} - MaxX:{maxX} - MinY:{minY} - MaxY:{maxY}");
                            Console.WriteLine($"\t\tWidth:{width} - Height:{height}");
                        #endregion

                        
                        uint x = chunk.XDepth - plane.SliceIndex;
                        var y1 = chunk.YDepth - minX;
                        var y2 = chunk.YDepth - maxX - 1;
                        var z1 = chunk.ZDepth - minY;
                        var z2 = chunk.ZDepth - maxY - 1;
                        quad = new MeshQuad
                        {
                            Vertex0 = new Vector3(x, y1, z1),
                            Vertex1 = new Vector3(x, y2, z1),
                            Vertex2 = new Vector3(x, y2, z2),
                            Vertex3 = new Vector3(x, y1, z2),
                            Normal = new Vector3(1, 0, 0),
                            VoxelID = voxelId
                        };
                        break;
                    }
                case (Axis.X, AxisOrder.Ascending):
                    // {
                    //     quad = new MeshQuad
                    //     {
                    //         Vertex0 = new Vector3(minX, by, bz),
                    //         Vertex1 = new Vector3(minX, by, bz + 1),
                    //         Vertex2 = new Vector3(minX, by + 1, bz + 1),
                    //         Vertex3 = new Vector3(minX, by + 1, bz),
                    //         Normal = new Vector3(-1, 0, 0),
                    //         VoxelID = voxelId
                    //     };
                    //     break;
                    // }
                case (Axis.Y, AxisOrder.Descending):
                    // {
                    //     quad = new MeshQuad
                    //     {
                    //         Vertex0 = new Vector3(bx, by + 1, bz + 1),
                    //         Vertex1 = new Vector3(bx + 1, by + 1, bz + 1),
                    //         Vertex2 = new Vector3(bx + 1, by + 1, bz),
                    //         Vertex3 = new Vector3(bx, by + 1, bz),
                    //         Normal = new Vector3(0, 1, 0),
                    //         VoxelID = voxelId
                    //     };
                    //     break;
                    // }
                case (Axis.Y, AxisOrder.Ascending):
                    // {
                    //     quad = new MeshQuad
                    //     {
                    //         Vertex0 = new Vector3(bx, by, bz),
                    //         Vertex1 = new Vector3(bx + 1, by, bz),
                    //         Vertex2 = new Vector3(bx + 1, by, bz + 1),
                    //         Vertex3 = new Vector3(bx, by, bz + 1),
                    //         Normal = new Vector3(0, -1, 0),
                    //         VoxelID = voxelId
                    //     };
                    //     break;
                    // }
                case (Axis.Z, AxisOrder.Descending):
                    // {
                    //     quad = new MeshQuad
                    //     {
                    //         Vertex0 = new Vector3(bx, by, bz + 1),
                    //         Vertex1 = new Vector3(bx + 1, by, bz + 1),
                    //         Vertex2 = new Vector3(bx + 1, by + 1, bz + 1),
                    //         Vertex3 = new Vector3(bx, by + 1, bz + 1),
                    //         Normal = new Vector3(0, 0, 1),
                    //         VoxelID = voxelId
                    //     };
                    //     break;
                    // }
                case (Axis.Z, AxisOrder.Ascending):
                    // {
                    //     quad = new MeshQuad
                    //     {
                    //         Vertex0 = new Vector3(bx, by, bz),
                    //         Vertex1 = new Vector3(bx, by + 1, bz),
                    //         Vertex2 = new Vector3(bx + 1, by + 1, bz),
                    //         Vertex3 = new Vector3(bx + 1, by, bz),
                    //         Normal = new Vector3(0, 0, -1),
                    //         VoxelID = voxelId
                    //     };
                    //     break;
                    // }
                    break;
                default: throw new ArgumentOutOfRangeException();
            }


            if (quad != null) quads.Add(quad);
        }

        return quads;
    }

}
