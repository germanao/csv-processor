using CsvProcessor.Poc.Domain;

namespace CsvProcessor.Poc.Transformation;

public sealed class CsvTransformer
{
    public CsvRecord Transform(CsvRecord record)
    {
        // TODO: Replace this hard-coded normalization with composable transformation rules.
        // Ideas: trim strings, normalize casing, parse decimals with CultureInfo, derive new columns, redact PII.
        var transformed = new Dictionary<string, string?>(record.Values, StringComparer.OrdinalIgnoreCase);

        if (transformed.TryGetValue("email", out var email) && email is not null)
        {
            // DELIBERATELY INCOMPLETE: This lowercases but does not trim, validate domains, or handle Unicode addresses.
            transformed["email"] = email.ToLowerInvariant();
        }

        if (transformed.TryGetValue("amount", out var amount) && amount is not null)
        {
            // BUG FOR LEARNING: This blindly replaces commas, which breaks cultures where comma is a decimal separator.
            transformed["amount"] = amount.Replace(",", string.Empty);
        }

        return new CsvRecord(record.RowNumber, transformed);
    }
}
