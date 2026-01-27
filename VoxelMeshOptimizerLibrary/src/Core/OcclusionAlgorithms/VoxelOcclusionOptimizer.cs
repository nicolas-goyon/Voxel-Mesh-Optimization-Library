using System.Data;
using VoxelMeshOptimizer.Core.OcclusionAlgorithms.Common;

namespace VoxelMeshOptimizer.Core.OcclusionAlgorithms;
/// <summary>
/// Optimizes voxel occlusion by computing visible voxel planes based on the provided voxel chunk.
/// </summary>
public class VoxelOcclusionOptimizer
{
    /// <summary>
    /// The voxel chunk to be processed.
    /// </summary>
    private readonly Chunk chunk;

    /// <summary>
    /// The visibility map generated from the voxel chunk.
    /// </summary>
    private readonly VoxelVisibilityMap visibilityMap;

    /// <summary>
    /// Initializes a new instance of the <see cref="VoxelOcclusionOptimizer"/> class.
    /// </summary>
    /// <param name="chunk">The voxel chunk to optimize.</param>
    public VoxelOcclusionOptimizer(Chunk chunk)
    {
        this.chunk = chunk ?? throw new NoNullAllowedException();
        visibilityMap = new VoxelVisibilityMap(chunk);
    }

    /// <summary>
    /// Computes and returns the visible voxel planes for all axes and their orientations.
    /// </summary>
    /// <returns>
    /// A <see cref="VisibleFaces"/> instance containing the visible planes grouped by axis and order.
    /// </returns>
    public VisibleFaces ComputeVisibleFaces()
    {
        VisibleFaces result = new VisibleFaces
        {
            PlanesByAxis =
            {
                [(Axis.X, AxisOrder.Ascending)] = BuildPlanesForAxis(Axis.X, AxisOrder.Ascending),
                [(Axis.X, AxisOrder.Descending)] = BuildPlanesForAxis(Axis.X, AxisOrder.Descending),
                [(Axis.Y, AxisOrder.Ascending)] = BuildPlanesForAxis(Axis.Y, AxisOrder.Ascending),
                [(Axis.Y, AxisOrder.Descending)] = BuildPlanesForAxis(Axis.Y, AxisOrder.Descending),
                [(Axis.Z, AxisOrder.Ascending)] = BuildPlanesForAxis(Axis.Z, AxisOrder.Ascending),
                [(Axis.Z, AxisOrder.Descending)] = BuildPlanesForAxis(Axis.Z, AxisOrder.Descending)
            }
        };

        return result;
    }

    /// <summary>
    /// Builds the visible planes for a specified slicing axis and order.
    /// </summary>
    /// <param name="sliceAxis">The axis that defines the slicing orientation.</param>
    /// <param name="axisOrder">The order (ascending or descending) in which to iterate over the slice.</param>
    /// <returns>
    /// A list of <see cref="VisiblePlane"/> objects representing the visible voxel planes for the provided slice.
    /// </returns>
    /// <remarks>
    /// The method maps the slice axis to the appropriate voxel face using a bitmask and 
    /// determines the iteration order via a helper function (<see cref="AxisExtensions.DefineIterationOrder"/>).
    /// This helper also computes the corresponding 2D plane dimensions and translates 
    /// 3D voxel coordinates to 2D plane positions using <see cref="AxisExtensions.GetSlicePlanePosition"/>.
    /// This design choice encapsulates complex logic within a dedicated helper, aiding maintainability.
    /// </remarks>
    private List<VisiblePlane> BuildPlanesForAxis(Axis sliceAxis, AxisOrder axisOrder)
    {
        // Map the slice axis to the corresponding voxel face flag.
        VoxelFace faceFlag = AxisExtensions.ToVoxelFace(sliceAxis, axisOrder);

        // Determine the order in which the voxels are iterated.
        (Axis majorA, AxisOrder majorAO, Axis middleA, AxisOrder middleAO, Axis minorA, AxisOrder minorAO) = AxisExtensions.DefineIterationOrder(sliceAxis, axisOrder);

        // Retrieve the dimensions of the 2D plane slice.
        (uint planeWidth, uint planeHeight) = chunk.GetPlaneDimensions(majorA, middleA, minorA);

        // Dictionary to store the visible planes keyed by the slice index.
        uint sliceCount = chunk.GetDepth(sliceAxis);
        VisiblePlane?[] planesBySlice = new VisiblePlane?[sliceCount];

        chunk.ForEachCoordinate(
            majorA: majorA, majorAsc: majorAO,
            middleA: middleA, middleAsc: middleAO,
            minorA: minorA, minorAsc: minorAO,
            (x, y, z) =>
            {
                VoxelFace faces = visibilityMap.GetVisibleFaces(x, y, z);
                if (!faces.HasFlag(faceFlag)) return;

                // Retrieve the current slice index.
                uint sliceIndex = AxisExtensions.GetDepthFromAxis(sliceAxis, axisOrder, x, y, z, chunk);

                // Select the appropriate visible plane.
                planesBySlice[sliceIndex] ??= new VisiblePlane(
                    majorA, majorAO,
                    middleA, middleAO,
                    minorA, minorAO,
                    sliceIndex,
                    planeWidth, planeHeight
                );
                VisiblePlane? plane = planesBySlice[sliceIndex];

                // Compute the 2D position on the plane from the 3D coordinates.
                (uint planeX, uint planeY) = AxisExtensions.GetSlicePlanePosition(
                    majorA, majorAO,
                    middleA, middleAO,
                    minorA, minorAO,
                    x, y, z, chunk);

                plane!.Voxels[planeX, planeY] = chunk.Get(x, y, z);
            }
        );

        // Gather the resulting non-empty planes.
        List<VisiblePlane> result = [];
        result.AddRange(planesBySlice.OfType<VisiblePlane>().Where(plane => !plane.IsPlaneEmpty));

        return result;
    }
}
