# Voxel Mesh Optimization Library

## Overview

The **Voxel Mesh Optimization Library** is a C# library designed to optimize voxel-based meshes for real-time rendering—ideal for game developers and any user interested in leveraging efficient 3D mesh generation techniques. The library transforms a collection of individual colored voxels into a single optimized mesh, reducing redundant faces and merging triangles with identical colors to improve display performance.

### Key Definitions

- **Voxel:**  
  A voxel is a colored point—in our context, treated as a colored cube. Each voxel does not support textures or multiple colors.

- **Chunk:**  
  A chunk is a rectangular, 3D grid of voxels (organized along the X, Y, and Z axes). In this project, a chunk may contain full, empty, or semi-transparent voxels.

- **Mesh:**  
  A mesh is a 3D object used in video games made up of triangles. Instead of having one mesh per voxel, this library builds a single mesh for an entire chunk by eliminating occluded faces and merging triangles that share the same color.

## Problem & Purpose

The main goal of this project is to improve rendering performance by reducing the number of triangles that need to be processed by creating one optimized mesh per chunk. Two main techniques are applied:

- **Occlusion Logic:**  
  Determines which voxel faces are visible based on their spatial neighbors.
- **Disjoint Set Algorithm (2D Optimization):**  
  Merges adjacent, similar faces into larger contiguous faces, decreasing the overall triangle count.

Multiple techniques can be used to optimize voxel meshes. In our current implementation, a simple occlusion algorithm is paired with a disjoint set algorithm to merge faces efficiently.

## Current Status

- **Completed:**
  - Occlusion logic for extracting visible voxel faces (fully implemented and tested).
  - A 2D disjoint set algorithm that optimizes face merging.
  - Fundamental representations for voxels, chunks, and meshes.
  - A Console Application Example demonstrating basic library usage.
  
- **Pending:**
  - The final end-to-end integration method that will:
    - Use occlusion logic to extract all visible faces.
    - Optimize these faces using the 2D disjoint set algorithm.
    - Generate a complete, optimized mesh from the processed data.
  - Enhancements to the instantiation process (e.g., adding a factory pattern) for better maintainability and performance tuning.

*Note:* The project is currently about 90% complete. The final method is still under development before the next major version (v1) is released.

## Intended Audience

This library is aimed at game developers and software engineers who need an efficient, flexible solution for voxel mesh optimization. It is tech-agnostic—meaning it can be integrated into various platforms and engines (a Unity example is planned for the future).

## Core Features

- **Optimized Mesh Creation:**  
  Converts an entire chunk of voxels into one mesh by removing occluded faces and merging similar ones.
- **Modular Architecture:**  
  Interfaces hide implementation details, allowing the library to evolve without breaking existing usage.
- **Tech-Agnostic Design:**  
  While currently demonstrated with a console app example, the library is designed for easy integration into different environments.
- **Extensible and Evolutive:**  
  Future enhancements could include advanced optimization techniques such as contiguous memory arrays, vectorized operations, and support for parallelism.

## Prerequisites

- **.NET Version:**  
  This project currently runs on .NET 9.
- **Dependencies:**  
  There are no additional prerequisites beyond the standard .NET installation.

## Installation & Setup

### Cloning the Repository

Clone the repository using SSH:

```bash
git clone git@github.com:nicolas-goyon/RectangleMerging.git
```

### Building the Project

1. Navigate to the project root:

   ```bash
   cd VoxelMeshOptimizerLibrary
   dotnet restore
   ```

2. Run the Console Application Example:

   ```bash
   cd examples/ConsoleAppExample
   dotnet run
   ```

3. Run Tests:

   ```bash
   cd VoxelMeshOptimizerLibrary
   dotnet restore
   dotnet test
   ```

*No additional environment variables or configuration files are required.*

## Usage

The library’s usage is demonstrated in the `ConsoleAppExample`. The typical workflow for using the library in your project is as follows:

1. **Implement the Interfaces:**  
   Create your own implementations for voxel, chunk, and mesh by following the provided interfaces.
   
2. **Instantiate the Optimizer:**  
   Create an instance of the optimizer (or later, use the factory pattern to hide the class instantiation).
   
3. **Run the Optimization:**  
   Call the optimization method to process a chunk, which:
   - Extracts visible faces using occlusion logic.
   - Optimizes the faces using the 2D disjoint set algorithm.
   - Generates an optimized mesh based on the processed data.

*Example usage is provided in the ConsoleAppExample project.*

## Development & Contributing

For detailed guidelines on setting up a development environment, contributing code, or reporting issues, please refer to:

- [CONTRIBUTING.md](CONTRIBUTING.md)
- [CODE_OF_CONDUCT.md](CODE_OF_CONDUCT.md)
- [CHANGELOG.md](CHANGELOG.md)

*Note:*  
The current instantiation of the optimizer uses direct `new` calls. As the project evolves, consider adopting a factory pattern to increase maintainability and decouple direct dependencies.

## Roadmap & Future Enhancements

- **V1 Release:**  
  The v1 release will feature an end-to-end fully integrated optimization method that brings together occlusion logic and disjoint set optimization to create a complete mesh.
  
- **Performance Improvements:**  
  Future versions (v2 and beyond) will focus on deeper optimizations, including:
  - Enhanced internal algorithms.
  - Contiguous memory arrays.
  - Vectorized operations and SIMD instructions.
  - Integration of BLAS libraries and caching optimizations.
  - Parallelization where possible.

## Related Projects & Resources

- **Elysian Outpost:**  
  This project is part of a larger ecosystem, including the [Elysian Outpost](https://github.com/nicolas-goyon/Elysian-Outpost)—a Stonehearth-like game remake.

- **Class Code to Diagram Parser:**  
  For automatic generation of class diagrams from your C# project, check out [ClassCodeToDiagramParser](https://github.com/nicolas-goyon/ClassCodeToDiagramParser).

## Support

If you have questions, need help, or wish to contribute further improvements, please open an issue in the repository or reach out directly. Contributions and feedback are greatly appreciated!

## License

This project is licensed under the MIT License—see the [LICENSE](LICENSE) file for details.
