from Rectangle import Rectangle

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
        

    def solve(self, steps):
        for i in range(steps):
            self.solveStep()

    
    
    def solveStep(self):
        for rectangleA in self.rectangleCollection:
            for rectangleB in self.rectangleCollection:
                if rectangleA.equals(rectangleB):
                    continue
                if rectangleA.isGluableTo(rectangleB):
                    rectangleA.merge(rectangleB, rectangleA.value, self.grid)
                    self.rectangleCollection.remove(rectangleB)
        

    
    
    
    
    # ---------------------------------------------------------------------------- #
    #                                     PRINT                                    #
    # ---------------------------------------------------------------------------- #
    
    def display(self):
        for i in range(self.height):
            for j in range(self.width):
                print(self.baseGrid[i][j], end=" ")
            print()
    
    def displayRectangleIndex(self):
        
        maxIndexLength = len(str(len(self.rectangleCollection))) +1
        
        for i in range(self.height):
            for j in range(self.width):
                # print(self.grid[i][j], end=" ")
                print(str(self.grid[i][j]).ljust(maxIndexLength), end=" ")
            print()
            
            
    def printRectangles(self):
        for rectangle in self.rectangleCollection:
            print("Rectangle ", rectangle.value)
            print("OffsetX ", rectangle.offsetX)
            print("OffsetY ", rectangle.offsetY)
            print("LengthX ", rectangle.lengthX)
            print("LengthY ", rectangle.lengthY)
            print("MaxX ", rectangle.maxX)
            print("MaxY ", rectangle.maxY)
            print("-------")
            
    def printRectanglesCompact(self):
        for rectangle in self.rectangleCollection:
            print("Rectangle ", rectangle.value)
        