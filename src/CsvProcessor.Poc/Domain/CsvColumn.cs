namespace CsvProcessor.Poc.Domain;

public sealed record CsvColumn(
    string Name,
    bool Required,
    Func<string?, bool>? IsValid = null,
    string? Example = null);
