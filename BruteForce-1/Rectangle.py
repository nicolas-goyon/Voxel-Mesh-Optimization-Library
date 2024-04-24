from generalUtils import *

class Rectangle: 
    flat = [] # [(x1, y1), (x2, y2), ...]
    matrix = [] # [[0, 0, 0], [0, 0, 0], ...]
    offsetX = 0
    offsetY = 0
    lengthX = 0
    lengthY = 0
    maxX = 0
    maxY = 0
    value = 0
    index = 0
    
    # def __init__(self, grid, x, y):
    #     self.rectangleFromCoordinates(grid, x, y)
        
    def __init__(self, x, y, value, index):
        self.offsetX = x
        self.offsetY = y
        self.value = value
        self.lengthX = 1
        self.lengthY = 1
        self.maxX = x
        self.maxY = y
        self.flat = [(x, y)]
        self.matrix = [[1]]
        self.index = index
        
        
        
        
    # ---------------------------------------------------------------------------- #
    #                                    PUBLIC                                    #
    # ---------------------------------------------------------------------------- #
        
    def equals(self, other):
        if self.index == other.index:
            return True
        if len(self.flat) != len(other.flat):
            return False
        for i in range(len(self.flat)):
            if self.flat[i] != other.flat[i]:
                return False
        return True
    
    def setValues(self, values, into):
        for i in range(self.lengthX):
            for j in range(self.lengthY):
                into[self.offsetX + i][self.offsetY + j] = values
                
    def merge(self, other, newValues, into):
        for i in range(self.lengthX):
            for j in range(self.lengthY):
                into[self.offsetX + i][self.offsetY + j] = self.index
        for i in range(other.lengthX):
            for j in range(other.lengthY):
                into[other.offsetX + i][other.offsetY + j] = self.index
        
        self = self.rectangleFromCoordinates(into, self.flat[0][0], self.flat[0][1])
        return self
    
    def isTouching(self, other):
        if self.offsetX == other.offsetX:
            if self.offsetY == other.offsetY + other.lengthY:
                return True
            if self.offsetY + self.lengthY == other.offsetY:
                return True
        if self.offsetY == other.offsetY:
            if self.offsetX == other.offsetX + other.lengthX:
                return True
            if self.offsetX + self.lengthX == other.offsetX:
                return True
        return False
    
    def isGluableTo(self, other):
        if not self.isTouching(other):
            return False
        
        if not self.isCompatibleTo(other):
            return False
        

        touchingSide = self.getTouchingSide(other)
        
        selfTouchingSideLength = 0
        otherTouchingSideLength = 0
        if touchingSide == TOP:
            selfTouchingSideLength = self.lengthY
            otherTouchingSideLength = other.lengthY
        if touchingSide == BOTTOM:
            selfTouchingSideLength = self.lengthY
            otherTouchingSideLength = other.lengthY
        if touchingSide == LEFT:
            selfTouchingSideLength = self.lengthX
            otherTouchingSideLength = other.lengthX
        if touchingSide == RIGHT:
            selfTouchingSideLength = self.lengthX
            otherTouchingSideLength = other.lengthX
        return selfTouchingSideLength == otherTouchingSideLength
            
    def isCompatibleTo(self, other):
        return self.value == other.value
    
    
    def fillIndex(self, grid):
        for i in range(self.lengthX):
            for j in range(self.lengthY):
                grid[self.offsetX + i][self.offsetY + j] = self.index
        return grid
    
    
    # ---------------------------------------------------------------------------- #
    #                                    PRIVATE                                   #
    # ---------------------------------------------------------------------------- #
    
    def rectangleFromCoordinates(self, grid, x, y):
        n = len(grid)
        m = len(grid[0])
        
        # get the number of the element
        self.index = grid[x][y]
        
        # get the rectangle of number
        rectangle = []
        for i in range(n):
            for j in range(m):
                if grid[i][j] == self.index:
                    rectangle.append((i, j))
    
        self.flat = rectangle
    
        # create a matrix from the input
        
        matrix = self.fromTuplesToMatrix(rectangle)
        # check if the matrix is a rectangle
        for i in range(len(matrix)):
            for j in range(len(matrix[i])):
                if matrix[i][j] == 0:
                    raise Exception("Not a rectangle : " + str(rectangle))
        
        self.matrix = self.fromTuplesToMatrix(rectangle)
        
        dimensions = self.getMatrixDimensions(rectangle)
        
        self.offsetX = dimensions["offsetX"]
        self.offsetY = dimensions["offsetY"]
        self.lengthX = dimensions["lengthX"]
        self.lengthY = dimensions["lengthY"]
        self.maxX = dimensions["maxX"]
        self.maxY = dimensions["maxY"]
        
        return self 
    

    # ---------------------------------------------------------------------------- #
    #                              fromTuplesToMatrix                              #
    # ---------------------------------------------------------------------------- #


    # input: [(1, 1), (1, 2), (2, 1), (2, 2)]
    # output: [
    #    [1, 1],
    #    [1, 1]
    # ]

    def fromTuplesToMatrix(self, input):
        # get the dimensions of the matrix
        dimensions = self.getMatrixDimensions(input)
        minX = dimensions["offsetX"]
        minY = dimensions["offsetY"]
        dimX = dimensions["lengthX"]
        dimY = dimensions["lengthY"]
        
        
        matrix = [[0 for i in range(dimY)] for j in range(dimX)]
        # put a 1 in the matrix for every element in the input
        offsetX = minX
        offsetY = minY
        
        for i in range(len(input)):
            matrix[input[i][0] - offsetX][input[i][1] - offsetY] = 1
        
        return matrix



    # ---------------------------------------------------------------------------- #
    #                              getMatrixDimensions                             #
    # ---------------------------------------------------------------------------- #


    # input: [
    #    [0, 1, 1, 0],
    #    [0, 1, 1, 0],
    #    [0, 0, 0, 0],
    #    [1, 1, 1, 1],
    # ]
    # x = 1
    # y = 1
    # output: {
    #    "offsetX" : 0,
    #    "offsetY" : 1,
    #    "lengthX" : 2,
    #    "lengthY" : 2,
    #    "maxX" : 3,
    #    "maxX" : 2,
    # }
    def getMatrixDimensions(self, input):
        maxX = 0
        maxY = 0
        minX = MAX_VALUE
        minY = MAX_VALUE
        
        for i in range(len(input)):
            if input[i][0] > maxX:
                maxX = input[i][0]
            if input[i][1] > maxY:
                maxY = input[i][1]
            if input[i][0] < minX:
                minX = input[i][0]
            if input[i][1] < minY:
                minY = input[i][1]
                
        res = {
            "offsetX" : minX,
            "offsetY" : minY,
            "lengthX" : maxX - minX + 1,
            "lengthY" : maxY - minY + 1,
            "maxX" : maxX,
            "maxY" : maxY,
        }
        
        return res

        
    def getTouchingSide(self, other):
        if self.offsetX == other.offsetX:
            if self.offsetY == other.offsetY + 1:
                return LEFT
            if self.offsetY == other.offsetY - 1:
                return RIGHT
        if self.offsetY == other.offsetY:
            if self.offsetX == other.offsetX + 1:
                return BOTTOM
            if self.offsetX == other.offsetX - 1:
                return TOP
        return None