namespace CsvProcessor.Core.Models;

public sealed record CustomerRecord(
    string CustomerId,
    string Email,
    string FullName,
    DateOnly DateOfBirth,
    decimal Balance,
    CustomerStatus Status,
    DateTimeOffset UpdatedAt,
    string SourceRowHash);

public enum CustomerStatus
{
    Unknown = 0,
    Active = 1,
    Inactive = 2,
    Suspended = 3
}
