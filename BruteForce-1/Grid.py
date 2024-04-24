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
        # self.rectangleCollection = [Rectangle(0, 0, 0, 0) for i in range(self.height * self.width)]
        self.rectangleCollection = [[Rectangle(0, 0, 0, 0) for i in range(self.width)] for j in range(self.height)]
        index = 0
        for i in range(self.height):
            for j in range(self.width):
                self.rectangleCollection[i][j] = Rectangle(i, j, self.baseGrid[i][j], index)
                index += 1
        
    # Complexity : steps * (n*m)^3
    def solve(self, steps):
        for i in range(steps):
            self.solveStep()

    def solveMax(self):
        # hasChanged = True
        # numberOfSteps = 0
        # while hasChanged:
            hasChanged = self.solveStep()
        #     numberOfSteps += 1
        # return numberOfSteps
    
    
    # Complexity : 
    def solveStep(self):
        searchQueue = queue.Queue()
        index = 0
        for i in range(self.height):
            for j in range(self.width):
                self.addToQueue(self.rectangleCollection[i][j], searchQueue)
                index += 1
        maxCount = 100
        while not searchQueue.empty() and maxCount > 0:
            maxCount -= 1
            hasChanged = False
            rectangleIndex = self.getQueue(searchQueue)
            
            rectangle = self.rectangleCollection[rectangleIndex // self.width][rectangleIndex % self.width]
            
            if rectangle.index != rectangleIndex:
                continue
            
            # print("=========")
            # self.display()
            # print("Current rectangle : ", rectangle.index)
            
            hasChanged1 = self.tryRight(rectangle, searchQueue)
            # if hasChanged:
            #     print("merged right")
            self.display()
            hasChanged2 = self.tryBottom(rectangle, searchQueue)
            # if hasChanged:
            #     print("merged bottom")
            
            hasChanged = hasChanged1 or hasChanged2
            if hasChanged:
                self.addToQueue(rectangle, searchQueue)
            
            # print("=========")
            
        return True

    def tryRight(self, rectangle, searchQueue):
        baseX = rectangle.offsetX
        baseY = rectangle.offsetY
        rightRectangleX = baseX
        rightRectangleY = baseY + rectangle.lengthY
        
        if rightRectangleY >= self.width:
            return False
        
        
        rightRectangle = self.rectangleCollection[rightRectangleX][rightRectangleY]
        
        
        if self.tryMerge(rectangle, rightRectangle):
            return True
        else:
            self.addToQueue(rightRectangle, searchQueue)
            return False
    
    def tryBottom(self, rectangle, searchQueue):
        baseX = rectangle.offsetX
        baseY = rectangle.offsetY
        bottomRectangleX = baseX + rectangle.lengthX
        bottomRectangleY = baseY
        
        if bottomRectangleX >= self.height:
            return False
        
        
        bottomRectangle = self.rectangleCollection[bottomRectangleX][bottomRectangleY]
        
        
        if self.tryMerge(rectangle, bottomRectangle):
            return True
        else:
            self.addToQueue(bottomRectangle, searchQueue)
            return False
        

    def tryMerge(self, rectangle, other):
        
        if rectangle.isGluableTo(other):
            self.updateValues(other, rectangle)
            rectangle.merge(other, rectangle.index)
            return True
        return False
    
    def updateValues(self, odlRectangle, newRectangle):
        offsetX = odlRectangle.offsetX
        offsetY = odlRectangle.offsetY
        lengthX = odlRectangle.lengthX
        lengthY = odlRectangle.lengthY
        for i in range(offsetX, offsetX + lengthX):
            for j in range(offsetY, offsetY + lengthY):
                self.rectangleCollection[i][j] = newRectangle
                
    def addToQueue(self, rectangle, searchQueue: queue.Queue):
        if self.elementsInQueue.count(rectangle.index) > 0:
            return
        self.elementsInQueue.append(rectangle.index)
        searchQueue.put(rectangle.index)
    
    def getQueue(self, searchQueue: queue.Queue):
        element =  searchQueue.get()
        self.elementsInQueue.remove(element)
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