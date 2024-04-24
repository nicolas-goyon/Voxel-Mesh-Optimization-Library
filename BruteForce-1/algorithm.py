from base import *
import sys
from generalUtils import *

TOP = "top"
BOTTOM = "bottom"
LEFT = "left"
RIGHT = "right"

MAX_VALUE = sys.maxsize

# ---------------------------------------------------------------------------- #
#                                   algorithm                                  #
# ---------------------------------------------------------------------------- #

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



# ---------------------------------------------------------------------------- #
#                                 algorithmStep                                #
# ---------------------------------------------------------------------------- #

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



# ---------------------------------------------------------------------------- #
#                        areCellsFromDifferentRectangles                       #
# ---------------------------------------------------------------------------- #



# input: [
#    [0, 1, 1, 0],
#    [0, 1, 1, 0],
#    [0, 2, 2, 0],
#    [1, 1, 1, 1],
# ]
# xA = 1
# yA = 1
# xB = 2
# yB = 1
# output: True


# input: [
#    [0, 1, 1, 0],
#    [0, 1, 1, 0],
#    [0, 2, 2, 0],
#    [1, 1, 1, 1],
# ]
# xA = 1
# yA = 1
# xB = 1
# yB = 2
# output: False
def areCellsFromDifferentRectangles(mathArray, xA, yA, xB, yB):
    rectangleA = getFlatRectangleFromCoordinates(mathArray, xA, yA)
    rectangleB = getFlatRectangleFromCoordinates(mathArray, xB, yB)
    
    for i in range(len(rectangleA)):
        for j in range(len(rectangleB)):
            if rectangleA[i] == rectangleB[j]:
                return False
    return True


# ---------------------------------------------------------------------------- #
#                                     Glue                                     #
# ---------------------------------------------------------------------------- #


# input: [
#    [0, 1, 1, 0],
#    [0, 1, 1, 0],
#    [0, 2, 2, 0],
#    [1, 1, 1, 1],
# ]
# xA = 1
# yA = 1
# xB = 2
# yB = 1
# newValues = 3
# output: [
#    [0, 3, 3, 0],
#    [0, 3, 3, 0],
#    [0, 3, 3, 0],
#    [1, 1, 1, 1],
# ]


def Glue(mathArray, xA, yA, xB, yB, newValues):
    rectangleA = getFlatRectangleFromCoordinates(mathArray, xA, yA)
    rectangleB = getFlatRectangleFromCoordinates(mathArray, xB, yB)
    
    for i in range(len(rectangleA)):
        mathArray[rectangleA[i][0]][rectangleA[i][1]] = newValues
        
    for i in range(len(rectangleB)):
        mathArray[rectangleB[i][0]][rectangleB[i][1]] = newValues
    




# ---------------------------------------------------------------------------- #
#                            areTouchingSideGluable                            #
# ---------------------------------------------------------------------------- #

# input: [
#    [0, 1, 1, 0],
#    [0, 1, 1, 0],
#    [0, 2, 2, 0],
#    [1, 1, 1, 1],
# ]
# xA = 1
# yA = 1
# xB = 2
# yB = 1
# output: True
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



# ---------------------------------------------------------------------------- #
#                              getGluableSideList                              #
# ---------------------------------------------------------------------------- #

# input: [
#    [0, 1, 1, 0],
#    [0, 1, 1, 0],
#    [0, 2, 2, 0],
#    [1, 1, 1, 1],
# ]
# xA = 1
# yA = 1
# xB = 2
# yB = 1
# output: {
#    "sideA" : [(1, 1), (1, 2)],
#    "sideB" : [(2, 1), (2, 2)],
# }
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
    
    
    
    
# ---------------------------------------------------------------------------- #
#                                getListFromSide                               #
# ---------------------------------------------------------------------------- #

# input: [
#    [0, 1, 1, 0],
#    [0, 1, 1, 0],
#    [0, 0, 0, 0],
#    [1, 1, 1, 1],
# ]
# x = 1
# y = 1
# side = BOTTOM
# output: [(1, 1), (1, 2)]
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



# ---------------------------------------------------------------------------- #
#                              getRectangleMatrix                              #
# ---------------------------------------------------------------------------- #



# input: [
#    [0, 1, 1, 0],
#    [0, 1, 1, 0],
#    [0, 0, 0, 0],
#    [1, 1, 1, 1],
# ]
# x = 1
# y = 1
# output: [
#    [1, 1],
#    [1, 1]
# ]
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
    
    
# ---------------------------------------------------------------------------- #
#                           sideOfElementInRectangle                           #
# ---------------------------------------------------------------------------- #

# input: [
#    [0, 1, 1, 0],
#    [0, 1, 1, 0],
#    [0, 0, 0, 0],
#    [1, 1, 1, 1],
# ]
# x = 1
# y = 1
# output: [BOTTOM, LEFT]
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


# ---------------------------------------------------------------------------- #
#                      getMatrixDimensionsFromCoordinates                      #
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
def getMatrixDimensionsFromCoordinates(mathArr,x, y):
    rectangle = getFlatRectangleFromCoordinates(mathArr, x, y)
    
    return getMatrixDimensions(rectangle)


# ---------------------------------------------------------------------------- #
#                        getFlatRectangleFromCoordinates                       #
# ---------------------------------------------------------------------------- #

# input: [
#    [0, 1, 1, 0],
#    [0, 1, 1, 0],
#    [0, 0, 0, 0],
#    [1, 1, 1, 1],
# ]
# x = 1
# y = 1
# output: [(0, 1), (0, 2), (1, 1), (1, 2)]
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


# ---------------------------------------------------------------------------- #
#                              fromTuplesToMatrix                              #
# ---------------------------------------------------------------------------- #


# input: [(1, 1), (1, 2), (2, 1), (2, 2)]
# output: [
#    [1, 1],
#    [1, 1]
# ]

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




# ---------------------------------------------------------------------------- #
#                                     MAIN                                     #
# ---------------------------------------------------------------------------- #


testArr = [
    [1, 1, 1, 1],
    [0, 1, 1, 0],
    [0, 0, 0, 0],
    [1, 1, 1, 1],
]
    
    
display_array(testArr)
print("-----------------")
algorithm(testArr, 5)