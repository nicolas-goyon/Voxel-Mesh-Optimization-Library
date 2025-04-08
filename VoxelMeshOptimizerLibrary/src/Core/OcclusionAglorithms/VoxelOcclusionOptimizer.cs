using System.Collections.Generic;
using VoxelMeshOptimizer.Core;
using VoxelMeshOptimizer.Core.OcclusionAlgorithms.Common;

namespace VoxelMeshOptimizer.Core.OcclusionAlgorithms
{
    public class VoxelOcclusionOptimizer : Occluder
    {
        private readonly Chunk<Voxel> chunk;
        private readonly VoxelVisibilityMap visibilityMap;

        public VoxelOcclusionOptimizer(Chunk<Voxel> chunk)
        {
            this.chunk = chunk;
            visibilityMap = new VoxelVisibilityMap(chunk);
        }

        public VisibleFaces ComputeVisiblePlanes()
        {
            // var result = new VisibleFaces();

            // // The six "human" axes we want to generate planes for:
            // var axesWeCareAbout = new[]
            // {
            //     HumanAxis.FrontToBack,
            //     HumanAxis.BackToFront,
            //     HumanAxis.LeftToRight,
            //     HumanAxis.RightToLeft,
            //     HumanAxis.BottomToTop,
            //     HumanAxis.TopToBottom
            // };

            // foreach (var axis in axesWeCareAbout)
            // {
            //     // Build planes for each axis, store them in the final data structure
            //     var planes = BuildPlanesForAxis(
            //         axis,
            //         out var majorUsed,
            //         out var middleUsed,
            //         out var minorUsed
            //     );
            //     result.PlanesByAxis[axis] = planes;
            // }

            // return result;
            throw new NotImplementedException();
        }

        /// <summary>
        /// Builds planes for the specified 'slice' axis 
        /// (i.e. the axis that determines sliceIndex).
        /// Also returns which major/middle/minor axes were used (for debug).
        /// </summary>
        // private List<VisiblePlane> BuildPlanesForAxis(
        //     HumanAxis sliceAxis,
        //     out HumanAxis majorUsed,
        //     out HumanAxis middleUsed,
        //     out HumanAxis minorUsed)
        // {
        //     // 1) Map sliceAxis to the bitmask face (Front, Back, etc.)
        //     var faceFlag = HumanAxisExtensions.ToVoxelFace(sliceAxis);

        //     (majorUsed, middleUsed, minorUsed) = HumanAxisExtensions.GetAxes(sliceAxis);
        //     (uint planeWidth, uint planeHeight) = chunk.GetPlaneDimensions(majorUsed, middleUsed, minorUsed);

        //     // 3) We create a dictionary of planes, keyed by "sliceIndex."
        //     //    E.g., if slicing on Z, "sliceIndex" is z in [0..Depth-1].
        //     var planesBySlice = new Dictionary<uint, VisiblePlane>();
        //     uint sliceCount = chunk.GetDepth(sliceAxis);

        //     chunk.ForEachCoordinate(
        //         major:  majorUsed,
        //         middle: middleUsed,
        //         minor:  minorUsed,
        //         (uint x, uint y, uint z) =>
        //         {
        //             var faces = visibilityMap.GetVisibleFaces(x, y, z);
        //             if (!faces.HasFlag(faceFlag)) return;

        //             // We need the "slice index" for x or y or z, 
        //             // depending on which axis is minor. 
        //             uint sliceIndex = ExtractCoord(sliceAxis, x, y, z);

        //             // Then fill in the plane's 2D array, 
        //             // figuring out which (planeX, planeY) coords to use
        //             // depending on the other two axes.
        //             var plane = planesBySlice[sliceIndex];

        //             // Calculate planeX, planeY based on the other two axes (major, middle).
        //             var (planeX, planeY) = HumanAxisExtension.ComputePlaneCoordinates(sliceAxis, x, y, z);

        //             plane.Voxels[planeX, planeY] = chunk.Get(x, y, z);
        //         }
        //     );

        //     // 7) Filter out planes that remain empty
        //     var result = new List<VisiblePlane>();
        //     foreach (var kvp in planesBySlice)
        //     {
        //         if (!kvp.Value.IsPlaneEmpty)
        //         {
        //             result.Add(kvp.Value);
        //         }
        //     }

        //     return result;
        // }




        // /// <summary>
        // /// Extracts the coordinate that belongs to the 'sliceAxis'.
        // /// E.g. if sliceAxis=FrontToBack => we want z
        // /// </summary>
        // private uint ExtractCoord(HumanAxis sliceAxis, uint x, uint y, uint z)
        // {
        //     return sliceAxis switch
        //     {
        //         HumanAxis.FrontToBack  => z,
        //         HumanAxis.BackToFront  => z,
        //         HumanAxis.LeftToRight  => x,
        //         HumanAxis.RightToLeft  => x,
        //         HumanAxis.BottomToTop  => y,
        //         HumanAxis.TopToBottom  => y,
        //         _ => 0
        //     };
        // }

    }
}
