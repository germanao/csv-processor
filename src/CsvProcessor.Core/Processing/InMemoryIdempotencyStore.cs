using System.Collections.Concurrent;

namespace CsvProcessor.Core.Processing;

public interface IIdempotencyStore
{
    bool TryStart(string batchId);
    void Complete(string batchId);
}

public sealed class InMemoryIdempotencyStore : IIdempotencyStore
{
    private readonly ConcurrentDictionary<string, byte> _startedBatches = new(StringComparer.OrdinalIgnoreCase);

    public bool TryStart(string batchId) => _startedBatches.TryAdd(batchId, 0);

    public void Complete(string batchId) => _startedBatches[batchId] = 1;
}
