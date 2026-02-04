# Voxel Mesh Optimization Library

`VoxelMeshOptimizer` turns voxel chunks into compact meshes.  It removes hidden faces and merges quads so that a whole chunk can be rendered with a minimal triangle count.

## Install
```bash
dotnet add package VoxelMeshOptimizer
```

## Quick start

### Basic usage 
```csharp
using VoxelMeshOptimizer.Core;
using VoxelMeshOptimizer.Core.OptimizationAlgorithms.DisjointSet;
using VoxelMeshOptimizer.Toolkit;

var chunk = new ExampleChunk(PerlinNoiseChunkGen.CreatePerlinLandscape(50, 123));
var optimizer = new DisjointSetMeshOptimizer(new ExampleMesh());
Mesh mesh = optimizer.Optimize(chunk);
File.WriteAllText("chunk.obj", ObjExporter.MeshToObjString(mesh));
```

### Multithreaded usage (Tasks-based)


### Multithreading usage


```csharp
using VoxelMeshOptimizer.Core;
using VoxelMeshOptimizer.Core.OptimizationAlgorithms.DisjointSet;
using VoxelMeshOptimizer.Toolkit;

// Initialization of the TaskManager
using CancellationTokenSource cts = new();
// Decomposition of the types : 
// - Int3 pos : Position of the chunks, usefull when generating a bunch of chunks and you need to remember where to put them
// - Func<Int3, Chunk> gen : Funciton to generate the Chunk's data
// - (Chunk chunk, Mesh mesh) : Result of a task, the initial chunk and its mesh
TaskManager<(Int3 pos, Func<Int3, Chunk> gen), (Chunk chunk, Mesh mesh)> manager =  new(
    workerCount: Environment.ProcessorCount, // TODO : set to 1 if you want one worker
    processor: (job, ct) => // TODO : What one job actually do
    {
        (Int3 pos, Func<Int3, Chunk> gen) = job;
        Chunk chunk = gen(pos);

        DisjointSetMeshOptimizer optimizer = new(new Mesh([]));
        Mesh mesh = optimizer.Optimize(chunk);

        return ValueTask.FromResult((chunk, mesh));
    }
);


// Creating chunks
int iterations = 3;
Int3 chunkPos = new() { x = 0, y = 0, z = 0 };
for (int i = 0; i < iterations; i++)
{
    _ = manager.Enqueue((chunkPos,
        p => new Chunk(PerlinNoiseChunkGen.CreatePerlinLandscape(size: 16, seed: 123))));
}

for (int i = 0; i < iterations; i++)
{
    if (manager.TryDequeueCompleted(out TaskManager<(Int3 pos, Func<Int3, Chunk> gen), (Chunk chunk, Mesh mesh)>.Completion r))
    {
        if (r.Error) {
            // An issue occured during the process
            // TODO : Handle the error
        }
        
        // TODO : Do what you like with the chunk
    }
    else
    {
        // TODO : Continue program's work and come back later
    }
    
}

// Destroy manager and prevent leaks
await manager.DisposeAsync();
```


## General Workflow

1. Build or load a `Chunk<Voxel>`.
2. `DisjointSetMeshOptimizer` uses `VoxelOcclusionOptimizer` and a 2‑D union‑find to merge faces.
3. Export the resulting mesh or feed it to your engine.

See the [GitHub repository](https://github.com/nicolas-goyon/Voxel-Mesh-Optimization-Library) for examples, benchmarks and complete documentation.
