namespace CsvProcessor.Core.Processing;

public interface IIdempotencyStore
{
    bool TryStart(string batchId);
    void Complete(string batchId);
}

public sealed class InMemoryIdempotencyStore : IIdempotencyStore
{
    public bool TryStart(string batchId)
    {
        // TODO(IDEMPOTENCY-01): Store started/completed batch IDs in a thread-safe collection.
        // TODO(IDEMPOTENCY-02): Decide how failed batches can be retried without allowing concurrent duplicates.
        // MOCK/WRONG ON PURPOSE: every batch is accepted, including replays.
        return true;
    }

    public void Complete(string batchId)
    {
        // TODO(IDEMPOTENCY-03): Mark the batch as completed and persist that state in production.
    }
}
