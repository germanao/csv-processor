namespace CsvProcessor.Infrastructure;

public sealed class LocalFileSystemSource : ICsvSource
{
    public Stream OpenRead(string path)
    {
        // TODO: Validate existence, permissions, and path traversal rules before opening the file.
        return File.OpenRead(path);
    }

    public Stream OpenWrite(string path)
    {
        // TODO: Create parent directories, decide overwrite behavior, and write to a temp file first.
        return File.Create(path);
    }
}
