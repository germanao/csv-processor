using CsvProcessor.Infrastructure;
using CsvProcessor.Models;
using CsvProcessor.Pipeline;

// Educational entry point: keep the bootstrapping simple so the learner can focus on pipeline design.
// TODO: Replace this ad-hoc argument parsing with System.CommandLine, validation, help text, and exit codes.
if (args.Length < 2)
{
    Console.Error.WriteLine("Usage: CsvProcessor <input.csv> <output.csv>");
    return 2;
}

var options = new ProcessingOptions(
    InputPath: args[0],
    OutputPath: args[1],
    Delimiter: ',',
    HasHeader: true,
    BatchSize: 500);

var pipeline = new CsvProcessingPipeline(
    new LocalFileSystemSource(),
    new CsvReaderStage(),
    new ValidationStage(),
    new TransformationStage(),
    new CsvWriterStage());

var summary = await pipeline.RunAsync(options, CancellationToken.None);

Console.WriteLine($"Rows seen: {summary.RowsRead}");
Console.WriteLine($"Rows written: {summary.RowsWritten}");
Console.WriteLine($"Rows rejected: {summary.RowsRejected}");

return summary.RowsRejected == 0 ? 0 : 1;
