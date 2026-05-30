using CsvProcessor.Core.Models;

namespace CsvProcessor.Core.Quality;

public sealed class DataQualityService
{
    public DataQualitySummary BuildSummary(
        IReadOnlyList<RawCsvRow> rawRows,
        IReadOnlyList<CustomerRecord> accepted,
        IReadOnlyList<InvalidRow> invalidRows,
        IReadOnlyList<DuplicateRow> duplicateRows)
    {
        // TODO(QUALITY-01): Split semicolon-delimited invalid reasons and count each reason case-insensitively.
        // TODO(QUALITY-02): Count null/blank values per source column.
        // TODO(QUALITY-03): Decide whether accepted count is before or after watermark filtering and document it.
        // MOCK/INCOMPLETE: top-level counts are real, but metric dictionaries are empty.
        return new DataQualitySummary(
            rawRows.Count,
            accepted.Count,
            invalidRows.Count,
            duplicateRows.Count,
            new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase),
            new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase));
    }
}
