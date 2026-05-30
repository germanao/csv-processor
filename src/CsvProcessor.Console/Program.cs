using System.Text.Json;
using CsvProcessor.Core.Processing;

if (args.Length == 0)
{
    Console.Error.WriteLine("Usage: dotnet run --project src/CsvProcessor.Console -- <path-to-csv> [previous-watermark]");
    return 2;
}

var csvPath = args[0];
var previousWatermark = args.Length > 1 && DateTimeOffset.TryParse(args[1], out var parsedWatermark) ? parsedWatermark : (DateTimeOffset?)null;
var csvText = await File.ReadAllTextAsync(csvPath).ConfigureAwait(false);
var processor = CsvCustomerBatchProcessor.CreateDefault();
var result = processor.Process(csvText, previousWatermark);

Console.WriteLine(JsonSerializer.Serialize(result, new JsonSerializerOptions { WriteIndented = true }));
return result.InvalidRows.Count == 0 ? 0 : 1;
