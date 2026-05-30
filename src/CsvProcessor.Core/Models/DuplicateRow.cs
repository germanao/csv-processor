namespace CsvProcessor.Core.Models;

public sealed record DuplicateRow(string BusinessKey, int RejectedRowNumber, int KeptRowNumber, string Reason);
