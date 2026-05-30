namespace CsvProcessor.Poc.Domain;

public sealed record CsvProcessingError(
    long RowNumber,
    string Code,
    string Message,
    string? ColumnName = null,
    string? RawValue = null);
