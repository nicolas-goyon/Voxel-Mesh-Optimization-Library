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
 
def areTouching(xA, yA, xB, yB):
    if xA == xB:
        return yA == yB + 1 or yA == yB - 1
    if yA == yB:
        return xA == xB + 1 or xA == xB - 1
    return False


def displayGrid(grid):
    maxIndex = 0
    for i in range(len(grid)):
        maxIndex += len(grid[i])
    
    maxIndexLength = len(str(maxIndex)) + 1
            
    print("-----------------")
    for i in range(len(grid)):
        for j in range(len(grid[i])):
            print(str(grid[i][j]).ljust(maxIndexLength), end=" ")
        print()
    print("-----------------")