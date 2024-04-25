using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using static System.Net.Mime.MediaTypeNames;



/*
 * 
 * The challenge involves tiling a 2D pixel grid with rectangles (or squares) in such a way that all rectangles contain the same color within them.
 * 
 * A more mathematical interpretation:
 * A grid can be visualized as a matrix with n rows and n columns. Let's denote I and J as two intervals within the range [1,n]. A block within a matrix M with n rows and n columns is a submatrix M[I,J] corresponding to the intervals I and J.
 * Input: An nxn matrix with integer values within the range [1,k], representing colors.
 * Output: A division of the matrix into blocks such that each block is of a single color.
 * Goal: To develop a heuristic that is swift enough for real-time problem-solving.
 * 
 * 
 */
namespace ConsoleApp1 {

    public class DisjointSetOptimizer {

        DisjointSet disjointSet;
        int[,] pixels;
        int rows;
        int cols;


        public DisjointSetOptimizer(int[,] pixels) {
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



    public struct DisjointSet {
        private int[] parent;  // parent[i] = parent of i
        private int[] size;    // size[i] = number of sites in tree rooted at i
                               // Note: not necessarily correct if i is not a root node
        private int count;     // number of components

        /**
         * Initializes an empty union-find data structure with
         * {@code n} elements {@code 0} through {@code n-1}.
         * Initially, each element is in its own set.
         *
         * @param  n the number of elements
         * @throws IllegalArgumentException if {@code n < 0}
         */
        public DisjointSet(int n) {
            count = n;
            parent = new int[n];
            size = new int[n];
            for (int i = 0; i < n; i++) {
                parent[i] = i;
                size[i] = 1;
            }
        }

        /**
         * Returns the number of sets.
         *
         * @return the number of sets (between {@code 1} and {@code n})
         */
        public int GetCount() {
            return count;
        }

        public bool IsRoot(int p) {
            return parent[p] == p;
        }


        /**
         * Returns the canonical element of the set containing element {@code p}.
         *
         * @param  p an element
         * @return the canonical element of the set containing {@code p}
         * @throws IllegalArgumentException unless {@code 0 <= p < n}
         */
        public int Find(int p) {
            Validate(p);
            int root = p;
            while (root != parent[root])
                root = parent[root];
            while (p != root) {
                int newp = parent[p];
                parent[p] = root;
                p = newp;
            }
            return root;
        }

        // validate that p is a valid index
        private void Validate(int p) {
            int n = parent.Length;
            if (p < 0 || p >= n) {
                throw new System.Exception("index " + p + " is not between 0 and " + (n - 1));
            }
        }

        /**
         * Merges the set containing element {@code p} with the set
         * containing element {@code q}.
         *
         * @param  p one element
         * @param  q the other element
         * @throws IllegalArgumentException unless
         *         both {@code 0 <= p < n} and {@code 0 <= q < n}
         */
        public void Union(int p, int q) {
            int rootP = Find(p);
            int rootQ = Find(q);
            if (rootP == rootQ) return;

            // make smaller root point to larger one
            if (size[rootP] < size[rootQ]) {
                parent[rootP] = rootQ;
                size[rootQ] += size[rootP];
            }
            else {
                parent[rootQ] = rootP;
                size[rootP] += size[rootQ];
            }
            count--;
        }

    }


}
