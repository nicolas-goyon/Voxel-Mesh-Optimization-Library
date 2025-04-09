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
            var result = new VisibleFaces();

            result.PlanesByAxis[(Axis.X, AxisOrder.Ascending)]  = BuildPlanesForAxis(Axis.X, AxisOrder.Ascending);
            result.PlanesByAxis[(Axis.X, AxisOrder.Descending)] = BuildPlanesForAxis(Axis.X, AxisOrder.Descending);

            result.PlanesByAxis[(Axis.Y, AxisOrder.Ascending)]  = BuildPlanesForAxis(Axis.Y, AxisOrder.Ascending);
            result.PlanesByAxis[(Axis.Y, AxisOrder.Descending)] = BuildPlanesForAxis(Axis.Y, AxisOrder.Descending);

            result.PlanesByAxis[(Axis.Z, AxisOrder.Ascending)]  = BuildPlanesForAxis(Axis.Z, AxisOrder.Ascending);
            result.PlanesByAxis[(Axis.Z, AxisOrder.Descending)] = BuildPlanesForAxis(Axis.Z, AxisOrder.Descending);


            return result;
        }

        /// <summary>
        /// Builds planes for the specified 'slice' axis 
        /// (i.e. the axis that determines sliceIndex).
        /// Also returns which major/middle/minor axes were used (for debug).
        /// </summary>
        private List<VisiblePlane> BuildPlanesForAxis(Axis sliceAxis, AxisOrder axisOrder)
        {
            // 1) Map sliceAxis to the bitmask face (Front, Back, etc.)
            var faceFlag = AxisExtensions.ToVoxelFace(sliceAxis, axisOrder);

            var (majorA, majorAO, middleA, middleAO, minorA, minorAO) = AxisExtensions.DefineIterationOrder(sliceAxis, axisOrder);

            (uint planeWidth, uint planeHeight) = chunk.GetPlaneDimensions(majorA, middleA, minorA);

            // 3) We create a dictionary of planes, keyed by "sliceIndex."
            //    E.g., if slicing on Z, "sliceIndex" is z in [0..Depth-1].
            var planesBySlice = new Dictionary<uint, VisiblePlane>();
            uint sliceCount = chunk.GetDepth(sliceAxis);

            chunk.ForEachCoordinate(
                major:  majorA, majorAsc: majorAO,
                middle: middleA, middleAsc: middleAO,
                minor:  minorA, minorAsc: minorAO,
                (uint x, uint y, uint z) =>
                {
                    var faces = visibilityMap.GetVisibleFaces(x, y, z);
                    if (!faces.HasFlag(faceFlag)) return;

                    // We need the "slice index" for x or y or z, 
                    // depending on which axis is minor. 
                    uint sliceIndex = AxisExtensions.GetDepthFromAxis(sliceAxis, axisOrder, x, y, z, chunk);

                    // Then fill in the plane's 2D array, 
                    // figuring out which (planeX, planeY) coords to use
                    // depending on the other two axes.
                    var plane = planesBySlice[sliceIndex];

                    // Calculate planeX, planeY based on the other two axes (major, middle).
                    var (planeX, planeY) = AxisExtensions.GetSlicePlanePosition(
                        majorA, majorAO, 
                        middleA, middleAO, 
                        minorA, minorAO, 
                        x, y, z, chunk);

                    plane.Voxels[planeX, planeY] = chunk.Get(x, y, z);
                }
            );

            // 7) Filter out planes that remain empty
            var result = new List<VisiblePlane>();
            foreach (var kvp in planesBySlice)
            {
                if (!kvp.Value.IsPlaneEmpty)
                {
                    result.Add(kvp.Value);
                }
            }

            return result;
        }





    }
}
