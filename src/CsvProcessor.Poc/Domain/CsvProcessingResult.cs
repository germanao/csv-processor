namespace CsvProcessor.Poc.Domain;

public sealed class CsvProcessingResult
{
    public long RowsRead { get; set; }

    public long RowsWritten { get; set; }

    public List<CsvProcessingError> Errors { get; } = new();
}
