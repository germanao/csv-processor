using CsvProcessor.Poc.Domain;

namespace CsvProcessor.Poc.Validation;

public sealed class CsvValidator
{
    private readonly CsvSchema _schema;

    public CsvValidator(CsvSchema schema)
    {
        _schema = schema;
    }

    public IReadOnlyList<CsvProcessingError> Validate(CsvRecord record)
    {
        var errors = new List<CsvProcessingError>();

        foreach (var column in _schema.Columns)
        {
            var value = record.Get(column.Name);

            if (column.Required && string.IsNullOrWhiteSpace(value))
            {
                errors.Add(new CsvProcessingError(
                    record.RowNumber,
                    "required_column_missing",
                    $"Column '{column.Name}' is required.",
                    column.Name,
                    value));

                continue;
            }

            // INCOMPLETE: Optional blank values should probably bypass format validators,
            // but this mock calls validators for every non-null value and ignores whitespace-only subtleties.
            if (value is not null && column.IsValid is not null && !column.IsValid(value))
            {
                errors.Add(new CsvProcessingError(
                    record.RowNumber,
                    "invalid_column_value",
                    $"Column '{column.Name}' does not match the expected shape.",
                    column.Name,
                    value));
            }
        }

        // TODO: Report unknown columns, duplicate headers, missing headers, and row-width mismatches.
        return errors;
    }
}
