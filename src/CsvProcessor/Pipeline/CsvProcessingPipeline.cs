using System.Diagnostics;
using CsvProcessor.Infrastructure;
using CsvProcessor.Models;

namespace CsvProcessor.Pipeline;

public sealed class CsvProcessingPipeline(
    ICsvSource source,
    CsvReaderStage reader,
    ValidationStage validator,
    TransformationStage transformer,
    CsvWriterStage writer)
{
    public async Task<ProcessingSummary> RunAsync(ProcessingOptions options, CancellationToken cancellationToken)
    {
        var stopwatch = Stopwatch.StartNew();
        var rowsRead = 0L;
        var rowsWritten = 0L;
        var rowsRejected = 0L;
        var wroteHeader = false;

        await using var input = source.OpenRead(options.InputPath);
        await using var output = source.OpenWrite(options.OutputPath);

        await foreach (var row in reader.ReadAsync(input, options, cancellationToken))
        {
            rowsRead++;

            var validation = validator.Validate(row);
            if (!validation.IsValid || validation.Row is null)
            {
                rowsRejected++;
                // TODO: Write rejected rows and validation.Errors to a quarantine file instead of dropping them.
                continue;
            }

            var transformed = transformer.Transform(validation.Row);

            if (!wroteHeader)
            {
                await writer.WriteHeaderAsync(output, transformed.Values.Keys.ToArray(), cancellationToken);
                wroteHeader = true;
            }

            await writer.WriteRowAsync(output, transformed, cancellationToken);
            rowsWritten++;

            // TODO: Batch writes according to options.BatchSize instead of flushing every row.
            // TODO: Add progress reporting and structured logs for long-running files.
        }

        stopwatch.Stop();
        return new ProcessingSummary(rowsRead, rowsWritten, rowsRejected, stopwatch.Elapsed);
    }
}
