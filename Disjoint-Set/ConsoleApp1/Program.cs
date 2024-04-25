// See https://aka.ms/new-console-template for more information
using System.Timers;

namespace ConsoleApp1 {
    class Program {
        private static int[,] pixelsData;

        static void Main(string[] args) {
            Console.WriteLine("Hello : ");

            //int[,] pixels = new int[,]{ // 11x10
            //        {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
            //        {0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0 },
            //        {0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0 },
            //        {0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0 },
            //        {0, 1, 1, 1, 1, 2, 1, 1, 1, 1, 0 },
            //        {0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0 },
            //        {0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0 },
            //        {0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0 },
            //        {0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0 },
            //        {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }
            //    };

            //int[,] pixels = new int[,]{ // 11x10
            //        {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
            //        {0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0 },
            //        {0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0 },
            //        {0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0 },
            //        {0, 1, 1, 1, 1, 2, 2, 2, 2, 2, 0 },
            //        {0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0 },
            //        {0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0 },
            //        {0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0 },
            //        {0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0 },
            //        {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }
            //    };

            //int[,] pixels = new int[,] {
            //    {0, 1, 1, 1},
            //    {0, 1, 0, 0},
            //    {0, 0, 2, 0},
            //    {1, 1, 1, 1},
            //};


            Random random = new Random();
            int[,] pixels = new int[100, 100];
            for (int i = 0; i < pixels.GetLength(0); i++) {
                for (int j = 0; j < pixels.GetLength(1); j++) {
                    pixels[i, j] = random.Next(0, 3);
                }
            }

            pixelsData = pixels;

            //for (int i = 0; i < pixels.GetLength(0); i++) {
            //    for (int j = 0; j < pixels.GetLength(1); j++) {
            //        Console.Write(pixels[i, j] + " ");
            //    }
            //    Console.WriteLine();
            //}

            Console.WriteLine("Optimizing...");

            ////MeshOptimisation meshOptimisation = new MeshOptimisation(pixels);
            DisjointSetOptimizer optimizer = new DisjointSetOptimizer(pixels);
            long startTime = DateTime.Now.Ticks;
            optimizer.Optimize();
            long endTime = DateTime.Now.Ticks;
            float timeTaken = (endTime - startTime) / 10000;
            Console.WriteLine("Time taken: " + timeTaken + "ms");
            //optimizer.DisplaySets();
            Console.WriteLine("Is answer correct: " + checkAnswer(optimizer.ToResult()));


            //MeshOptimisation meshOptimisation = new MeshOptimisation(pixels);
            //long startTime2 = DateTime.Now.Ticks;
            //meshOptimisation.Optimize();
            //long endTime2 = DateTime.Now.Ticks;
            //float timeTaken2 = (endTime2 - startTime2) / 10000;
            //Console.WriteLine("Time taken: " + timeTaken2 + "ms");
            //meshOptimisation.Display();

        }


        public static bool checkAnswer(List<List<(int x, int y)>> components) {
            // For each upper list, each element should be have the same color aswell as being a rectangle
            foreach (List<(int x, int y)> component in components) {
                if (!isSameColor(component) || !isARectangle(component)) {
                    Console.WriteLine("Error with : " + component.Count);
                    Console.WriteLine("Position : " + component[0].x + " " + component[0].y);

                    Console.WriteLine("Component Data :");
                    foreach ((int x, int y) coordinate in component) {
                        Console.WriteLine(coordinate.x + " " + coordinate.y);
                    }


                    return false;
                }
            }
            return true;
        }

        public static bool isARectangle(List<(int x, int y)> component) {
            // Check if the component is a rectangle

            int minX = component[0].x;
            int minY = component[0].y;
            int maxX = component[0].x;
            int maxY = component[0].y;

            foreach ((int x, int y) coordinate in component) {
                if (coordinate.x < minX) {
                    minX = coordinate.x;
                }
                if (coordinate.y < minY) {
                    minY = coordinate.y;
                }
                if (coordinate.x > maxX) {
                    maxX = coordinate.x;
                }
                if (coordinate.y > maxY) {
                    maxY = coordinate.y;
                }
            }

            int width = maxX - minX + 1;
            int height = maxY - minY + 1;
            return component.Count == width * height;

        }

        public static bool isSameColor(List<(int x, int y)> component) {
            // Check if the component is the same color
            int color = pixelsData[component[0].x, component[0].y];
            foreach ((int x, int y) coordinate in component) {
                if (pixelsData[coordinate.x, coordinate.y] != color) { 
                    return false;
                }
            }
            return true;

        }

    }
}


