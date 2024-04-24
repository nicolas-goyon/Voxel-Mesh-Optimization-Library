from generalUtils import *

class Rectangle: 
    offsetX = 0
    offsetY = 0
    lengthX = 0
    lengthY = 0
    value = 0
    index = 0
    
    def __init__(self, x, y, value, index):
        self.offsetX = x
        self.offsetY = y
        self.value = value
        self.lengthX = 1
        self.lengthY = 1
        self.index = index
        
        
        
        
    # ---------------------------------------------------------------------------- #
    #                                    PUBLIC                                    #
    # ---------------------------------------------------------------------------- #

    # Complexity : constant
    def equals(self, other):
        if self.index == other.index: # NOTE : Complexity : constant
            return True
        if (self.offsetX != other.offsetX or 
            self.offsetY != other.offsetY or 
            self.lengthX != other.lengthX or 
            self.lengthY != other.lengthY or 
            self.value != other.value): # NOTE : Complexity : constant
            return False
        return True
    
    
    # Complexity : constant
    def merge(self, other, newIndex):
        mergeAxis = None
        
        if self.offsetX == other.offsetX:
            mergeAxis = "y"
        if self.offsetY == other.offsetY:
            mergeAxis = "x"
        
        self.index = newIndex
        
        self.offsetX = min(self.offsetX, other.offsetX)
        self.offsetY = min(self.offsetY, other.offsetY)
        
        if mergeAxis == "x":
            self.lengthX += other.lengthX
            self.lengthY = max(self.lengthY, other.lengthY)
        else :
            self.lengthY += other.lengthY
            self.lengthX = max(self.lengthX, other.lengthX)
        
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