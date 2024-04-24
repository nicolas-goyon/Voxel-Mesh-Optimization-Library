from base import *
from Grid import *
from Rectangle import *
from generalUtils import *

# ---------------------------------------------------------------------------- #
#                                     MAIN                                     #
# ---------------------------------------------------------------------------- #


testArr = [
    [1, 1, 1, 1],
    [0, 1, 1, 0],
    [0, 0, 0, 0],
    [1, 1, 1, 1],
]

testGrid = SolveGrid(testArr)
    
display_array(testArr)
print("-----------------")
testGrid.solve(5)

testGrid.displayRectangleIndex()
