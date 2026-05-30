using System.Globalization;
using CsvProcessor.Core.Models;

namespace CsvProcessor.Core.Validation;

public sealed class CustomerRowMapper
{
    public RowMappingResult Map(RawCsvRow row)
    {
        // TODO(MAP-01): Normalize all text fields before validation.
        // TODO(MAP-02): Validate required fields and accumulate every failure in a deterministic order.
        // TODO(MAP-03): Parse DateOnly and DateTimeOffset using CultureInfo.InvariantCulture.
        // TODO(MAP-04): Parse balances with clear currency/decimal rules.
        // TODO(MAP-05): Map legacy statuses like A/I/S/1/0 into CustomerStatus.
        // TODO(MAP-06): Generate a stable SourceRowHash from canonical source values.
        var id = row.Get("customer_id").Trim();
        var email = row.Get("email").Trim().ToLowerInvariant();

        if (string.IsNullOrWhiteSpace(id) || string.IsNullOrWhiteSpace(email))
        {
            return RowMappingResult.Invalid(new InvalidRow(row.RowNumber, "customer_id and email are required", row.Values));
        }

        // MOCK/INCOMPLETE ON PURPOSE:
        // - Email validation only checks for '@'.
        // - Missing/invalid dates silently become MinValue/Unix epoch.
        // - Balance parsing failures become 0.
        // - Status values other than "active" become Unknown but are not rejected.
        // Complete the TODOs above and turn each bad value into an InvalidRow reason.
        var fullName = row.Get("full_name").Trim();
        _ = DateOnly.TryParse(row.Get("date_of_birth"), CultureInfo.InvariantCulture, DateTimeStyles.None, out var dateOfBirth);
        _ = decimal.TryParse(row.Get("balance"), NumberStyles.Number, CultureInfo.InvariantCulture, out var balance);
        _ = DateTimeOffset.TryParse(row.Get("updated_at"), CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal, out var updatedAt);
        var status = row.Get("status").Equals("active", StringComparison.OrdinalIgnoreCase) ? CustomerStatus.Active : CustomerStatus.Unknown;

        if (!email.Contains('@', StringComparison.Ordinal))
        {
            return RowMappingResult.Invalid(new InvalidRow(row.RowNumber, "email is invalid", row.Values));
        }

        var record = new CustomerRecord(id, email, fullName, dateOfBirth, balance, status, updatedAt.ToUniversalTime(), "TODO-source-row-hash");
        return RowMappingResult.Valid(record, row.RowNumber);
    }
}

public sealed record RowMappingResult(CustomerRecord? Record, InvalidRow? InvalidRow, int RowNumber)
{
    public bool IsValid => Record is not null;
    public static RowMappingResult Valid(CustomerRecord record, int rowNumber) => new(record, null, rowNumber);
    public static RowMappingResult Invalid(InvalidRow invalidRow) => new(null, invalidRow, invalidRow.RowNumber);
}
