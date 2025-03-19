namespace VoxelMeshOptimizer.Core.OptimizationAlgorithms.DisjointSet;

public class DisjointSet2DOptimizer {

    DisjointSet disjointSet;
    int[,] pixels;
    int rows;
    int cols;


    public DisjointSet2DOptimizer(int[,] pixels) {
        this.pixels = pixels;
        rows = pixels.GetLength(0);
        cols = pixels.GetLength(1);
        disjointSet = new DisjointSet(rows * cols);
    }

    /**
        * 
        */
    public void Optimize() {
        CreateSets();
    }

    public void CreateSets() {
        for (int i = 0; i < rows; i++) {
            for (int j = 0; j < cols; j++) {
                // do not create a set if the pixel is already part of a set
                if (IsNotAlone(i, j)) { 
                    continue;
                }
                CreateOneSet(i, j);
            }
        }
    }


    // Create a rectangle set starting from the pixel at (x,y) with the same color
    public void CreateOneSet(int x, int y) {
        int currentWidth = 1;
        int currentHeight = 1;

        // Grow the rectangle to the right (at the end, 1xK rectangle)
        while (true) {
            int nextY = y + currentWidth;
            // Check if the next column is within the bounds (break if not)
            if (nextY >= cols) {
                break;
            }

            // Check if the next pixel has the same color (break if not)
            if (pixels[x, y] != pixels[x, nextY] || IsNotAlone(x, nextY)) { 
                break;
            }


            currentWidth += 1;
        }
        // Grow the rectangle downwards (at the end, NxK rectangle)
        while (true) {
            int nextX = x + currentHeight;
            // Check if the next row is within the bounds (break if not)
            if (nextX >= rows) {
                break;
            }

            // Check if the next row of pixels has the same color (break if not)
            bool sameColor = true;
            for (int i = y; i < y + currentWidth; i++) {
                if (pixels[x, y] != pixels[nextX, i] || IsNotAlone(nextX, i)) { 
                    sameColor = false;
                    break;
                }
            }

            if (!sameColor) {
                break;
            }

            currentHeight += 1;
        }



        // Create the set
        for (int i = x; i < x + currentHeight; i++) {
            for (int j = y; j < y + currentWidth; j++) {
                disjointSet.Union(x * cols + y, i * cols + j);
            }
        }

    }

    public bool IsNotAlone(int x, int y) {
        int root = disjointSet.Find(x * cols + y);

        if (root != x * cols + y) {
            return true;
        }

        // Check adjacent pixels
        if (x - 1 >= 0 && disjointSet.Find((x - 1) * cols + y) == root) {
            return true;
        }

        if (x + 1 < rows && disjointSet.Find((x + 1) * cols + y) == root) {
            return true;
        }

        if (y - 1 >= 0 && disjointSet.Find(x * cols + y - 1) == root) {
            return true;
        }

        if (y + 1 < cols && disjointSet.Find(x * cols + y + 1) == root) {
            return true;
        }
        return false;
    }




    public void DisplaySets() {
        // For each set, display the pixels
        int[,] roots = new int[rows, cols];
        for (int i = 0; i < rows; i++) {
            for (int j = 0; j < cols; j++) {
                roots[i, j] = disjointSet.Find(i * cols + j);
            }
        }

        for (int i = 0; i < rows; i++) {
            for (int j = 0; j < cols; j++) {
                Console.Write(roots[i, j] + " ");
            }
            Console.WriteLine();
        }
    }


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
