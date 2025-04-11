
# Architecture Overview

This document describes the architecture of the Voxel Mesh Optimization Library. It outlines the design, major components, data flow, and key algorithms that enable the library to optimize voxel meshes by merging multiple voxel faces into a single, efficient mesh.

## 1. System Overview

The core goal of this library is to convert a chunk (a 3D grid of voxels) into a single optimized mesh for real-time rendering. The process is broken down into several main steps:
- **Occlusion Processing:**  
  Determines visible voxel faces by identifying which faces are not obscured by neighboring voxels.
- **Face Merging:**  
  Applies a 2D disjoint set algorithm on the extracted visible faces to merge adjacent faces with the same color into larger, contiguous faces.
- **Mesh Generation:**  
  Uses the optimized face data to construct a triangle-based mesh, reducing the overall number of triangles that need to be rendered.

## 2. Major Components

### 2.1 Voxel, Chunk, and Mesh Representations

- **Voxel:**  
  A voxel represents a colored point, modeled as a cube. It lacks texture mapping and supports a single solid color.
- **Chunk:**  
  A chunk is a rectangular grid of voxels defined by dimensions along the X, Y, and Z axes. Chunks may contain empty or semi-transparent voxels.
- **Mesh:**  
  A mesh is a 3D construct composed of triangles, used for rendering objects in game engines and other applications.

### 2.2 Core Modules

- **Occlusion Logic Module:**  
  Contains algorithms to determine which voxel faces are visible based on their adjacency to other voxels. This is critical for reducing unnecessary geometry in the final mesh.
  
- **Disjoint Set Optimization Module:**  
  Implements a 2D disjoint set (union-find) algorithm to merge adjacent visible faces that share the same color into larger faces. This reduces the number of triangles in the mesh.
  
- **Mesh Optimizer Interface:**  
  Defines the interface for mesh optimization. Implementations, such as the current disjoint set-based optimizer, adhere to this interface. This abstraction makes the library extensible and technology-agnostic.

- **Factory (Planned):**  
  Although the current implementation instantiates the optimizer using direct `new` calls, a factory pattern is planned. This will hide the actual class instantiation and improve maintainability by decoupling class names from usage.

## 3. Data Flow

1. **Input Stage:**  
   The process starts with a chunk that contains a set of voxels.
   
2. **Occlusion Processing:**  
   The occlusion module analyzes each voxel in the chunk to extract the visible faces. This step determines which faces will appear in the final mesh and discards faces that are not visible.
   
3. **Optimization via Disjoint Set:**  
   The visible faces are then passed through the disjoint set algorithm, which merges contiguous faces with matching color properties.
   
4. **Mesh Generation:**  
   Finally, the optimized face data is used to generate a mesh. This mesh is a collection of vertices and triangles that represent the final 3D object.

## 4. Design Diagrams

Below is a detailed PlantUML diagram  that illustrates the classes and their interactions.
![Design Overview Diagram](./docs/diagrams/ClassDiagram.svg)


## 5. Design Decisions

- **Modular Design:**  
  The separation of occlusion, optimization, and mesh generation into distinct modules allows independent development and testing. It also makes future optimization techniques easier to integrate.
  
- **Interface-Based Approach:**  
  By defining clear interfaces (such as for `MeshOptimizer`, `Chunk`, `Voxel`, and `Mesh`), the library is flexible and can accommodate various implementations. This approach supports future improvements without breaking existing usage.

- **Extensibility with Factory Pattern (Future Work):**  
  The plan to integrate a factory for optimizer instantiation will help ensure durability and ease of maintenance as new optimization algorithms are developed.

## 6. Future Enhancements

- **End-to-End Integration:**  
  Finalize the method that combines occlusion processing and 2D disjoint set optimization into a comprehensive workflow.
  
- **Performance Optimizations:**  
  Explore advanced techniques like contiguous memory arrays, vectorized operations, SIMD instructions, caching strategies, and parallel processing.

- **Improved Maintainability:**  
  Implement a factory pattern to manage optimizer instantiation, decoupling direct class dependencies.

---

*For further details or questions regarding the architecture, please refer to the [CONTRIBUTING.md](CONTRIBUTING.md) and open an issue in the repository.*
