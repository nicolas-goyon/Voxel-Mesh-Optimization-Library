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
    
    def merge(self, other, newValues, into):
        for i in range(self.lengthX):
            for j in range(self.lengthY):
                into[self.offsetX + i][self.offsetY + j] = self.index
        for i in range(other.lengthX):
            for j in range(other.lengthY):
                into[other.offsetX + i][other.offsetY + j] = self.index
        
        self = self.rectangleFromCoordinates(into, self.flat[0][0], self.flat[0][1])
        self.value = newValues
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
        self.flat = []
        for i in range(n):
            for j in range(m):
                if grid[i][j] == self.index:
                    self.flat.append((i, j))
        maxX = 0
        maxY = 0
        minX = MAX_VALUE
        minY = MAX_VALUE
        matrix = [[0 for i in range(self.lengthY)] for j in range(self.lengthX)]
        
        for i in range(len(self.flat)):
            if self.flat[i][0] > maxX:
                maxX = self.flat[i][0]
            if self.flat[i][1] > maxY:
                maxY = self.flat[i][1]
            if self.flat[i][0] < minX:
                minX = self.flat[i][0]
            if self.flat[i][1] < minY:
                minY = self.flat[i][1]
                
            matrix[self.flat[i][0] - self.offsetY][self.flat[i][1] - self.offsetY] = 1
    
        self.offsetX = minX
        self.offsetY = minY
        self.lengthX = maxX - minX + 1
        self.lengthY = maxY - minY + 1
        self.maxX = maxX
        self.maxY = maxY
        
        
        # check if the matrix is a rectangle
        for i in range(len(matrix)):
            for j in range(len(matrix[i])):
                if matrix[i][j] == 0:
                    raise Exception("Not a rectangle : " + str(self.flat))
        
        return self 
    


        
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