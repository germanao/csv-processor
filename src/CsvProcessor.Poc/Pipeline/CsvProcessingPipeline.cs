using CsvProcessor.Poc.Domain;
using CsvProcessor.Poc.Parsing;
using CsvProcessor.Poc.Transformation;
using CsvProcessor.Poc.Validation;
using CsvProcessor.Poc.Writing;

namespace CsvProcessor.Poc.Pipeline;

public sealed class CsvProcessingPipeline
{
    private readonly ICsvReader _reader;
    private readonly CsvValidator _validator;
    private readonly CsvTransformer _transformer;
    private readonly CsvWriter _writer;

    public CsvProcessingPipeline(
        ICsvReader reader,
        CsvValidator validator,
        CsvTransformer transformer,
        CsvWriter writer)
    {
        _reader = reader;
        _validator = validator;
        _transformer = transformer;
        _writer = writer;
    }

    public async Task<CsvProcessingResult> ProcessAsync(
        string inputPath,
        string outputPath,
        CancellationToken cancellationToken)
    {
        var result = new CsvProcessingResult();
        var acceptedRows = new List<CsvRecord>();

        await foreach (var record in _reader.ReadAsync(inputPath, cancellationToken))
        {
            result.RowsRead++;

            var errors = _validator.Validate(record);
            if (errors.Count > 0)
            {
                result.Errors.AddRange(errors);

                // TODO: Make this behavior configurable: skip, stop immediately, or write rejected rows to a dead-letter file.
                continue;
            }

            var transformed = _transformer.Transform(record);
            acceptedRows.Add(transformed);
        }

        await _writer.WriteAsync(outputPath, acceptedRows, cancellationToken);
        result.RowsWritten = acceptedRows.Count;

        // TODO: Emit structured metrics such as elapsed time, rows/second, validation error rate, and output byte count.
        return result;
    }
}
