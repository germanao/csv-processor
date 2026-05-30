namespace CsvProcessor.Core.Models;

public sealed record RawCsvRow(int RowNumber, IReadOnlyDictionary<string, string> Values)
{
    public string Get(string column) => Values.TryGetValue(column, out var value) ? value : string.Empty;
}
