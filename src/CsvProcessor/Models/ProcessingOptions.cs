namespace CsvProcessor.Models;

/// <summary>
/// Runtime configuration for the CSV processor.
/// TODO: Add encoding, culture, quote character, escape strategy, schema path, bad-row output, and overwrite policy.
/// </summary>
public sealed record ProcessingOptions(
    string InputPath,
    string OutputPath,
    char Delimiter,
    bool HasHeader,
    int BatchSize);
