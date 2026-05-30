using CsvProcessor.Core.Csv;
using CsvProcessor.Core.Models;
using CsvProcessor.Core.Quality;
using CsvProcessor.Core.Validation;

namespace CsvProcessor.Core.Processing;

public sealed class CsvCustomerBatchProcessor(
    CsvReader csvReader,
    SchemaDriftDetector schemaDriftDetector,
    CustomerRowMapper mapper,
    CustomerDedupeService dedupeService,
    DataQualityService qualityService,
    WatermarkProcessor watermarkProcessor)
{
    public static readonly string[] ExpectedColumns = ["customer_id", "email", "full_name", "date_of_birth", "balance", "status", "updated_at"];

    public ProcessingResult Process(string csvText, DateTimeOffset? previousWatermark = null)
    {
        // TODO(PIPELINE-01): Keep this orchestration readable: parse -> drift -> map -> dedupe -> watermark -> quality.
        // TODO(PIPELINE-02): Decide which failures abort the whole batch and which failures quarantine one row.
        // TODO(PIPELINE-03): Add telemetry/logging boundaries around each stage in real systems.
        var headers = csvReader.ReadHeader(csvText);
        var drift = schemaDriftDetector.Compare(ExpectedColumns, headers);
        var rawRows = csvReader.Read(csvText);

        if (drift.HasBreakingDrift)
        {
            var invalidRows = rawRows.Select(row => new InvalidRow(row.RowNumber, $"schema drift: missing {string.Join(", ", drift.MissingColumns)}", row.Values)).ToArray();
            var emptyQuality = qualityService.BuildSummary(rawRows, [], invalidRows, []);
            return new ProcessingResult([], invalidRows, [], emptyQuality, previousWatermark);
        }

        var validRows = new List<RowWithSource>();
        var invalid = new List<InvalidRow>();

        foreach (var rawRow in rawRows)
        {
            var mapped = mapper.Map(rawRow);
            if (mapped.Record is null)
            {
                invalid.Add(mapped.InvalidRow!);
            }
            else
            {
                validRows.Add(new RowWithSource(mapped.Record, mapped.RowNumber));
            }
        }

        var deduped = dedupeService.Deduplicate(validRows);
        var incrementalRecords = watermarkProcessor.FilterNewerThan(deduped.Records, previousWatermark);
        var nextWatermark = watermarkProcessor.CalculateNextWatermark(incrementalRecords, previousWatermark);
        var quality = qualityService.BuildSummary(rawRows, incrementalRecords, invalid, deduped.Duplicates);

        return new ProcessingResult(incrementalRecords, invalid, deduped.Duplicates, quality, nextWatermark);
    }

    public static CsvCustomerBatchProcessor CreateDefault() => new(
        new CsvReader(),
        new SchemaDriftDetector(),
        new CustomerRowMapper(),
        new CustomerDedupeService(),
        new DataQualityService(),
        new WatermarkProcessor());
}
