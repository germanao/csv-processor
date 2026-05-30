using CsvProcessor.Models;

namespace CsvProcessor.Pipeline;

public sealed class TransformationStage
{
    public CsvRow Transform(CsvRow row)
    {
        // Mock transformation: trim only mapped values, not RawFields.
        // TODO: Make transformations explicit, ordered, testable, culture-aware, and reversible when possible.
        var normalized = row.Values.ToDictionary(
            pair => pair.Key.Trim(),
            pair => pair.Value.Trim(),
            StringComparer.OrdinalIgnoreCase);

        // Deliberately simplistic derived column example.
        // TODO: Decide whether generated columns belong in the same output file or a downstream projection model.
        normalized["ProcessedAtUtc"] = DateTime.UtcNow.ToString("O");

        return row with { Values = normalized };
    }
}
