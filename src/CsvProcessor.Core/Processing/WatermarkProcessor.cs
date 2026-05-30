using CsvProcessor.Core.Models;

namespace CsvProcessor.Core.Processing;

public sealed class WatermarkProcessor
{
    public IReadOnlyList<CustomerRecord> FilterNewerThan(IReadOnlyList<CustomerRecord> records, DateTimeOffset? previousWatermark)
    {
        if (previousWatermark is null)
        {
            return records;
        }

        return records.Where(record => record.UpdatedAt > previousWatermark.Value).ToArray();
    }

    public DateTimeOffset? CalculateNextWatermark(IReadOnlyList<CustomerRecord> processedRecords, DateTimeOffset? previousWatermark)
    {
        var batchMax = processedRecords.Count == 0 ? null : processedRecords.Max(record => (DateTimeOffset?)record.UpdatedAt);
        return batchMax is null || batchMax < previousWatermark ? previousWatermark : batchMax;
    }
}
