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

    # Complexity : n*m
    def equals(self, other):
        if self.index == other.index: # NOTE : Complexity : constant
            return True
        if len(self.flat) != len(other.flat): # NOTE : Complexity : constant
            return False
        for i in range(len(self.flat)): # NOTE : Complexity : n*m
            if self.flat[i] != other.flat[i]:
                return False
        return True
    
    
    # Complexity : 5 * n*m
    def merge(self, other, newValues):
        
        mergeAxis = None
        
        if self.offsetX == other.offsetX:
            mergeAxis = "y"
        if self.offsetY == other.offsetY:
            mergeAxis = "x"
        
        self.value = newValues
        self.flat = self.flat + other.flat # NOTE : Complexity : n*m
        
        
        self.offsetX = min(self.offsetX, other.offsetX)
        self.offsetY = min(self.offsetY, other.offsetY)
        
        self.lengthX = (mergeAxis == "x") * (self.lengthX + other.lengthX) + (mergeAxis != "x") * self.lengthX
        self.lengthY = (mergeAxis == "y") * (self.lengthY + other.lengthY) + (mergeAxis != "y") * self.lengthY
        self.maxX = self.offsetX + self.lengthX
        self.maxY = self.offsetY + self.lengthY
        
        
        self.matrix = [[0 for i in range(self.lengthY)] for j in range(self.lengthX)]
        for i in range(len(self.flat)):
            cell = self.flat[i]
            xPos = cell[0] - self.offsetX
            yPos = cell[1] - self.offsetY
            self.matrix[xPos][yPos] = 1
            
        
        
        return self
    
    # Complexity : constant
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
    
    # Complexity : constant
    def isGluableTo(self, other):
        if not self.isTouching(other): # NOTE : Complexity : constant
            return False
        
        if not self.isCompatibleTo(other): # NOTE : Complexity : constant
            return False
        

        touchingSide = self.getTouchingSide(other) # NOTE : Complexity : constant
        
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
            
    # Complexity : constant
    def isCompatibleTo(self, other):
        return self.value == other.value
    
    
    # Complexity : n*m
    def fillIndex(self, grid): 
        for i in range(self.lengthX):
            for j in range(self.lengthY):
                grid[self.offsetX + i][self.offsetY + j] = self.index
        return grid
    
    
    # ---------------------------------------------------------------------------- #
    #                                    PRIVATE                                   #
    # ---------------------------------------------------------------------------- #
    

    # Complexity : constant
    def getTouchingSide(self, other):
        if self.offsetX == other.offsetX:
            if self.offsetY == other.offsetY + other.lengthY:
                return RIGHT
            if self.offsetY + self.lengthY == other.offsetY:
                return LEFT
        if self.offsetY == other.offsetY:
            if self.offsetX == other.offsetX + other.lengthX:
                return BOTTOM
            if self.offsetX + self.lengthX == other.offsetX:
                return TOP
        return None