using CsvProcessor.Core.Processing;

namespace CsvProcessor.Worker;

public sealed class Worker(IngestionQueue queue, CsvCustomerBatchProcessor processor, IIdempotencyStore idempotencyStore, ILogger<Worker> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await foreach (var job in queue.ReadAllAsync(stoppingToken).ConfigureAwait(false))
        {
            // TODO(WORKER-02): Add try/catch per job, retry policy, poison-message handling, and durable state.

            if (!idempotencyStore.TryStart(job.BatchId))
            {
                logger.LogInformation("Skipping duplicate batch {BatchId}", job.BatchId);
                continue;
            }

            var result = processor.Process(job.CsvText);
            idempotencyStore.Complete(job.BatchId);
            logger.LogInformation("Processed batch {BatchId}: {Accepted} accepted, {Invalid} invalid, {Duplicates} duplicates", job.BatchId, result.Accepted.Count, result.InvalidRows.Count, result.DuplicateRows.Count);
        }
    }
}
