using System.Threading.Channels;

namespace VoxelMeshOptimizer.Core.TaskManager;

public sealed class TaskManager<TIn, TOut> : IAsyncDisposable
{
    public readonly record struct Completion(int Id, bool Success, TOut? Result, Exception? Error);

    private sealed record WorkItem(int Id, TIn Input, TaskCompletionSource<TOut> Tcs);

    private readonly Func<TIn, CancellationToken, ValueTask<TOut>> _processor;
    private readonly CancellationToken _shutdown;

    private readonly Channel<WorkItem> _work;
    private readonly Channel<Completion> _completed;

    private readonly Task[] _workers;
    private readonly Task _drainTask;

    private int _nextId;

    public TaskManager(
        int workerCount,
        Func<TIn, CancellationToken, ValueTask<TOut>> processor,
        CancellationToken shutdown = default,
        int? boundedCapacity = null)
    {
        if (workerCount <= 0) throw new ArgumentOutOfRangeException(nameof(workerCount));
        _processor = processor ?? throw new ArgumentNullException(nameof(processor));
        _shutdown = shutdown;

        _work = boundedCapacity is { } cap
            ? Channel.CreateBounded<WorkItem>(new BoundedChannelOptions(cap)
            {
                SingleReader = false,
                SingleWriter = false,
                FullMode = BoundedChannelFullMode.Wait
            })
            : Channel.CreateUnbounded<WorkItem>(new UnboundedChannelOptions
            {
                SingleReader = false,
                SingleWriter = false
            });

        _completed = Channel.CreateUnbounded<Completion>(new UnboundedChannelOptions
        {
            SingleReader = false,
            SingleWriter = false
        });

        _workers = Enumerable.Range(0, workerCount)
            .Select(_ => Task.Run(WorkerLoop))
            .ToArray();

        // When workers stop, close the completed channel so consumers can finish cleanly.
        _drainTask = Task.WhenAll(_workers).ContinueWith(t =>
        {
            _completed.Writer.TryComplete(t.Exception);
        }, TaskScheduler.Default);
    }

    /// <summary>
    /// Enqueue an item. Returns a Task you can await if you want the per-item result.
    /// (Independently, the result also appears in the Completed queue.)
    /// </summary>
    public Task<TOut> Enqueue(TIn input)
    {
        _shutdown.ThrowIfCancellationRequested();

        int id = Interlocked.Increment(ref _nextId);

        TaskCompletionSource<TOut> tcs = new(TaskCreationOptions.RunContinuationsAsynchronously);
        WorkItem item = new(id, input, tcs);

        // If writer is completed, this will throw.
        if (!_work.Writer.TryWrite(item))
            throw new InvalidOperationException("TaskManager is not accepting more work (CompleteAdding was called).");

        return tcs.Task;
    }

    /// <summary>
    /// Stop accepting new work. Existing queued work will still be processed.
    /// </summary>
    private void CompleteAdding() => _work.Writer.TryComplete();

    /// <summary>
    /// Non-blocking: attempt to read a completion.
    /// </summary>
    public bool TryDequeueCompleted(out Completion completion)
        => _completed.Reader.TryRead(out completion);

    /// <summary>
    /// Async blocking: wait for the next completion.
    /// Throws ChannelClosedException when the manager is drained & disposed/closed.
    /// </summary>
    public ValueTask<Completion> WaitForCompletedAsync(CancellationToken ct = default)
        => _completed.Reader.ReadAsync(ct);

    /// <summary>
    /// Wait until all workers finish (after CompleteAdding or shutdown cancellation).
    /// </summary>
    public Task WhenAllCompletedAsync() => _drainTask;

    private async Task WorkerLoop()
    {
        try
        {
            await foreach (var item in _work.Reader.ReadAllAsync(_shutdown).ConfigureAwait(false))
            {
                try
                {
                    var result = await _processor(item.Input, _shutdown).ConfigureAwait(false);

                    item.Tcs.TrySetResult(result);

                    await _completed.Writer.WriteAsync(
                        new Completion(item.Id, true, result, null),
                        _shutdown).ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    item.Tcs.TrySetException(ex);

                    await _completed.Writer.WriteAsync(
                        new Completion(item.Id, false, default, ex),
                        _shutdown).ConfigureAwait(false);
                }
            }
        }
        catch (OperationCanceledException)
        {
            // shutdown requested: exit workers
        }
    }

    public async ValueTask DisposeAsync()
    {
        CompleteAdding();
        try { await WhenAllCompletedAsync().ConfigureAwait(false); }
        catch { /* surfaced via completions/awaiters; ignore here */ }
    }
}