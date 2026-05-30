namespace CsvProcessor.Poc.Domain;

public sealed class CsvSchema
{
    public CsvSchema(IReadOnlyList<CsvColumn> columns)
    {
        Columns = columns;
    }

    public IReadOnlyList<CsvColumn> Columns { get; }

    public static CsvSchema CreateTeachingSchema()
    {
        // TODO: Move this schema to configuration so the processor can handle multiple CSV contracts.
        return new CsvSchema(new[]
        {
            new CsvColumn("id", Required: true, IsValid: value => long.TryParse(value, out _), Example: "1001"),
            new CsvColumn("name", Required: true, IsValid: value => !string.IsNullOrWhiteSpace(value), Example: "Ada Lovelace"),
            new CsvColumn("email", Required: false, IsValid: value => value is null || value.Contains('@'), Example: "ada@example.com"),
            new CsvColumn("amount", Required: false, IsValid: value => decimal.TryParse(value, out _), Example: "42.50")
        });
    }
}
