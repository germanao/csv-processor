using System.Collections.Generic;
using System.Runtime.CompilerServices;
using CsvProcessor.Models;

namespace CsvProcessor.Pipeline;

public sealed class CsvReaderStage
{
    public async IAsyncEnumerable<CsvRow> ReadAsync(
        Stream input,
        ProcessingOptions options,
        [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        using var reader = new StreamReader(input);

        string[]? headers = null;
        var rowNumber = 0L;

        while (!reader.EndOfStream)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var line = await reader.ReadLineAsync(cancellationToken);
            rowNumber++;

            if (line is null)
            {
                continue;
            }

            var fields = SplitLineNaively(line, options.Delimiter);

            if (rowNumber == 1 && options.HasHeader)
            {
                headers = fields;
                continue;
            }

            // TODO: Handle duplicate headers and rows with more/less fields than the header count.
            var values = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            for (var index = 0; index < fields.Length; index++)
            {
                var columnName = headers is not null && index < headers.Length
                    ? headers[index]
                    : $"Column{index + 1}";

                values[columnName] = fields[index];
            }

            yield return new CsvRow(rowNumber, values, fields);
        }
    }

    private static string[] SplitLineNaively(string line, char delimiter)
    {
        // INTENTIONALLY WRONG: this breaks for quoted delimiters, escaped quotes, and embedded newlines.
        // Exercise: implement a state machine or compare behavior against CsvHelper/TextFieldParser.
        return line.Split(delimiter);
    }
}
