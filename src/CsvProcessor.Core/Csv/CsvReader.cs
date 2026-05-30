using System.Text;
using CsvProcessor.Core.Models;

namespace CsvProcessor.Core.Csv;

public sealed class CsvReader
{
    public IReadOnlyList<RawCsvRow> Read(string csvText)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(csvText);

        var rows = SplitRecords(csvText).Where(row => !string.IsNullOrWhiteSpace(row)).ToList();
        if (rows.Count == 0)
        {
            return [];
        }

        var headers = SplitFields(rows[0]).Select(NormalizeHeader).ToArray();
        var result = new List<RawCsvRow>(rows.Count - 1);

        for (var rowIndex = 1; rowIndex < rows.Count; rowIndex++)
        {
            var fields = SplitFields(rows[rowIndex]);
            var values = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            for (var columnIndex = 0; columnIndex < headers.Length; columnIndex++)
            {
                values[headers[columnIndex]] = columnIndex < fields.Count ? fields[columnIndex].Trim() : string.Empty;
            }

            result.Add(new RawCsvRow(rowIndex + 1, values));
        }

        return result;
    }

    public IReadOnlyList<string> ReadHeader(string csvText)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(csvText);
        var firstRecord = SplitRecords(csvText).FirstOrDefault() ?? string.Empty;
        return SplitFields(firstRecord).Select(NormalizeHeader).ToArray();
    }

    private static string NormalizeHeader(string header) => header.Trim().Replace(" ", "_", StringComparison.Ordinal).ToLowerInvariant();

    private static IReadOnlyList<string> SplitRecords(string text)
    {
        var records = new List<string>();
        var current = new StringBuilder();
        var inQuotes = false;

        foreach (var character in text.Replace("\r\n", "\n", StringComparison.Ordinal).Replace('\r', '\n'))
        {
            if (character == '"')
            {
                inQuotes = !inQuotes;
            }

            if (character == '\n' && !inQuotes)
            {
                records.Add(current.ToString());
                current.Clear();
                continue;
            }

            current.Append(character);
        }

        if (current.Length > 0)
        {
            records.Add(current.ToString());
        }

        return records;
    }

    private static IReadOnlyList<string> SplitFields(string record)
    {
        var fields = new List<string>();
        var current = new StringBuilder();
        var inQuotes = false;

        for (var index = 0; index < record.Length; index++)
        {
            var character = record[index];
            if (character == '"')
            {
                if (inQuotes && index + 1 < record.Length && record[index + 1] == '"')
                {
                    current.Append('"');
                    index++;
                    continue;
                }

                inQuotes = !inQuotes;
                continue;
            }

            if (character == ',' && !inQuotes)
            {
                fields.Add(current.ToString());
                current.Clear();
                continue;
            }

            current.Append(character);
        }

        fields.Add(current.ToString());
        return fields;
    }
}
