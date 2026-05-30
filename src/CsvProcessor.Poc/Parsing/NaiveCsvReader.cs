using System.Runtime.CompilerServices;
using CsvProcessor.Poc.Domain;

namespace CsvProcessor.Poc.Parsing;

public sealed class NaiveCsvReader : ICsvReader
{
    public async IAsyncEnumerable<CsvRecord> ReadAsync(
        string path,
        [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        // FIXME: This parser is intentionally wrong for real CSV files.
        // It ignores quoted delimiters, escaped quotes, embedded newlines, BOMs, custom encodings, and comments.
        // Replace it with a state-machine parser or a well-understood library after writing failing tests.
        using var reader = File.OpenText(path);
        var headerLine = await reader.ReadLineAsync(cancellationToken);
        if (headerLine is null)
        {
            yield break;
        }

        var headers = headerLine.Split(',');
        long rowNumber = 1;

        while (!reader.EndOfStream)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var line = await reader.ReadLineAsync(cancellationToken);
            rowNumber++;

            if (string.IsNullOrWhiteSpace(line))
            {
                // TODO: Decide whether blank lines are ignored, reported, or emitted as empty records.
                continue;
            }

            var values = line.Split(',');
            var record = new Dictionary<string, string?>(StringComparer.OrdinalIgnoreCase);

            for (var i = 0; i < headers.Length; i++)
            {
                // BUG FOR LEARNING: Missing columns are silently treated as null and extra columns are discarded.
                // Add row-width validation in either the parser or validator.
                record[headers[i]] = i < values.Length ? values[i] : null;
            }

            yield return new CsvRecord(rowNumber, record);
        }
    }
}
