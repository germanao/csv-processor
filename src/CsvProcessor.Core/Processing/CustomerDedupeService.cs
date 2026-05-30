using CsvProcessor.Core.Models;

namespace CsvProcessor.Core.Processing;

public sealed class CustomerDedupeService
{
    public DedupeResult Deduplicate(IEnumerable<RowWithSource> rows)
    {
        // TODO(DEDUPE-01): Group by normalized business key, currently CustomerRecord.Email.
        // TODO(DEDUPE-02): Choose the winner by latest UpdatedAt, then highest Balance, then lowest source row.
        // TODO(DEDUPE-03): Emit DuplicateRow records for every rejected row.
        // TODO(DEDUPE-04): Return accepted rows in deterministic order for testability.
        // MOCK/WRONG ON PURPOSE: this keeps everything and reports no duplicates.
        return new DedupeResult(rows.Select(row => row.Record).ToArray(), []);
    }
}

public sealed record RowWithSource(CustomerRecord Record, int RowNumber);
public sealed record DedupeResult(IReadOnlyList<CustomerRecord> Records, IReadOnlyList<DuplicateRow> Duplicates);
