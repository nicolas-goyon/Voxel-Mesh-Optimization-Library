// using VoxelMeshOptimizer.Core;
// using VoxelMeshOptimizer.Core.OptimizationAlgorithms.DisjointSet;

// namespace ConsoleAppExample;

// class Program
// {
//     static void Main(string[] args)
//     {
//         ushort[,,] voxels = {
//             { {0,0,0} , {0,0,0}, {0,0,0}},
//             { {0,0,0} , {0,0,0}, {0,0,0}},
//             { {0,0,0} , {0,0,0}, {0,0,0}},
//         };
//         var exampleChunk = new ExampleChunk(voxels);
//         var optimizer = new DisjointSetMeshOptimizer();
//         Mesh optimizedMesh = optimizer.Optimize(exampleChunk);

//         Console.WriteLine("Mesh optimized successfully!");
//     }
// }


// using System.Data;

// public interface IChunk<T> where T: IVoxel{
//     public T Get(int x, int y);

//     public int GetSize => 2;
// }

// public interface IVoxel {
//     public int data{get;}
// }

// public class ExChunk : IChunk<ExVoxel>{
//     private ExVoxel data;
//     public ExVoxel Get(int x, int y) => data;
//     public void Set(int x, int y, ExVoxel newData) => data = newData;
// }

// public class ExVoxel : IVoxel{
//     public int data {get;set;}
// }



// class Program
// {
//     static void Main(string[] args)
//     {
//         IChunk<ExVoxel> masterData = new ExChunk();
//         // masterData.Set(1,2, new ExVoxel());

//         Console.WriteLine(masterData.GetSize);// Compile error, GetSize doesn't exist in ExChunk
//     }
// }