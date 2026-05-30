namespace CsvProcessor.Infrastructure;

public interface ICsvSource
{
    // TODO: Return streams plus metadata such as content length, ETag/checksum, and detected encoding.
    Stream OpenRead(string path);

    // TODO: Support atomic writes through a temp file and final rename to avoid partial outputs.
    Stream OpenWrite(string path);
}
