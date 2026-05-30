using CsvProcessor.Core.Processing;

namespace CsvProcessor.Tests;

public sealed class CsvCustomerBatchProcessorTests
{
    [Fact]
    public void Process_NormalizesInvalidRowsAndDuplicates()
    {
        const string csv = """
customer_id,email,full_name,date_of_birth,balance,status,updated_at
 c-1 , USER@EXAMPLE.COM , Ada   Lovelace ,1815-12-10,100.50,active,2025-01-01T10:00:00Z
c-2,bad-email,Grace Hopper,1906-12-09,99.00,active,2025-01-02T10:00:00Z
c-3,user@example.com,Ada Byron,1815-12-10,200.00,A,2025-01-03T10:00:00Z
""";

        var result = CsvCustomerBatchProcessor.CreateDefault().Process(csv);

        Assert.Single(result.Accepted);
        Assert.Equal("user@example.com", result.Accepted[0].Email);
        Assert.Equal("Ada Byron", result.Accepted[0].FullName);
        Assert.Single(result.InvalidRows);
        Assert.Single(result.DuplicateRows);
        Assert.Equal(3, result.QualitySummary.TotalRows);
    }

    [Fact]
    public void Process_ReturnsSchemaDriftAsInvalidRows()
    {
        const string csv = """
customer_id,email,name,date_of_birth,balance,status,updated_at
c-1,user@example.com,Ada Lovelace,1815-12-10,100.50,active,2025-01-01T10:00:00Z
""";

        var result = CsvCustomerBatchProcessor.CreateDefault().Process(csv);

        Assert.Empty(result.Accepted);
        Assert.Single(result.InvalidRows);
        Assert.Contains("schema drift", result.InvalidRows[0].Reason);
    }
}
