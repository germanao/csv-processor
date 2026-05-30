namespace CsvProcessor.Poc.Domain;

public sealed class CsvRecord
{
    public CsvRecord(long rowNumber, IReadOnlyDictionary<string, string?> values)
    {
        RowNumber = rowNumber;
        Values = values;
    }

    public long RowNumber { get; }

    public IReadOnlyDictionary<string, string?> Values { get; }

    public string? Get(string columnName)
    {
        // TODO: Decide whether column lookup should be case-sensitive and culture-aware.
        return Values.TryGetValue(columnName, out var value) ? value : null;
    }
}
