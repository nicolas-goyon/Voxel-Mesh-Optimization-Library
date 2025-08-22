# Voxel Mesh Optimization Library

`VoxelMeshOptimizer` turns voxel chunks into compact meshes.  It removes hidden faces and merges quads so that a whole chunk can be rendered with a minimal triangle count.

## Install
```bash
dotnet add package VoxelMeshOptimizer
```

## Quick start
```csharp
using VoxelMeshOptimizer.Core;
using VoxelMeshOptimizer.Core.OptimizationAlgorithms.DisjointSet;
using VoxelMeshOptimizer.Toolkit;

var chunk = new ExampleChunk(PerlinNoiseChunkGen.CreatePerlinLandscape(50, 123));
var optimizer = new DisjointSetMeshOptimizer(new ExampleMesh());
Mesh mesh = optimizer.Optimize(chunk);
File.WriteAllText("chunk.obj", ObjExporter.MeshToObjString(mesh));
```

1. Build or load a `Chunk<Voxel>`.
2. `DisjointSetMeshOptimizer` uses `VoxelOcclusionOptimizer` and a 2‑D union‑find to merge faces.
3. Export the resulting mesh or feed it to your engine.

See the [GitHub repository](https://github.com/nicolas-goyon/Voxel-Mesh-Optimization-Library) for examples, benchmarks and complete documentation.
