using CsvProcessor.Poc.Domain;

namespace CsvProcessor.Poc.Writing;

public sealed class CsvWriter
{
    public async Task WriteAsync(
        string path,
        IReadOnlyList<CsvRecord> records,
        CancellationToken cancellationToken)
    {
        // TODO: Avoid buffering all transformed records in memory; stream rows to disk as they are processed.
        Directory.CreateDirectory(Path.GetDirectoryName(path) ?? ".");

        await using var stream = File.CreateText(path);

        if (records.Count == 0)
        {
            return;
        }

        var headers = records[0].Values.Keys.ToArray();
        await stream.WriteLineAsync(string.Join(',', headers));

        foreach (var record in records)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var line = string.Join(',', headers.Select(header => Escape(record.Get(header))));
            await stream.WriteLineAsync(line);
        }
    }

    private static string Escape(string? value)
    {
        if (value is null)
        {
            return string.Empty;
        }

        // FIXME: This is intentionally incomplete. It quotes values containing commas but forgets quotes and newlines.
        // Correct CSV escaping doubles embedded quotes and quotes fields containing delimiters, quotes, or line breaks.
        return value.Contains(',') ? $"\"{value}\"" : value;
    }
}
