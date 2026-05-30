namespace CsvProcessor.Models;

public sealed record RowProcessingResult(
    CsvRow? Row,
    bool IsValid,
    IReadOnlyList<string> Errors)
{
    public static RowProcessingResult Accepted(CsvRow row) => new(row, true, Array.Empty<string>());

    public static RowProcessingResult Rejected(CsvRow row, params string[] errors) => new(row, false, errors);
}
