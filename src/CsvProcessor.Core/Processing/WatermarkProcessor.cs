using CsvProcessor.Core.Models;

namespace CsvProcessor.Core.Processing;

public sealed class WatermarkProcessor
{
    public IReadOnlyList<CustomerRecord> FilterNewerThan(IReadOnlyList<CustomerRecord> records, DateTimeOffset? previousWatermark)
    {
        // TODO(WATERMARK-01): When previousWatermark is present, return only records with UpdatedAt > previousWatermark.
        // TODO(WATERMARK-02): Decide how to handle records equal to the watermark to avoid replay duplicates.
        // MOCK/WRONG ON PURPOSE: currently ignores the watermark.
        return records;
    }

    public DateTimeOffset? CalculateNextWatermark(IReadOnlyList<CustomerRecord> processedRecords, DateTimeOffset? previousWatermark)
    {
        // TODO(WATERMARK-03): Return max(processed UpdatedAt, previousWatermark) without moving backwards.
        // MOCK/INCOMPLETE: returns the previous watermark even when newer records exist.
        return previousWatermark;
    }
}
