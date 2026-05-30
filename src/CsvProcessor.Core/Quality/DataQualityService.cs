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
        var invalidReasonCounts = invalidRows
            .SelectMany(row => row.Reason.Split(';', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries))
            .GroupBy(reason => reason, StringComparer.OrdinalIgnoreCase)
            .ToDictionary(group => group.Key, group => group.Count(), StringComparer.OrdinalIgnoreCase);

        var blankColumnCounts = rawRows
            .SelectMany(row => row.Values.Where(pair => string.IsNullOrWhiteSpace(pair.Value)).Select(pair => pair.Key))
            .GroupBy(column => column, StringComparer.OrdinalIgnoreCase)
            .ToDictionary(group => group.Key, group => group.Count(), StringComparer.OrdinalIgnoreCase);

        return new DataQualitySummary(rawRows.Count, accepted.Count, invalidRows.Count, duplicateRows.Count, invalidReasonCounts, blankColumnCounts);
    }
}
