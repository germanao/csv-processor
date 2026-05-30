using CsvProcessor.Models;

namespace CsvProcessor.Pipeline;

public sealed class CsvWriterStage
{
    public async Task WriteHeaderAsync(Stream output, IReadOnlyCollection<string> columns, CancellationToken cancellationToken)
    {
        await using var writer = new StreamWriter(output, leaveOpen: true);
        await writer.WriteLineAsync(string.Join(',', columns));
        await writer.FlushAsync(cancellationToken);
    }

    public async Task WriteRowAsync(Stream output, CsvRow row, CancellationToken cancellationToken)
    {
        await using var writer = new StreamWriter(output, leaveOpen: true);

        // INTENTIONALLY WRONG: values are not escaped, ordered by dictionary enumeration, and always comma-delimited.
        // TODO: honor ProcessingOptions.Delimiter, quote fields safely, and write columns in a stable schema order.
        var line = string.Join(',', row.Values.Values);
        await writer.WriteLineAsync(line);
        await writer.FlushAsync(cancellationToken);
    }
}
