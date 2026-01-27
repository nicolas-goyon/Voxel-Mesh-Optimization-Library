using System.Collections.Concurrent;
using VoxelMeshOptimizer.Helper;

namespace VoxelMeshOptimizer.Core.Multithreading
{
    public sealed class ChunkGenerationThread : IDisposable
    {
        private readonly Queue<(Int3 chunkPosition, Func<Int3, Chunk> chunkDataGenerationCallback)> _tasks = new();
        private readonly ConcurrentQueue<ChunkMeshGenerationWorker> _pendingWorkers = new();
        private readonly ConcurrentQueue<(Chunk chunk, Mesh mesh)> _completedMeshes = new();
        private readonly AutoResetEvent _signal = new(false);
        private readonly Thread _thread;
        private volatile bool _running = true;
        private readonly int _maxConcurrentWorkers;

        public ChunkGenerationThread(int maxConcurrentWorkers = 2)
        {
            _thread = new Thread(ThreadLoop)
            {
                IsBackground = true,
                Name = "ChunkMeshGeneration"
            };
            _thread.Start();
            _maxConcurrentWorkers = maxConcurrentWorkers;
        }

        public string PendingMeshesCount => _completedMeshes.Count.ToString();

        public void EnqueueChunk(Int3 chunkPosition, Func<Int3, Chunk> chunkDataGenerationCallback)
        {
            if (!_running)
            {
                throw new ObjectDisposedException(nameof(ChunkGenerationThread));
            }

            _tasks.Enqueue((chunkPosition, chunkDataGenerationCallback));
            _signal.Set();
        }


        public bool TryDequeueGeneratedMesh(out (Chunk chunk, Mesh mesh) result)
        {
            return _completedMeshes.TryDequeue(out result);
        }

        private void ThreadLoop()
        {
            while (true)
            {
                // Start new workers if we have capacity
                while (_pendingWorkers.Count < _maxConcurrentWorkers && _tasks.Count > 0)
                {
                    (Int3 chunkPosition, Func<Int3, Chunk> chunkDataGenerationCallback) = _tasks.Dequeue();
                    if (chunkDataGenerationCallback == null) throw new ArgumentNullException(nameof(chunkDataGenerationCallback));
                    Chunk chunk = chunkDataGenerationCallback(chunkPosition);
                    ChunkMeshGenerationWorker worker = new ChunkMeshGenerationWorker(chunk);
                    _pendingWorkers.Enqueue(worker);
                }

                // Process existing workers
                int pendingCount = _pendingWorkers.Count;
                for (int i = 0; i < pendingCount; i++)
                {
                    if (!_pendingWorkers.TryDequeue(out ChunkMeshGenerationWorker worker)) continue;
                    
                    (Chunk chunk, Mesh mesh) = worker.Execute();
                    _completedMeshes.Enqueue((chunk, mesh));
                }

                // If no tasks are pending and no workers are running, wait for new tasks
                if (_tasks.Count != 0 || !_pendingWorkers.IsEmpty) continue;
                
                if (!_running)
                {
                    return;
                }
                _signal.WaitOne();
            }
        }
        
        public void Dispose()
        {
            if (!_running)
            {
                return;
            }

            _running = false;
            _signal.Set();
            // _thread.Join(); 
            _thread.Abort();
            _signal.Dispose();
        }
        
    }
}