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
        var expected = expectedColumns.Select(Normalize).ToHashSet(StringComparer.OrdinalIgnoreCase);
        var observed = observedColumns.Select(Normalize).ToHashSet(StringComparer.OrdinalIgnoreCase);

        var missing = expected.Except(observed, StringComparer.OrdinalIgnoreCase).Order(StringComparer.OrdinalIgnoreCase).ToArray();
        var unexpected = observed.Except(expected, StringComparer.OrdinalIgnoreCase).Order(StringComparer.OrdinalIgnoreCase).ToArray();
        var possibleRenames = missing
            .SelectMany(missingColumn => unexpected.Select(unexpectedColumn => new { missingColumn, unexpectedColumn, Score = Similarity(missingColumn, unexpectedColumn) }))
            .Where(candidate => candidate.Score >= 0.55)
            .GroupBy(candidate => candidate.missingColumn)
            .ToDictionary(group => group.Key, group => group.OrderByDescending(candidate => candidate.Score).First().unexpectedColumn, StringComparer.OrdinalIgnoreCase);

        return new SchemaDriftReport(missing, unexpected, possibleRenames);
    }

    private static string Normalize(string value) => value.Trim().Replace(" ", "_", StringComparison.Ordinal).ToLowerInvariant();

    private static double Similarity(string left, string right)
    {
        var leftTokens = left.Split('_', StringSplitOptions.RemoveEmptyEntries).ToHashSet(StringComparer.OrdinalIgnoreCase);
        var rightTokens = right.Split('_', StringSplitOptions.RemoveEmptyEntries).ToHashSet(StringComparer.OrdinalIgnoreCase);
        var unionCount = leftTokens.Union(rightTokens, StringComparer.OrdinalIgnoreCase).Count();
        return unionCount == 0 ? 0 : leftTokens.Intersect(rightTokens, StringComparer.OrdinalIgnoreCase).Count() / (double)unionCount;
    }
}
