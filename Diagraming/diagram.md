classDiagram
  namespace ConsoleAppExample {
    class ExampleVoxel {
    }
    class Program {
    }
    class ExampleMesh {
      +Vertices: IReadOnlyList<(float x, float y, float z)>
      +Triangles: IReadOnlyList<int>
    }
    class ExampleChunk {
      +GetVoxels() IEnumerable<ExampleVoxel>
      +Set(uint x,  uint y,  uint z,  ExampleVoxel voxel) void
      +AreDifferentAxis(Axis major,  Axis middle,  Axis minor) bool
    }
  }
  namespace VoxelMeshOptimizer_Core {
    class MeshOptimizer {
      +Optimize(Chunk<Voxel> chunk) Mesh
    }
    class Voxel {
      +ID: ushort
      +IsSolid: bool
    }
    class VoxelFace {
      +None
      +Zpos
      +Zneg
      +Xneg
      +Xpos
      +Ypos
      +Yneg
    }
    class Axis {
      +X
      +Y
      +Z
    }
    class AxisOrder {
      +Ascending
      +Descending
    }
    class AxisExtensions {
      +«static» ToVoxelFace(Axis axis,  AxisOrder axisOrder) VoxelFace
      +«static» GetDepthFromAxis(Axis axis,  AxisOrder axisOrder,  uint x,  uint y,  uint z,  Chunk<Voxel> chunk) uint
    }
    class Chunk {
      +XDepth: uint
      +YDepth: uint
      +ZDepth: uint
      +ForEachCoordinate(Axis major,  AxisOrder majorAsc,  Axis middle,  AxisOrder middleAsc,  Axis minor,  AxisOrder minorAsc,  Action<uint ,  uint ,  uint > action) void
      +Get(uint x,  uint y,  uint z) T
      +GetDepth(Axis axis) uint
      +IsOutOfBound(uint x,  uint y,  uint z) bool
      +AreDifferentAxis(Axis major, Axis middle, Axis minor) bool
    }
    class Mesh {
    }
    class VoxelVisibilityMap {
      +GetVisibleFaces(uint x,  uint y,  uint z) VoxelFace
    }
  }
  namespace VoxelMeshOptimizer_Core_OcclusionAlgorithms {
    class VoxelOcclusionOptimizer {
    }
    class Occluder {
      +ComputeVisiblePlanes() VisibleFaces
    }
  }
  namespace VoxelMeshOptimizer_Core_OcclusionAlgorithms_Common {
    class VisibleFaces {
      +PlanesByAxis: Dictionary<(Axis, AxisOrder), List<VisiblePlane>>
    }
    class VisiblePlane {
      +MajorAxis: Axis
      +MajorAxisOrder: AxisOrder
      +MiddleAxis: Axis
      +MiddleAxisOrder: AxisOrder
      +MinorAxis: Axis
      +MinorAxisOrder: AxisOrder
      +SliceIndex: uint
      +IsPlaneEmpty: bool
      +Describe() string
    }
  }
  namespace VoxelMeshOptimizer_Core_OptimizationAlgorithms_DisjointSet {
    class DisjointSet2DOptimizer {
      +Optimize() void
    }
    class DisjointSetMeshOptimizer {
    }
  }
  namespace VoxelMeshOptimizer_Tests_Core {
    class ChunkInterfaceTests {
      +ChunkCreation_Error_WhenThedimentionsAre0() void
      +ChunkDimensionsAndPlaneDimensions() void
      +AreDifferentAxis() void
      +GetDimension_ForEachCoordinate_CorrectOrder() void
      +GetDimension_ForEachCoordinate_Throws_WhenAxisEquals() void
      +SetGet_ThrowsError_WhenOutOfBound() void
    }
    class AxisExtensionsTests {
      +GetDepthFromAxis_Ascending_ReturnsCoordinateValue_ForAxisX() void
      +GetDepthFromAxis_Descending_ReturnsFlippedValue_ForAxisX() void
      +GetDepthFromAxis_OnNearFace_ReturnsZeroDepth() void
      +GetDepthFromAxis_OnFarFace_ReturnsMaxDepth_ForDescendingOrder() void
      +GetDepthFromAxis_OutOfBoundCoordinate_ThrowsArgumentOutOfRangeException() void
      +DefineIterationOrder_ForMajorAxisX_ReturnsExpectedMapping() void
      +DefineIterationOrder_ForMajorAxisY_ReturnsExpectedMapping() void
      +DefineIterationOrder_ForMajorAxisZ_ReturnsExpectedMapping() void
      +DefineIterationOrder_InvalidNonDistinctAxes_ThrowsException() void
      +GetSlicePlanePosition_Ascending_ReturnsSameAbsoluteCoordinates() void
      +GetSlicePlanePosition_Descending_ReturnsFlippedCoordinatesForPlaneAxes() void
      +GetSlicePlanePosition_MixedOrders_SliceOrderDoesNotInfluenceOutput() void
      +GetSlicePlanePosition_NonDistinctAxes_ThrowsArgumentException() void
      +GetDepthFromAxis_InvalidEnumValue_CastInteger_ThrowsExceptionOrHandlesGracefully() void
    }
  }
  namespace VoxelMeshOptimizer_Tests_DisjointSetTesting {
    class DisjointSetTests {
      +InitialCount_ShouldBeEqualToNumberOfElements() void
      +Union_ShouldReduceSetCount() void
      +Find_ShouldReturnSameRoot_ForConnectedElements() void
      +Find_ShouldReturnDifferentRoots_ForDisconnectedElements() void
      +IsRoot_ShouldBeTrue_ForInitialSingleElements() void
      +Union_WithSameSet_ShouldNotChangeCount() void
      +Constructor_ShouldThrowException_ForNegativeElements() void
      +Find_ShouldThrowException_ForInvalidIndex() void
      +Union_ShouldThrowException_ForInvalidIndices() void
      +ZeroElements_ShouldHaveZeroCount() void
      +Operations_ShouldThrowException_OnZeroSizedSet() void
      +MultipleUnions_ShouldCreateSingleSet() void
      +IsRoot_ShouldBeFalse_ForNonRootElements() void
    }
    class DisjointSet2DOptimizerTests {
      +Optimize_ShouldMergeIntoOneRectangle_WhenAllPixelsHaveSameValue() void
      +Optimize_ShouldNotMerge_WhenPixelsHaveDifferentValues() void
      +Optimize_ShouldCreateMultipleRectangles_ForMultipleGroups() void
      +Optimize_ShouldHandleSinglePixelGroups() void
      +Optimize_ShouldNotMergeIntoNonRectangleShapes() void
      +Constructor_NullPixels_ThrowsArgumentNullException() void
      +Constructor_EmptyPixels_ThrowsArgumentException() void
      +Optimize_SinglePixel_NoUnionPerformed() void
      +Optimize_TwoDifferentPixels_TwoSetsCreated() void
      +Optimize_TwoSamePixels_OneSetCreated() void
      +Optimize_RectangleOfSameColor_CreatesSingleSet() void
      +Optimize_MixedColors_CreatesCorrectSets() void
    }
    class DisjointSetMeshOptimizerTests {
      +Optimize_ShouldThrow_NotImplementedException() void
    }
  }
  namespace VoxelMeshOptimizer_Tests_DummyClasses {
    class TestChunk {
      +Set(uint x,  uint y,  uint z,  TestVoxel voxel) void
      +AreDifferentAxis(Axis major,  Axis middle,  Axis minor) bool
    }
    class TestVoxel {
    }
  }
  namespace VoxelMeshOptimizer_Tests_Occlusion {
    class VoxelVisibilityMapTests {
      +SolidChunk_AllOuterFacesShouldBeVisible_InnerFacesNotVisible() void
      +SingleVoxel_AllSixFacesShouldBeVisible() void
      +EmptyChunk_NoVoxelsNoVisibleFaces() void
      +MixedSolidAndNull_CheckTransitions() void
      +CheckErrorHandling_OutOfRangeShouldReturnNone() void
    }
  }
  namespace VoxelMeshOptimizer_Tests_OcclusionAlgorithms_Common {
    class VisiblePlaneTests {
      +Constructor_ShouldInitializePropertiesCorrectly() void
      +IsPlaneEmpty_ShouldReturnTrue_WhenNoVoxelsAreSet() void
      +IsPlaneEmpty_ShouldReturnFalse_WhenAtLeastOneVoxelIsSet() void
      +ToString_ShouldIncludeAxisOrderSigns() void
      +Describe_ShouldIncludeAxisOrderSignsAndVoxelData() void
      +Constructor_ShouldHandleZeroSizedPlane() void
      +Voxels_SetVoxel_ShouldBeAccessibleCorrectly() void
    }
  }
  namespace VoxelMeshOptimizer_Tests_OcclusionTests {
    class VoxelOcclusionOptimizerTests {
      +Constructor_NullChunk_ThrowsException() void
      +ComputeVisiblePlanes_EmptyChunk_ReturnsNoVisibleFaces() void
      +ComputeVisiblePlanes_FullChunk_Returns6VisibleFaces() void
      +ComputeVisiblePlanes_SingleVoxelChunk_Returns6VisibleFaces() void
      +ComputeVisiblePlanes_IrregularChunk_WithInternalHole_ProducesExpectedVisibleFaces() void
    }
  }

  VoxelOcclusionOptimizer --|> Occluder
  DisjointSetMeshOptimizer --|> MeshOptimizer
  TestChunk --|> Chunk
  TestVoxel --|> Voxel
  ExampleVoxel --|> Voxel
  ExampleMesh --|> Mesh
  ExampleChunk --|> Chunk

  AxisExtensions ..> Axis
  AxisExtensions ..> VoxelFace
  Chunk ..> Axis
  ExampleChunk ..> ExampleVoxel
  MeshOptimizer ..> Mesh
  Occluder ..> VisibleFaces
  TestChunk ..> TestVoxel
  VisiblePlane ..> Axis
  VisiblePlane ..> AxisOrder
  VoxelVisibilityMap ..> VoxelFace