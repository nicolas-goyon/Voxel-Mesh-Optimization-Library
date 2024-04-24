from base import *
from Grid import *
from Rectangle import *
from generalUtils import *

# ---------------------------------------------------------------------------- #
#                                     MAIN                                     #
# ---------------------------------------------------------------------------- #


# testArr = [
#     [1, 1, 1, 1],
#     [0, 1, 1, 0],
#     [0, 0, 0, 0],
#     [1, 1, 1, 1],
# ]

testGrid = SolveGrid(echec_table)
    
display_array(echec_table)
print("-----------------")
# testGrid.solve(5)
numberOfSteps = testGrid.solveMax()
testGrid.display()
print("Number of steps : ", numberOfSteps)
