namespace CsvProcessor.Pipeline;

public sealed record ProcessingSummary(
    long RowsRead,
    long RowsWritten,
    long RowsRejected,
    TimeSpan Elapsed)
{
    public static ProcessingSummary Empty => new(0, 0, 0, TimeSpan.Zero);
}
