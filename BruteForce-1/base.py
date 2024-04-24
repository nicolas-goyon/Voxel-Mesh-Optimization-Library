def display_array(arr):
    for i in range(len(arr)):
        for j in range(len(arr[i])):
            print(arr[i][j], end=" ")
        print()


# 8x10
one = [
    [1, 1, 1, 1, 1, 1, 1, 1, 1, 1],
    [1, 1, 1, 1, 1, 1, 1, 1, 1, 1],
    [1, 1, 1, 1, 1, 1, 1, 1, 1, 1],
    [1, 1, 1, 1, 1, 1, 1, 1, 1, 1],
    [1, 1, 1, 1, 1, 1, 1, 1, 1, 1],
    [1, 1, 1, 1, 1, 1, 1, 1, 1, 1],
    [1, 1, 1, 1, 1, 1, 1, 1, 1, 1],
    [1, 1, 1, 1, 1, 1, 1, 1, 1, 1],
]
# 8x10
lines = [
    [1, 1, 1, 1, 1, 1, 1, 1, 1, 1],
    [2, 2, 2, 2, 2, 2, 2, 2, 2, 2],
    [1, 1, 1, 1, 1, 1, 1, 1, 1, 1],
    [2, 2, 2, 2, 2, 2, 2, 2, 2, 2],
    [1, 1, 1, 1, 1, 1, 1, 1, 1, 1],
    [2, 2, 2, 2, 2, 2, 2, 2, 2, 2],
    [1, 1, 1, 1, 1, 1, 1, 1, 1, 1],
    [2, 2, 2, 2, 2, 2, 2, 2, 2, 2],
]

# 100x100
big_lines = []

for i in range(50):
    big_lines.append([1 for j in range(100)])
    big_lines.append([2 for j in range(100)]) 

# 4x4
echec_table = [
    [1, 2, 1, 2],
    [2, 1, 2, 1],
    [1, 2, 1, 2],
    [2, 1, 2, 1],
]


result_one = [
    {
        "number" : 1,
        "start" : (0, 0),
        "end" : (9, 9),
    }
]

result_lines = [
    {
        "number" : 1,
        "start" : (0, 0),
        "end" : (0, 9),
    },
    {
        "number" : 2,
        "start" : (1, 0),
        "end" : (1, 9),
    },
    {
        "number" : 1,
        "start" : (2, 0),
        "end" : (2, 9),
    },
    {
        "number" : 2,
        "start" : (3, 0),
        "end" : (3, 9),
    },
    {
        "number" : 1,
        "start" : (4, 0),
        "end" : (4, 9),
    },
    {
        "number" : 2,
        "start" : (5, 0),
        "end" : (5, 9),
    },
    {
        "number" : 1,
        "start" : (6, 0),
        "end" : (6, 9),
    },
    {
        "number" : 2,
        "start" : (7, 0),
        "end" : (7, 9),
    },
]
    
result_echec_table = [
    {
        "number" : 1,
        "start" : (0, 0),
        "end" : (0, 0),
    },
    {
        "number" : 2,
        "start" : (1, 0),
        "end" : (1, 0),
    },
    {
        "number" : 1,
        "start" : (2, 0),
        "end" : (2, 0),
    },
    {
        "number" : 2,
        "start" : (3, 0),
        "end" : (3, 0),
    },
    {
        "number" : 2,
        "start" : (0, 1),
        "end" : (0, 1),
    },
    {
        "number" : 1,
        "start" : (1, 1),
        "end" : (1, 1),
    },
    {
        "number" : 2,
        "start" : (2, 1),
        "end" : (2, 1),
    },
    {
        "number" : 1,
        "start" : (3, 1),
        "end" : (3, 1),
    },
    {
        "number" : 1,
        "start" : (0, 2),
        "end" : (0, 2),
    },
    {
        "number" : 2,
        "start" : (1, 2),
        "end" : (1, 2),
    },
    {
        "number" : 1,
        "start" : (2, 2),
        "end" : (2, 2),
    },
    {
        "number" : 2,
        "start" : (3, 2),
        "end" : (3, 2),
    },
    {
        "number" : 2,
        "start" : (0, 3),
        "end" : (0, 3),
    },
    {
        "number" : 1,
        "start" : (1, 3),
        "end" : (1, 3),
    },
    {
        "number" : 2,
        "start" : (2, 3),
        "end" : (2, 3),
    },
    {
        "number" : 1,
        "start" : (3, 3),
        "end" : (3, 3),
    },
]
