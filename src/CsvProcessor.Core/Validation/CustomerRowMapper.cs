using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using CsvProcessor.Core.Models;

namespace CsvProcessor.Core.Validation;

public sealed class CustomerRowMapper
{
    public RowMappingResult Map(RawCsvRow row)
    {
        var reasons = new List<string>();
        var id = NormalizeId(row.Get("customer_id"));
        var email = NormalizeEmail(row.Get("email"));
        var fullName = NormalizeName(row.Get("full_name"));
        var status = ParseStatus(row.Get("status"));

        if (string.IsNullOrWhiteSpace(id)) reasons.Add("customer_id is required");
        if (string.IsNullOrWhiteSpace(email) || !email.Contains('@', StringComparison.Ordinal)) reasons.Add("email is invalid");
        if (string.IsNullOrWhiteSpace(fullName)) reasons.Add("full_name is required");
        if (!DateOnly.TryParse(row.Get("date_of_birth"), CultureInfo.InvariantCulture, DateTimeStyles.None, out var dateOfBirth)) reasons.Add("date_of_birth is invalid");
        if (!decimal.TryParse(row.Get("balance"), NumberStyles.Number | NumberStyles.AllowCurrencySymbol, CultureInfo.InvariantCulture, out var balance)) reasons.Add("balance is invalid");
        if (status == CustomerStatus.Unknown) reasons.Add("status is invalid");
        if (!DateTimeOffset.TryParse(row.Get("updated_at"), CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal, out var updatedAt)) reasons.Add("updated_at is invalid");

        if (reasons.Count > 0)
        {
            return RowMappingResult.Invalid(new InvalidRow(row.RowNumber, string.Join("; ", reasons), row.Values));
        }

        var record = new CustomerRecord(id, email, fullName, dateOfBirth, balance, status, updatedAt.ToUniversalTime(), Hash(row.Values));
        return RowMappingResult.Valid(record, row.RowNumber);
    }

    private static string NormalizeId(string value) => value.Trim().ToUpperInvariant();
    private static string NormalizeEmail(string value) => value.Trim().ToLowerInvariant();
    private static string NormalizeName(string value) => string.Join(' ', value.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries));

    private static CustomerStatus ParseStatus(string value) => value.Trim().ToLowerInvariant() switch
    {
        "active" or "a" or "1" => CustomerStatus.Active,
        "inactive" or "i" or "0" => CustomerStatus.Inactive,
        "suspended" or "s" => CustomerStatus.Suspended,
        _ => CustomerStatus.Unknown
    };

    private static string Hash(IReadOnlyDictionary<string, string> values)
    {
        var canonical = string.Join('|', values.OrderBy(pair => pair.Key, StringComparer.OrdinalIgnoreCase).Select(pair => $"{pair.Key}={pair.Value.Trim()}"));
        return Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(canonical)));
    }
}

public sealed record RowMappingResult(CustomerRecord? Record, InvalidRow? InvalidRow, int RowNumber)
{
    public bool IsValid => Record is not null;
    public static RowMappingResult Valid(CustomerRecord record, int rowNumber) => new(record, null, rowNumber);
    public static RowMappingResult Invalid(InvalidRow invalidRow) => new(null, invalidRow, invalidRow.RowNumber);
}
