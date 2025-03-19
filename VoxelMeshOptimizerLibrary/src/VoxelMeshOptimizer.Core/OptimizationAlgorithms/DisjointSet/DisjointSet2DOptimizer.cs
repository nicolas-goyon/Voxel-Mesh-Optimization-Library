using CommunityToolkit.Diagnostics;

namespace VoxelMeshOptimizer.Core.OptimizationAlgorithms.DisjointSet;

/// <summary>
/// Optimizes a 2D array of pixels by merging contiguous regions of the same color
/// using a disjoint set (union-find) data structure.
/// </summary>
public class DisjointSet2DOptimizer {

    private readonly DisjointSet disjointSet;
    private readonly int[,] pixels;
    private readonly int rows;
    private readonly int cols;

    /// <summary>
    /// Initializes a new instance of the <see cref="DisjointSet2DOptimizer"/> class.
    /// </summary>
    /// <param name="pixels">A 2D array representing pixel colors.</param>
    /// <exception cref="ArgumentNullException">Thrown when the pixels array is null.</exception>
    /// <exception cref="ArgumentException">Thrown when the pixels array is empty.</exception>
    public DisjointSet2DOptimizer(int[,] pixels) {
        Guard.IsNotNull(pixels, nameof(pixels));
        Guard.IsGreaterThan(pixels.GetLength(0), 0, nameof(pixels));
        Guard.IsGreaterThan(pixels.GetLength(1), 0, nameof(pixels));

        this.pixels = pixels;
        rows = pixels.GetLength(0);
        cols = pixels.GetLength(1);
        disjointSet = new DisjointSet(rows * cols);
    }

    /// <summary>
    /// Executes the optimization process by identifying and merging contiguous
    /// regions of the same color in the pixel array.
    /// </summary>
    public void Optimize() {
        CreateSets();
    }


    /// <summary>
    /// Creates disjoint sets for all pixels, merging contiguous regions of the same color.
    /// </summary>
    private void CreateSets() {
        for (int i = 0; i < rows; i++) {
            for (int j = 0; j < cols; j++) {
                // Skip pixels that are already part of a set
                if (IsNotAlone(i, j)) { 
                    continue;
                }
                CreateOneSet(i, j);
            }
        }
    }

    /// <summary>
    /// Creates a set for a contiguous region of pixels starting from the specified coordinates.
    /// </summary>
    /// <param name="x">The row index of the starting pixel.</param>
    /// <param name="y">The column index of the starting pixel.</param>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown when the coordinates (x, y) are out of the pixel array bounds.
    /// </exception>
    private void CreateOneSet(int x, int y) {
        Guard.IsInRange(x, 0, rows, nameof(x));
        Guard.IsInRange(y, 0, cols, nameof(y));

        int currentWidth = 1;
        int currentHeight = 1;

        // Expand the rectangle to the right
        int nextY = y + currentWidth;
        while (nextY < cols && pixels[x, y] == pixels[x, nextY] && !IsNotAlone(x, nextY))
        {
            currentWidth++;
            nextY = y + currentWidth;
        }

        // Expand the rectangle downward
        int nextX = x + currentHeight;
        while (nextX < rows)
        {
            bool sameColor = true;
            for (int i = y; i < y + currentWidth; i++)
            {
                if (pixels[x, y] != pixels[nextX, i] || IsNotAlone(nextX, i))
                {
                    sameColor = false;
                    break;
                }
            }

            if (!sameColor)
            {
                break;
            }

            currentHeight++;
            nextX = x + currentHeight;
        }


        // Merge the identified region into a single set
        for (int i = x; i < x + currentHeight; i++) {
            for (int j = y; j < y + currentWidth; j++) {
                disjointSet.Union(x * cols + y, i * cols + j);
            }
        }

    }

    /// <summary>
    /// Determines whether the pixel at the specified coordinates is part of an existing set.
    /// </summary>
    /// <param name="x">The row index of the pixel.</param>
    /// <param name="y">The column index of the pixel.</param>
    /// <returns>
    /// <c>true</c> if the pixel is part of an existing set; otherwise, <c>false</c>.
    /// </returns>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown when the coordinates (x, y) are out of the pixel array bounds.
    /// </exception>
    private bool IsNotAlone(int x, int y) {
        Guard.IsInRange(x, 0, rows, nameof(x));
        Guard.IsInRange(y, 0, cols, nameof(y));
        
        int root = disjointSet.Find(x * cols + y);

        if (root != x * cols + y) {
            return true;
        }

        // Check adjacent pixels
        return (x - 1 >= 0     && disjointSet.Find((x - 1) * cols + y) == root) ||
            (x + 1 < rows   && disjointSet.Find((x + 1) * cols + y) == root) ||
            (y - 1 >= 0     && disjointSet.Find(x * cols + y - 1)   == root) ||
            (y + 1 < cols   && disjointSet.Find(x * cols + y + 1)   == root);
        
    }


    /// <summary>
    /// Converts the disjoint sets into a list of pixel coordinate groups.
    /// </summary>
    /// <returns>
    /// A list where each element is a list of tuples representing the coordinates
    /// of pixels in the same set.
    /// </returns>
    public List<List<(int x, int y)>> ToResult() {

        Dictionary<int, List<(int x, int y)>> sets = [];

        for (int i = 0; i < rows; i++) {
            for (int j = 0; j < cols; j++) {
                int root = disjointSet.Find(i * cols + j);
                if (!sets.ContainsKey(root)) {
                    sets[root] = [];
                }
                sets[root].Add((i, j));
            }
        }

        List<List<(int x, int y)>> result = [];
        foreach (var set in sets) {
            result.Add(set.Value);
        }


        return result;
    }


}
