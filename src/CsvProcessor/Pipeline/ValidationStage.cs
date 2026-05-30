using CsvProcessor.Models;

namespace CsvProcessor.Pipeline;

public sealed class ValidationStage
{
    public RowProcessingResult Validate(CsvRow row)
    {
        var errors = new List<string>();

        // Mock rule: pretend every dataset must contain an Id column.
        // TODO: Load rules from schema metadata instead of hard-coding domain assumptions.
        if (!row.Values.TryGetValue("Id", out var id) || string.IsNullOrWhiteSpace(id))
        {
            errors.Add("Missing Id.");
        }

        // Intentionally incomplete: this accepts non-numeric IDs after checking only for whitespace.
        // TODO: parse with invariant culture, validate uniqueness, and report exact column/row coordinates.

        return errors.Count == 0
            ? RowProcessingResult.Accepted(row)
            : RowProcessingResult.Rejected(row, errors.ToArray());
    }
}
