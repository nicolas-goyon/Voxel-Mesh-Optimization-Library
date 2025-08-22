# Architecture Overview

The Voxel Mesh Optimization Library converts a voxel chunk into a compact mesh.  The design separates voxel visibility detection, face optimisation and mesh generation so that each part can evolve independently.

## System Overview
1. **Occlusion Processing** – `VoxelOcclusionOptimizer` scans a `Chunk<Voxel>` and records which faces are visible.
2. **Face Merging** – `DisjointSetVisiblePlaneOptimizer` applies a 2‑D union‑find on each visible plane to merge adjacent quads.
3. **Mesh Generation** – `VisibleFacesMesher` turns the optimised planes into `MeshQuad` objects that compose the final `Mesh`.
4. **Toolkit & Export** – Helpers such as `PerlinNoiseChunkGen` create sample chunks and `ObjExporter` writes meshes as Wavefront OBJ for inspection.

## Major Components
- **Core types** – `Voxel`, `Chunk<TVoxel>`, `Mesh`, `MeshQuad`.
- **OcclusionAlgorithms** – exposes `VoxelOcclusionOptimizer` and supporting utilities.
- **OptimizationAlgorithms** – contains `DisjointSetMeshOptimizer` and plane optimisers.
- **Toolkit** – utilities for chunk generation, meshing of visible faces and mesh export.
- **Examples & Benchmarks** – separate projects demonstrating usage and automatically verifying performance targets.

## Data Flow
```text
Chunk -> VoxelOcclusionOptimizer -> VisibleFaces -> DisjointSetVisiblePlaneOptimizer -> VisibleFacesMesher -> Mesh -> (ObjExporter)
```

## Design Decisions
- **Interface‑driven**: key abstractions (`MeshOptimizer`, `Chunk`, `Voxel`, `Mesh`) allow engine‑agnostic usage.
- **Modular**: occlusion, optimisation and export modules can be replaced or extended without affecting others.
- **Performance awareness**: benchmarks run in CI to guard against regressions; results are published on GitHub Pages.

## Future Work
Potential future enhancements include SIMD/vectorised operations, more advanced meshing strategies and parallel processing.
