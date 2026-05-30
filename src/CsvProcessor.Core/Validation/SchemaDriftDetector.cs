namespace CsvProcessor.Core.Validation;

public sealed record SchemaDriftReport(
    IReadOnlyList<string> MissingColumns,
    IReadOnlyList<string> UnexpectedColumns,
    IReadOnlyDictionary<string, string> PossibleRenames)
{
    public bool HasBreakingDrift => MissingColumns.Count > 0;
}

public sealed class SchemaDriftDetector
{
    public SchemaDriftReport Compare(IEnumerable<string> expectedColumns, IEnumerable<string> observedColumns)
    {
        // TODO(SCHEMA-01): Normalize with the same function used by CsvReader.
        // TODO(SCHEMA-02): Report both missing and unexpected columns.
        // TODO(SCHEMA-03): Add possible rename hints, for example full_name -> name.
        // Current mock only detects missing columns and intentionally ignores unexpected columns.
        var expected = expectedColumns.Select(Normalize).ToHashSet(StringComparer.OrdinalIgnoreCase);
        var observed = observedColumns.Select(Normalize).ToHashSet(StringComparer.OrdinalIgnoreCase);
        var missing = expected.Except(observed, StringComparer.OrdinalIgnoreCase).Order(StringComparer.OrdinalIgnoreCase).ToArray();

        return new SchemaDriftReport(missing, [], new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase));
    }

    private static string Normalize(string value) => value.Trim().Replace(" ", "_", StringComparison.Ordinal).ToLowerInvariant();
}
