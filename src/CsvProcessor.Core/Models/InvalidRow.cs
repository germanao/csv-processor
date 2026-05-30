namespace CsvProcessor.Core.Models;

public sealed record InvalidRow(int RowNumber, string Reason, IReadOnlyDictionary<string, string> Values);
