using System.Text;
using CsvProcessor.Models;
using CsvProcessor.Pipeline;
using Xunit;

namespace CsvProcessor.Tests;

public sealed class CsvReaderStageTests
{
    [Fact(Skip = "Learning exercise: implement the parser before enabling this test.")]
    public async Task ReadAsync_Should_Handle_Quoted_Commas()
    {
        var csv = "Id,Name\n1,\"Ada, Lovelace\"\n";
        await using var stream = new MemoryStream(Encoding.UTF8.GetBytes(csv));
        var stage = new CsvReaderStage();
        var options = new ProcessingOptions("input.csv", "output.csv", ',', HasHeader: true, BatchSize: 100);

        var rows = new List<CsvRow>();
        await foreach (var row in stage.ReadAsync(stream, options, CancellationToken.None))
        {
            rows.Add(row);
        }

        Assert.Equal("Ada, Lovelace", rows.Single().Values["Name"]);
    }
}
