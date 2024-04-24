from base import *
from Grid import *
from Rectangle import *
from generalUtils import *
import time

# ---------------------------------------------------------------------------- #
#                                     MAIN                                     #
# ---------------------------------------------------------------------------- #


# testArr = [
#     [1, 1, 1, 1],
#     [0, 1, 1, 0],
#     [0, 0, 0, 0],
#     [1, 1, 1, 1],
# ]

testGrid = SolveGrid(big_lines)
startTime = time.time()
display_array(big_lines)
print("-----------------")
# testGrid.solve(5)
numberOfSteps = testGrid.solveMax()
testGrid.display()
endTime = time.time()
print("Number of steps : ", numberOfSteps)
print("Time : ", endTime - startTime)
