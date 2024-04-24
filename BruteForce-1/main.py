from base import *
from Grid import *
from Rectangle import *
from generalUtils import *
import time

# ---------------------------------------------------------------------------- #
#                                     MAIN                                     #
# ---------------------------------------------------------------------------- #


testArr = [
    [1, 1, 1, 1],
    [0, 1, 1, 0],
    [0, 0, 0, 0],
    [1, 1, 1, 1],
]

big = big_lines

testGrid = SolveGrid(big)
startTime = time.time()
display_array(big)
print("-----------------")
# testGrid.solve(5)
numberOfSteps = testGrid.solveMax()
testGrid.display()
endTime = time.time()
print("Number of steps : ", numberOfSteps)
print("Time : ", endTime - startTime)
