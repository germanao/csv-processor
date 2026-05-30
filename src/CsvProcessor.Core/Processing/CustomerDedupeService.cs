using CsvProcessor.Core.Models;

namespace CsvProcessor.Core.Processing;

public sealed class CustomerDedupeService
{
    public DedupeResult Deduplicate(IEnumerable<RowWithSource> rows)
    {
        var kept = new Dictionary<string, RowWithSource>(StringComparer.OrdinalIgnoreCase);
        var duplicates = new List<DuplicateRow>();

        foreach (var row in rows)
        {
            var key = row.Record.Email;
            if (!kept.TryGetValue(key, out var existing))
            {
                kept[key] = row;
                continue;
            }

            var winner = ChooseWinner(existing, row);
            var loser = ReferenceEquals(winner, existing) ? row : existing;
            kept[key] = winner;
            duplicates.Add(new DuplicateRow(key, loser.RowNumber, winner.RowNumber, "duplicate email; kept latest updated_at then highest balance"));
        }

        return new DedupeResult(
            kept.Values.Select(value => value.Record).OrderBy(record => record.Email, StringComparer.OrdinalIgnoreCase).ToArray(),
            duplicates.OrderBy(duplicate => duplicate.RejectedRowNumber).ToArray());
    }

    private static RowWithSource ChooseWinner(RowWithSource left, RowWithSource right)
    {
        var updatedAtComparison = right.Record.UpdatedAt.CompareTo(left.Record.UpdatedAt);
        if (updatedAtComparison > 0) return right;
        if (updatedAtComparison < 0) return left;

        var balanceComparison = right.Record.Balance.CompareTo(left.Record.Balance);
        if (balanceComparison > 0) return right;
        if (balanceComparison < 0) return left;

        return left.RowNumber <= right.RowNumber ? left : right;
    }
}

public sealed record RowWithSource(CustomerRecord Record, int RowNumber);
public sealed record DedupeResult(IReadOnlyList<CustomerRecord> Records, IReadOnlyList<DuplicateRow> Duplicates);
