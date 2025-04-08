using System.Text;

namespace VoxelMeshOptimizer.Core.OcclusionAlgorithms.Common
{
    /// <summary>
    /// Each 2D slice in a given iteration order, containing references to visible voxels.
    /// 
    /// - MajorAxis, MiddleAxis, MinorAxis: describe the iteration order used.
    /// - MinorAxis is considered the "slice" axis, i.e., whichever axis changes last.
    /// - SliceIndex is the index along the MinorAxis dimension.
    /// - Voxels is the 2D array of (width x height) or (depth x height), etc., 
    ///   depending on which axes form the plane.
    /// </summary>
    public class VisiblePlane
    {
        /// <summary>
        /// The axis iterated first in the triple-nested loop (outer loop).
        /// </summary>
        public Axis MajorAxis { get; }

        /// <summary>
        /// The axis iterated second (middle loop).
        /// </summary>
        public Axis MiddleAxis { get; }

        /// <summary>
        /// The axis iterated last (innermost loop), i.e. the "slice" axis.
        /// </summary>
        public Axis MinorAxis { get; }

        /// <summary>
        /// The index along the MinorAxis for this plane.
        /// For instance, if MinorAxis = FrontToBack, then this might be z=0,1,2...
        /// </summary>
        public uint SliceIndex { get; }

        /// <summary>
        /// 2D array of voxels in this plane. The shape depends on which two axes 
        /// are used for the plane coordinates. 
        /// E.g. if MinorAxis=Z, then (X,Y) might define Voxels[x,y].
        /// </summary>
        public Voxel?[,] Voxels { get; }

        public VisiblePlane(
            Axis majorAxis,
            Axis middleAxis,
            Axis minorAxis,
            uint sliceIndex,
            uint width,
            uint height)
        {
            MajorAxis = majorAxis;
            MiddleAxis = middleAxis;
            MinorAxis = minorAxis;
            SliceIndex = sliceIndex;
            Voxels = new Voxel?[width, height];
        }

        /// <summary>
        /// Check if this plane contains no voxels.
        /// </summary>
        public bool IsPlaneEmpty
        {
            get
            {
                int w = Voxels.GetLength(0);
                int h = Voxels.GetLength(1);
                for (int x = 0; x < w; x++)
                {
                    for (int y = 0; y < h; y++)
                    {
                        if (Voxels[x, y] != null)
                            return false;
                    }
                }
                return true;
            }
        }

        /// <summary>
        /// For debugging: returns a string like:
        /// "Plane(Major=BottomToTop, Middle=LeftToRight, Minor=FrontToBack, SliceIndex=2)"
        /// </summary>
        public override string ToString()
        {
            return $"Plane(Major={MajorAxis}, Middle={MiddleAxis}, Minor={MinorAxis}, SliceIndex={SliceIndex})";
        }


        
        /// <summary>
        /// Produces a multi-line string describing the plane's axes, slice index, 
        /// and the IDs of any voxels in the 2D array.
        /// </summary>
        public string Describe()
        {
            var sb = new StringBuilder();

            // Header info
            sb.AppendLine($"Plane(Major={MajorAxis}, Middle={MiddleAxis}, Minor={MinorAxis}, SliceIndex={SliceIndex})");
            sb.AppendLine("Voxels (each cell shows 'ID' or '.' if null):");

            int width  = Voxels.GetLength(0);
            int height = Voxels.GetLength(1);

            // We'll print row by row, top row = 0 for clarity,
            // but you can invert the order if you prefer a top-down view.
            for (int y = 0; y < height; y++)
            {
                sb.Append($"Row {y}: ");
                for (int x = 0; x < width; x++)
                {
                    var v = Voxels[x, y];
                    if (v is null)
                    {
                        sb.Append(". ");
                    }
                    else
                    {
                        sb.Append(v.ID).Append(' ');
                    }
                }
                sb.AppendLine();
            }

            return sb.ToString();
        }
    }
}
