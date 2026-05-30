using CsvProcessor.Core.Models;

namespace CsvProcessor.Core.Csv;

public sealed class CsvReader
{
    public IReadOnlyList<RawCsvRow> Read(string csvText)
    {
        // TODO(CSV-01): Validate null/empty input and decide whether this layer throws or returns an empty batch.
        // TODO(CSV-02): Replace this naive implementation with a real CSV parser.
        // Learning targets:
        // - Preserve newlines inside quoted fields.
        // - Support escaped quotes: "" inside a quoted field should become ".
        // - Keep source row numbers aligned with the original file for quarantine reports.
        // - Decide how to handle rows with too many/few columns.
        var lines = csvText.Split('\n', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        if (lines.Length <= 1)
        {
            return [];
        }

        var headers = SplitFieldsNaively(lines[0]).Select(NormalizeHeader).ToArray();
        var rows = new List<RawCsvRow>();

        for (var lineIndex = 1; lineIndex < lines.Length; lineIndex++)
        {
            var fields = SplitFieldsNaively(lines[lineIndex]);
            var values = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            for (var columnIndex = 0; columnIndex < headers.Length; columnIndex++)
            {
                // TODO(CSV-03): Record missing fields as schema/row-shape problems instead of silently filling blanks.
                values[headers[columnIndex]] = columnIndex < fields.Length ? fields[columnIndex].Trim() : string.Empty;
            }

            rows.Add(new RawCsvRow(lineIndex + 1, values));
        }

        return rows;
    }

    public IReadOnlyList<string> ReadHeader(string csvText)
    {
        // TODO(CSV-04): Reuse the real record splitter after CSV-02 is complete.
        var firstLine = csvText.Split('\n', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).FirstOrDefault() ?? string.Empty;
        return SplitFieldsNaively(firstLine).Select(NormalizeHeader).ToArray();
    }

    private static string NormalizeHeader(string header)
    {
        // TODO(CSV-05): Centralize normalization so schema drift and row mapping use the exact same rule.
        return header.Trim().Replace(" ", "_", StringComparison.Ordinal).ToLowerInvariant();
    }

    private static string[] SplitFieldsNaively(string line)
    {
        // MOCK/WRONG ON PURPOSE: comma splitting is not a compliant CSV parser.
        // Complete this only after writing tests for quoted commas, quoted newlines, and escaped quotes.
        return line.Split(',');
    }
}
