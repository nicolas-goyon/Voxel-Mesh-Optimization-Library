from Rectangle import Rectangle
from generalUtils import displayGrid
import queue

class SolveGrid :
    width = 0
    height = 0
    rectangleCollection = [] # [Rectangle, Rectangle, ...]
    baseGrid = [] # [[...], [...], [...], ...]
    elementsInQueue = []
    
            
    def __init__(self, baseGrid):
        self.height = len(baseGrid)
        self.width = len(baseGrid[0])
        self.baseGrid = baseGrid
        self.rectangleCollection = [[Rectangle(0, 0, 0, 0) for i in range(self.width)] for j in range(self.height)]
        index = 0
        for i in range(self.height):
            for j in range(self.width):
                self.rectangleCollection[i][j] = Rectangle(i, j, self.baseGrid[i][j], index)
                index += 1
        
    # Complexity : 
    def solveMax(self):
        searchQueue = queue.Queue()
        index = 0
        for i in range(self.height):
            for j in range(self.width):
                self.addToQueue(self.rectangleCollection[i][j], searchQueue)
                index += 1
        while not searchQueue.empty(): # 
            # Complexity : n*m each time
            hasChanged = False
            rectangleIndex = self.getQueue(searchQueue) # NOTE : Complexity : n*m
            
            rectangle = self.rectangleCollection[rectangleIndex // self.width][rectangleIndex % self.width]
            
            if rectangle.index != rectangleIndex:
                continue
            
            hasChanged1 = self.tryRight(rectangle, searchQueue) # NOTE : Complexity : n*m
            hasChanged2 = self.tryBottom(rectangle, searchQueue) # NOTE : Complexity : n*m
            hasChanged = hasChanged1 or hasChanged2
            if hasChanged:
                self.addToQueue(rectangle, searchQueue) # NOTE : Complexity : n*m
        return True

    def tryRight(self, rectangle, searchQueue): # NOTE : Complexity : n*m
        baseX = rectangle.offsetX
        baseY = rectangle.offsetY
        rightRectangleX = baseX
        rightRectangleY = baseY + rectangle.lengthY
        
        if rightRectangleY >= self.width:
            return False
        
        
        rightRectangle = self.rectangleCollection[rightRectangleX][rightRectangleY]
        
        
        if self.tryMerge(rectangle, rightRectangle): # NOTE : Complexity : n*m
            return True
        else:
            res = self.addToQueue(rightRectangle, searchQueue) # NOTE : Complexity : n*m
            if res :
                print("Error")
            return False
    
    def tryBottom(self, rectangle, searchQueue): # NOTE : Complexity : n*m
        baseX = rectangle.offsetX
        baseY = rectangle.offsetY
        bottomRectangleX = baseX + rectangle.lengthX
        bottomRectangleY = baseY
        
        if bottomRectangleX >= self.height:
            return False
        
        
        bottomRectangle = self.rectangleCollection[bottomRectangleX][bottomRectangleY]
        
        
        if self.tryMerge(rectangle, bottomRectangle): # NOTE : Complexity : n*m
            return True
        else:
            res = self.addToQueue(bottomRectangle, searchQueue) # NOTE : Complexity : n*m
            if res :
                print("Error")
            return False
        

    def tryMerge(self, rectangle: Rectangle, other: Rectangle): # NOTE : Complexity : n*m
        if rectangle.isGluableTo(other): # NOTE : Complexity : constant
            self.updateValues(other, rectangle) # NOTE : Complexity : n*m
            rectangle.merge(other, rectangle.index) # NOTE : Complexity : constant
            return True
        return False
    
    def updateValues(self, odlRectangle: Rectangle, newRectangle: Rectangle): # NOTE : Complexity : n*m
        offsetX = odlRectangle.offsetX
        offsetY = odlRectangle.offsetY
        lengthX = odlRectangle.lengthX
        lengthY = odlRectangle.lengthY
        for i in range(offsetX, offsetX + lengthX): 
            for j in range(offsetY, offsetY + lengthY):
                self.rectangleCollection[i][j] = newRectangle
                
    def addToQueue(self, rectangle, searchQueue: queue.Queue): # NOTE : Complexity : n*m 
        if self.elementsInQueue.count(rectangle.index) > 0: # NOTE : Complexity : n*m
            return False
        self.elementsInQueue.append(rectangle.index) # NOTE : Complexity : constant
        searchQueue.put(rectangle.index) # NOTE : Complexity : constant
        return True
    
    def getQueue(self, searchQueue: queue.Queue): # NOTE : Complexity : n*m
        element =  searchQueue.get() # NOTE : Complexity : constant
        self.elementsInQueue.remove(element) # NOTE : Complexity : n*m
        return element
    
    # ---------------------------------------------------------------------------- #
    #                                     PRINT                                    #
    # ---------------------------------------------------------------------------- #
    
    def display(self):
        grid = [[-1 for i in range(self.width)] for j in range(self.height)]
        for i in range(self.height):
            for j in range(self.width):
                grid[i][j] = self.rectangleCollection[i][j].index
        displayGrid(grid)
    def displayBase(self):
        displayGrid(self.baseGrid)