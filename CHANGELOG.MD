# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

---
## [v1.0.1] - 2025-04-19
### Changed
- **DisjointSet plane optimization:** 
  - Remove legacy ToResult method
  - Updated test logic
  - File renamed


## [v1.0.0] - 2025-04-19
### Added 
- **End-To-End Logic :**
  - DisjointSetOptimizer done
  - Full tests added
### Upcoming
- Maintainablility refactor
- SIMD/Vectorization operation


## [v0.1.0] - 2025-04-11
### Added
- **Occlusion Logic:**  
  - Implemented and tested occlusion functionality for extracting visible voxel faces.
- **2D Disjoint Set Algorithm:**  
  - Completed and tested the 2D disjoint set algorithm for mesh optimization.
- **Core Representations:**  
  - Basic implementations for voxel, chunk, and mesh representations.
- **Example Application:**  
  - A console application demonstrating basic usage of the library.

### Incomplete / Pending Implementation
- **End-to-End Integration Method:**  
  - A final integration method that should:
    - Use the occlusion logic to extract all visible faces.
    - Optimize these faces using the 2D disjoint set algorithm.
    - Generate an optimized mesh from the processed data.
    
  *Note: This integration is ~90% complete. Final adjustments are needed to close the end-to-end workflow, which is crucial for the upcoming v1 release featuring a fully working implementation.*

### Changed
- Established the initial project structure with a clear separation between core functionality, examples, and tests.

### Fixed
- Addressed minor issues in the test harness for occlusion processing and the disjoint set algorithm.
