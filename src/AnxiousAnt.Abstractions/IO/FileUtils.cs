namespace AnxiousAnt.IO;

/// <summary>
/// Provides utility methods for file operations including writing text to files with various encodings.
/// </summary>
public static class FileUtils
{
    private static readonly Lazy<Encoding> DefaultEncoding = new(static () => new UTF8Encoding(false));

    /// <summary>
    /// Writes the specified text content to a file at the given path using the default UTF-8 encoding,
    /// and ensures all changes are flushed to the disk.
    /// </summary>
    /// <param name="filename">The full file path where the text content will be written.</param>
    /// <param name="content">The text content to write to the file.</param>
    /// <exception cref="ArgumentException">Thrown if the path is null, empty, or consists only of white spaces.</exception>
    /// <exception cref="ArgumentNullException">Thrown if the content is null.</exception>
    /// <exception cref="IOException">Thrown if an I/O error occurs during the write or flush operation.</exception>
    [ExcludeFromCodeCoverage]
    public static void WriteText(string filename, string content) =>
        WriteText(filename, content, DefaultEncoding.Value);

    /// <summary>
    /// Writes the specified text content to a file at the given path using the default encoding,
    /// and ensures all changes are flushed to the disk.
    /// </summary>
    /// <param name="filename">The full file path where the text content will be written.</param>
    /// <param name="content">The text content to write to the file.</param>
    /// <param name="encoding">The <see cref="Encoding"/> to write text with.</param>
    /// <exception cref="ArgumentException">Thrown if the path is null, empty, or consists only of white spaces.</exception>
    /// <exception cref="ArgumentNullException">Thrown if the content is null.</exception>
    /// <exception cref="IOException">Thrown if an I/O error occurs during the write or flush operation.</exception>
    public static void WriteText(string filename, string content, Encoding encoding)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(filename);
        ArgumentNullException.ThrowIfNull(content);
        ArgumentNullException.ThrowIfNull(encoding);

        using var stream = File.Create(filename);
        using var writer = new StreamWriter(stream, encoding);
        writer.Write(content);
        writer.Flush();

        stream.Flush(true);
    }

    /// <summary>
    /// Safely writes the specified content to a file by first writing to a temporary file,
    /// replacing the original file with the temporary one atomically, and optionally creating a backup.
    /// </summary>
    /// <param name="filename">The full path to the target file where the content will be written.</param>
    /// <param name="content">The content to write to the file.</param>
    /// <exception cref="ArgumentException">Thrown if the filename is null, empty, or consists only of white spaces.</exception>
    /// <exception cref="ArgumentNullException">Thrown if the content is null.</exception>
    /// <exception cref="IOException">Thrown if an I/O error occurs during the write, replace, or delete operation.</exception>
    public static void SafeWrite(string filename, string content)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(filename);
        ArgumentNullException.ThrowIfNull(content);

        if (!File.Exists(filename))
        {
            WriteText(filename, content);
            return;
        }

        var tempFilename = $"{filename}.{Guid.NewGuid():N}.tmp";
        var backupFilename = $"{filename}.{Guid.NewGuid():N}.bck";

        WriteText(tempFilename, content);
        try
        {
            File.Replace(tempFilename, filename, backupFilename);
        }
        catch (IOException)
        {
            File.Move(filename, backupFilename);
            File.Move(tempFilename, filename);
        }

        File.Delete(backupFilename);
    }
}