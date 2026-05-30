using CsvProcessor.Poc.Domain;
using CsvProcessor.Poc.Parsing;
using CsvProcessor.Poc.Pipeline;
using CsvProcessor.Poc.Transformation;
using CsvProcessor.Poc.Validation;
using CsvProcessor.Poc.Writing;

namespace CsvProcessor.Poc;

public static class Program
{
    public static async Task<int> Main(string[] args)
    {
        // TODO: Replace this hand-written argument parser with System.CommandLine or a small validated options type.
        // TODO: Add options for delimiter, culture, encoding, quote mode, batch size, and error handling strategy.
        var inputPath = ReadOption(args, "--input") ?? "samples/input.csv";
        var outputPath = ReadOption(args, "--output") ?? "out/output.csv";

        var schema = CsvSchema.CreateTeachingSchema();
        var pipeline = new CsvProcessingPipeline(
            new NaiveCsvReader(),
            new CsvValidator(schema),
            new CsvTransformer(),
            new CsvWriter());

        var result = await pipeline.ProcessAsync(inputPath, outputPath, CancellationToken.None);

        Console.WriteLine($"Rows read: {result.RowsRead}");
        Console.WriteLine($"Rows written: {result.RowsWritten}");
        Console.WriteLine($"Errors: {result.Errors.Count}");

        // TODO: Decide whether validation failures should return a non-zero exit code.
        // For now this mock always exits successfully so learners can inspect partial output.
        return 0;
    }

    private static string? ReadOption(string[] args, string optionName)
    {
        var index = Array.IndexOf(args, optionName);
        if (index < 0 || index + 1 >= args.Length)
        {
            return null;
        }

        return args[index + 1];
    }
}
