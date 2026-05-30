using System.Threading.Channels;

namespace CsvProcessor.Worker;

public sealed record IngestionJob(string BatchId, string CsvText);

public sealed class IngestionQueue
{
    // TODO(WORKER-01): Make capacity configurable and expose backpressure metrics.

    private readonly Channel<IngestionJob> _channel = Channel.CreateBounded<IngestionJob>(new BoundedChannelOptions(100)
    {
        FullMode = BoundedChannelFullMode.Wait,
        SingleReader = true,
        SingleWriter = false
    });

    public ValueTask EnqueueAsync(IngestionJob job, CancellationToken cancellationToken) => _channel.Writer.WriteAsync(job, cancellationToken);

    public IAsyncEnumerable<IngestionJob> ReadAllAsync(CancellationToken cancellationToken) => _channel.Reader.ReadAllAsync(cancellationToken);
}
