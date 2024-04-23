# RectangelMerging


My problem 
To give you some context, I'm currently working on optimizing the display in a voxel-based video game, which are essentially single-color cubes, the equivalent of a pixel in 3D.

At the moment, I'm able to generate cubes from their respective coordinates and colors. However, the process becomes considerably slow when I add a large number of cubes.

To start, I'm looking to solve a more easily definable problem. I'm looking for an algorithm that, ideally, wouldn't necessarily provide an optimal solution, but could significantly reduce the number of rectangles used.

Imagine an orthogonal grid of variable size composed of numbers. The goal is to create squares or rectangles covering the grid, provided that all the numbers inside a rectangle are identical. Ideally, it would be preferable to have a solution with the fewest possible rectangles, but I think such an algorithm would not be feasible for real-time rendering. The goal would rather be to find an algorithm that could reduce by an average factor of 4 the number of rectangles.

Here are some examples to illustrate the problem:

1. Consider a chessboard grid. In this case, the solution is simple, because there are no two adjacent squares of the same color, so the grid can't be improved.
2. Consider a unicolor grid. Here, the solution is also simple, because the result is a single square filling the entire grid.
3. Consider a grid where each column alternates colors (column 1 in black, column 2 in white, column 3 in black, etc.). In this case, the ideal result would be vertical rectangles.

I think there is an approximation method for this type of problem, where the approximation would allow to obtain a lower average number of rectangles without requiring an exorbitant computation time.
