using System.Collections;
using System.Collections.Generic;

public class MeshOptimisation(int[,] pixels) {


    private Grid grid = new(pixels);

    public void Optimize() {
        grid.SolveMax();
    }

    public void Display() {
        Console.WriteLine(grid.Display());
    }

}

public class Rectangle {
    private enum TouchingSide {
        TOP,
        BOTTOM,
        LEFT,
        RIGHT
    }

    private (int X, int Y) offset;
    private (int X, int Y) length;
    private readonly int value;
    public int Index { get; private set;}

    public Rectangle(int x, int y, int value, int index) {
        offset = (x, y);
        length = (1, 1);
        this.value = value;
        this.Index = index;
    }

    public bool Equals(Rectangle other) {
        if (Index == other.Index) {
            return true;
        }
        if (offset != other.offset || length != other.length || value != other.value) {
            return false;
        }
        return true;
    }

    public Rectangle Merge(Rectangle other, int newIndex) {
        string mergeAxis = null;

        if (offset.X == other.offset.X) {
            mergeAxis = "y";
        }
        if (offset.Y == other.offset.Y) {
            mergeAxis = "x";
        }

        Index = newIndex;

        offset.X = Math.Min(offset.X, other.offset.X);
        offset.Y = Math.Min(offset.Y, other.offset.Y);

        if (mergeAxis == "x") {
            length.X += other.length.X;
            length.Y = Math.Max(length.Y, other.length.Y);
        } else {
            length.Y += other.length.Y;
            length.X = Math.Max(length.X, other.length.X);
        }

        return this;
    }

    public bool IsTouching(Rectangle other) {
        try {
            GetTouchingSide(other);
            return true;
        } catch (System.Exception) {
            return false;
        }
    }

    public bool IsGluableTo(Rectangle other) {
        if (!IsTouching(other)) {
            return false;
        }

        if (!IsCompatibleTo(other)) {
            return false;
        }

        TouchingSide touchingSide = GetTouchingSide(other);

        int selfTouchingSideLength = 0;
        int otherTouchingSideLength = 0;

        switch (touchingSide) {
            case TouchingSide.TOP:
            case TouchingSide.BOTTOM:
                selfTouchingSideLength = length.Y;
                otherTouchingSideLength = other.length.Y;
                break;
            case TouchingSide.LEFT:
            case TouchingSide.RIGHT:
                selfTouchingSideLength = length.X;
                otherTouchingSideLength = other.length.X;
                break;
        }

        return selfTouchingSideLength == otherTouchingSideLength;
    }

    public bool IsCompatibleTo(Rectangle other) {
        return value == other.value;
    }

    public int[,] FillIndex(int[,] grid) {
        for (int i = 0; i < length.X; i++) {
            for (int j = 0; j < length.Y; j++) {
                grid[offset.Y + i, offset.Y + j] = Index;
            }
        }
        return grid;
    }

    private TouchingSide GetTouchingSide(Rectangle other) {
        if (offset.X == other.offset.X) {
            if (offset.Y == other.offset.Y + other.length.Y) {
                return TouchingSide.RIGHT;
            }
            if (offset.Y + length.Y == other.offset.Y   ) {
                return TouchingSide.LEFT;
            }
        }
        if (offset.Y == other.offset.Y) {
            if (offset.X == other.offset.X + other.length.X) {
                return TouchingSide.BOTTOM;
            }
            if (offset.X + length.X == other.offset.X) {
                return TouchingSide.TOP;
            }
        }
        throw new System.Exception("No touching side found");
    }

    public override string ToString() {
        return $"Rectangle(offset: {offset}, length: {length}, value: {value}, index: {Index})";
    }

    public (int X, int Y) Offset {
        get {
            return offset;
        }
    }

    public (int X, int Y) Length {
        get {
            return length;
        }
    }
    
}
public struct Grid {
    private readonly int width;
    private readonly int height;
    private Rectangle[,] rectangleCollection;
    private readonly Queue<int> searchQueue;

    public Grid(int[,] baseGrid) {
        height = baseGrid.GetLength(0);
        width = baseGrid.GetLength(1);
        rectangleCollection = new Rectangle[height, width];
        int index = 0;
        for (int i = 0; i < height; i++) {
            for (int j = 0; j < width; j++) {
                rectangleCollection[i, j] = new Rectangle(i, j, baseGrid[i, j], index);
                index++;
            }
        }
        searchQueue = new Queue<int>();
        
    }

    public bool SolveMax() {
        int index = 0;
        for (int i = 0; i < height; i++) {
            for (int j = 0; j < width; j++) {
                AddQueue(rectangleCollection[i, j]);
                index++;
            }
        }
        while (searchQueue.Count > 0) { 

            int rectangleIndex = GetQueue();
            Rectangle rectangle = rectangleCollection[rectangleIndex / width, rectangleIndex % width];
            if (rectangle.Index != rectangleIndex) {
                continue;
            }

            bool hasChanged1 = TryRight(rectangle);
            rectangle = rectangleCollection[rectangleIndex / width, rectangleIndex % width];

            bool hasChanged2 = TryBottom(rectangle);
            if (hasChanged1 || hasChanged2) {
                AddQueue(rectangle);
            }
        }
        return true;
    }

    private bool TryRight(Rectangle rectangle) {
        int baseX = rectangle.Offset.X;
        int baseY = rectangle.Offset.Y;
        int rightRectangleX = baseX;
        int rightRectangleY = baseY + rectangle.Length.Y;
        if (rightRectangleY >= width) {
            return false;
        }

        Rectangle rightRectangle = rectangleCollection[rightRectangleX, rightRectangleY];

        if (TryMerge(rectangle, rightRectangle)) {
            return true;
        } else {
            AddQueue(rightRectangle);
            return false;
        }


    }

    private bool TryBottom(Rectangle rectangle) {
        int baseX = rectangle.Offset.X;
        int baseY = rectangle.Offset.Y;
        int bottomRectangleX = baseX + rectangle.Length.X;
        int bottomRectangleY = baseY;

        if (bottomRectangleX >= height) {
            return false;
        }

        Rectangle bottomRectangle = rectangleCollection[bottomRectangleX, bottomRectangleY];

        if (TryMerge(rectangle, bottomRectangle)) {
            return true;
        } else {
            AddQueue(bottomRectangle);
            return false;
        }
    }

    private bool TryMerge(Rectangle rectangle, Rectangle other) {


        if (rectangle.IsGluableTo(other)) {
            Rectangle newRectangle = rectangle.Merge(other, rectangle.Index);
            UpdateValues(newRectangle);
            return true;
        }
        return false;
    }

    private void UpdateValues(Rectangle newRectangle) {
        int offsetX = newRectangle.Offset.X;
        int offsetY = newRectangle.Offset.Y;
        int lengthX = newRectangle.Length.X;
        int lengthY = newRectangle.Length.Y;
        for (int i = offsetX; i < offsetX + lengthX; i++) {
            for (int j = offsetY; j < offsetY + lengthY; j++) {
                this.rectangleCollection[i, j] = newRectangle;
            }
        }


    }




    private bool AddQueue(Rectangle rectangle) {
        if (searchQueue.Contains(rectangle.Index)) {
            return false;
        }
        searchQueue.Enqueue(rectangle.Index);
        return true;
    }

    private int GetQueue() {
        return searchQueue.Dequeue();
    }

    public string Display() {
        string res = "";
        for (int i = 0; i < height; i++) {
            for (int j = 0; j < width; j++) {
                res += rectangleCollection[i, j].Index + " ";
            }
            res += "\n";
        }
        return res;
    }

}