namespace CsvProcessor.Models;

/// <summary>
/// Represents one parsed CSV row. Values are stored by header name when a header exists.
/// TODO: Preserve original byte offset and raw text so invalid rows can be audited and replayed.
/// </summary>
public sealed record CsvRow(
    long RowNumber,
    IReadOnlyDictionary<string, string> Values,
    IReadOnlyList<string> RawFields);
