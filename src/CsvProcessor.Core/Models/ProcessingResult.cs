namespace CsvProcessor.Core.Models;

public sealed record ProcessingResult(
    IReadOnlyList<CustomerRecord> Accepted,
    IReadOnlyList<InvalidRow> InvalidRows,
    IReadOnlyList<DuplicateRow> DuplicateRows,
    DataQualitySummary QualitySummary,
    DateTimeOffset? NextWatermark);

public sealed record DataQualitySummary(
    int TotalRows,
    int AcceptedRows,
    int InvalidRows,
    int DuplicateRows,
    IReadOnlyDictionary<string, int> InvalidReasonCounts,
    IReadOnlyDictionary<string, int> NullOrBlankColumnCounts);
