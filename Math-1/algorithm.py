from base import *
import sys

TOP = "top"
BOTTOM = "bottom"
LEFT = "left"
RIGHT = "right"

MAX_VALUE = sys.maxsize


def fillArray(n, m):
    arr = [[0 for i in range(m)] for j in range(n)]
    index = 0
    for i in range(n):
        for j in range(m):
            arr[i][j] = index
            index += 1
    return arr

test = fillArray(3, 3)

def algorithm(baseArr, steps):
    n = len(baseArr)
    m = len(baseArr[0])
    mathArray = [n, m]
    mathArray = fillArray(n, m) # Array of cosecutive numbers
    for i in range(steps):
        algorithmStep(baseArr, mathArray)
        print("Step ", i + 1)
        display_array(mathArray)
        print("================")
        print("================")

def algorithmStep(baseArr, mathArray):
    n = len(baseArr)
    m = len(baseArr[0])
    changeDone = False
    
    for i in range(n):
        for j in range(m):
            if (changeDone):
                changeDone = False
                continue
            if (i == 0 and j == 0) or (i == n - 1 and j == m - 1): # Skip the first and last cell
                continue
            
            if j + 1 < m and baseArr[i][j] == baseArr[i][j + 1] and areCellsFromDifferentRectangles(mathArray, i, j, i, j + 1):
                if (areTouchingSideGluable(mathArray, i, j, i, j + 1)):
                    Glue(mathArray, i, j, i, j + 1, mathArray[i][j])
                    changeDone = True
                    display_array(mathArray)
                    print("================")

def areCellsFromDifferentRectangles(mathArray, xA, yA, xB, yB):
    rectangleA = getFlatRectangleFromCoordinates(mathArray, xA, yA)
    rectangleB = getFlatRectangleFromCoordinates(mathArray, xB, yB)
    
    for i in range(len(rectangleA)):
        for j in range(len(rectangleB)):
            if rectangleA[i] == rectangleB[j]:
                return False
    return True

def Glue(mathArray, xA, yA, xB, yB, newValues):
    rectangleA = getFlatRectangleFromCoordinates(mathArray, xA, yA)
    rectangleB = getFlatRectangleFromCoordinates(mathArray, xB, yB)
    
    for i in range(len(rectangleA)):
        mathArray[rectangleA[i][0]][rectangleA[i][1]] = newValues
        
    for i in range(len(rectangleB)):
        mathArray[rectangleB[i][0]][rectangleB[i][1]] = newValues
    


def areTouching(xA, yA, xB, yB):
    if xA == xB:
        return yA == yB + 1 or yA == yB - 1
    if yA == yB:
        return xA == xB + 1 or xA == xB - 1
    return False

def areTouchingSideGluable(mathArr, xA, yA, xB, yB):
    sides = getGluableSideList(mathArr, xA, yA, xB, yB)
    
    sideA = sides["sideA"]
    sideB = sides["sideB"]
    
    for i in range(len(sideA)):
        isTouchingOne = False
        for j in range(len(sideB)):
            if areTouching(sideA[i][0], sideA[i][1], sideB[j][0], sideB[j][1]):
                isTouchingOne = True
                break
        if not isTouchingOne:
            return False
    if len(sideA) != len(sideB):
        return False
    return True
    
def getGluableSideList(mathArr, xA, yA, xB, yB):
    # for a and for b, check which side is touching the other
    sideOfA = ""
    sideOfB = ""
    # if A is on the top of B : A is touching the bottom of B
    if xA < xB:
        sideOfA = BOTTOM
        sideOfB = TOP
    elif xA > xB:
        sideOfA = TOP
        sideOfB = BOTTOM
    elif yA < yB:
        sideOfA = RIGHT
        sideOfB = LEFT
    elif yA > yB:
        sideOfA = LEFT
        sideOfB = RIGHT
    
    listSideA = getListFromSide(mathArr, xA, yA, sideOfA)
    listSideB = getListFromSide(mathArr, xB, yB, sideOfB)
    
    res = {
        "sideA" : listSideA,
        "sideB" : listSideB,
    }
    
    return res
    
def getListFromSide(mathArr, x, y, side): # ex: 1, 1, TOP -> all top elements of the rectangle (with offset x, y)
    matrix = getRectangleMatrix(mathArr, x, y)
    dimensions = getMatrixDimensionsFromCoordinates(mathArr, x, y)
    
    if matrix == None:
        return None
    n = len(matrix)
    m = len(matrix[0])
    
    offsetX = dimensions["offsetX"]
    offsetY = dimensions["offsetY"]
    
    if side == TOP:
        return [(offsetX, i + offsetY) for i in range(m)]
    if side == BOTTOM:
        return [(offsetX + n - 1, i + offsetY) for i in range(m)]
    if side == LEFT:
        return [(i + offsetX, offsetY) for i in range(n)]
    if side == RIGHT:
        return [(i + offsetX, offsetY + m - 1) for i in range(n)]
    
    return None

def getRectangleMatrix(mathArr, x, y):
    rectangle = getFlatRectangleFromCoordinates(mathArr, x, y)
    
    # create a matrix from the input
    
    matrix = fromTuplesToMatrix(rectangle)
    # check if the matrix is a rectangle
    for i in range(len(matrix)):
        for j in range(len(matrix[i])):
            if matrix[i][j] == 0:
                print("Not a rectangle")
                return None
    
    matrix = fromTuplesToMatrix(rectangle)
    return matrix
    
    
def sideOfElementInRectangle(mathArr, x, y): # Check if (x,y) is on the top, bottom, left or right of its rectangle
    rectangle = getFlatRectangleFromCoordinates(mathArr, x, y)
    # check if rectangle is a rectangle
    matrix = fromTuplesToMatrix(rectangle)
    if matrix == None:
        print("Not a rectangle")
        return None
    sides = [] # TOP, BOTTOM, LEFT, RIGHT

    # check if the element is on the top of the rectangle
    dimensions = getMatrixDimensions(rectangle)
    minX = dimensions["offsetX"]
    minY = dimensions["offsetY"]
    maxX = dimensions["maxX"]
    maxY = dimensions["maxY"]
    if x == minX:
        sides.append(TOP)
    if x == maxX:
        sides.append(BOTTOM)
    if y == minY:
        sides.append(LEFT)
    if y == maxY:
        sides.append(RIGHT)
        
    return sides
    

def getMatrixDimensions(input):
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

def getMatrixDimensionsFromCoordinates(mathArr,x, y):
    rectangle = getFlatRectangleFromCoordinates(mathArr, x, y)
    
    return getMatrixDimensions(rectangle)

def getFlatRectangleFromCoordinates(mathArr, x, y):
    n = len(mathArr)
    m = len(mathArr[0])
    
    # get the number of the element
    number = mathArr[x][y]
    
    # get the rectangle of number
    rectangle = []
    for i in range(n):
        for j in range(m):
            if mathArr[i][j] == number:
                rectangle.append((i, j))
    
    return rectangle

def fromTuplesToMatrix(input):
    # get the dimensions of the matrix
    dimensions = getMatrixDimensions(input)
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

testArr = [
    [1, 1, 1, 1],
    [0, 1, 1, 0],
    [0, 0, 0, 0],
    [1, 1, 1, 1],
]
    
    
display_array(testArr)
print("-----------------")
algorithm(testArr, 5)