using CsvProcessor.Poc.Domain;

namespace CsvProcessor.Poc.Parsing;

public interface ICsvReader
{
    IAsyncEnumerable<CsvRecord> ReadAsync(string path, CancellationToken cancellationToken);
}
