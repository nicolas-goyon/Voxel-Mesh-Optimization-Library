from Rectangle import Rectangle
from generalUtils import displayGrid

class SolveGrid :
    grid = [] # [[...], [...], [...], ...]
    width = 0
    height = 0
    rectangleCollection = [] # [Rectangle, Rectangle, ...]
    baseGrid = [] # [[...], [...], [...], ...]
    
            
    def __init__(self, baseGrid):
        self.height = len(baseGrid)
        self.width = len(baseGrid[0])
        self.grid = [[0 for i in range(self.width)] for j in range(self.height)]
        self.baseGrid = baseGrid
        self.rectangleCollection = [Rectangle(0, 0, 0, 0) for i in range(self.height * self.width)]
        index = 0
        for i in range(self.height):
            for j in range(self.width):
                self.grid[i][j] = index
                self.rectangleCollection[index] = Rectangle(i, j, self.baseGrid[i][j], index)
                index += 1
        
    # Complexity : steps * (n*m)^3
    def solve(self, steps):
        for i in range(steps):
            self.solveStep()

    def solveMax(self):
        isSolved = False
        numberOfSteps = 0
        while not isSolved:
            gridCopy = [[0 for i in range(self.width)] for j in range(self.height)]
            for i in range(self.height):
                for j in range(self.width):
                    gridCopy[i][j] = self.grid[i][j]
            self.solveStep()
            numberOfSteps += 1
            areEquals = True
            for i in range(self.height):
                for j in range(self.width):
                    if gridCopy[i][j] != self.grid[i][j]:
                        areEquals = False
                        break
                if not areEquals:
                    break
            isSolved = areEquals
        return numberOfSteps
    
    
    # Complexity : (n*m)^3
    def solveStep(self):
        # p : number of rectangles
        for rectangleA in self.rectangleCollection: # NOTE : Complexity : p
            for rectangleB in self.rectangleCollection: # NOTE : Complexity : p
                if rectangleA.equals(rectangleB): # NOTE : Complexity : n*m
                    continue
                if rectangleA.isGluableTo(rectangleB): # NOTE : Complexity : constant
                    rectangleA.merge(rectangleB, rectangleA.value, self.grid) # NOTE : Complexity : 5 * n*m
                    self.rectangleCollection.remove(rectangleB) # TODO : find the complexity
        
        # complexity : p^2 * (5)n*m => p^2 * n*m
        # p in worst case : n*m => (n*m)^2 * n*m => (n*m)^3

    
    
    
    
    # ---------------------------------------------------------------------------- #
    #                                     PRINT                                    #
    # ---------------------------------------------------------------------------- #
    
    def display(self):
        displayGrid(self.grid)
    def displayBase(self):
        displayGrid(self.baseGrid)