using System.Globalization;

namespace AnxiousAnt.Http;

/// <summary>
/// Provides extension methods for the <see cref="HttpClient"/> class.
/// </summary>
public static class HttpClientExtensions
{
    private const int SmallDownloadBufferSize = 8 * 1024; // 8kb~
    private const int LargeDownloadBufferSize = 32 * 1024; // 32kb~;
    private const int LargeDownloadSizeThreshold = 100 * 1024 * 1024; // 100mb~;

    /// <summary>
    /// Sends an HTTP HEAD request to the specified URL.
    /// </summary>
    /// <param name="client">The <see cref="HttpClient"/> used to send the request.</param>
    /// <param name="url">The URL of the request.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>
    /// A task that represents the asynchronous operation and provides the HTTP response.
    /// </returns>
    public static Task<HttpResponseMessage> HeadAsync(
        this HttpClient client,
        Url url,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ArgumentNullException.ThrowIfNull(client);
        ArgumentNullException.ThrowIfNull(url);

        return SendAsync(client, url, HttpMethod.Head, HttpCompletionOption.ResponseHeadersRead, cancellationToken);
    }

    /// <summary>
    /// Sends an HTTP GET request to the specified URL.
    /// </summary>
    /// <param name="client">The <see cref="HttpClient"/> used to send the request.</param>
    /// <param name="url">The URL of the request.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>
    /// A task that represents the asynchronous operation and provides the HTTP response.
    /// </returns>
    [ExcludeFromCodeCoverage]
    public static Task<HttpResponseMessage> GetAsync(
        this HttpClient client,
        Url url,
        CancellationToken cancellationToken = default) =>
        GetAsync(client, url, HttpCompletionOption.ResponseHeadersRead, cancellationToken);

    /// <summary>
    /// Sends an HTTP GET request to the specified URL.
    /// </summary>
    /// <param name="client">The <see cref="HttpClient"/> used to send the request.</param>
    /// <param name="url">The URL of the request.</param>
    /// <param name="completionOption">When the operation should complete (as soon as a response is available or after
    /// reading the whole response content).</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>
    /// A task that represents the asynchronous operation and provides the HTTP response.
    /// </returns>
    public static Task<HttpResponseMessage> GetAsync(
        this HttpClient client,
        Url url,
        HttpCompletionOption completionOption,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ArgumentNullException.ThrowIfNull(client);
        ArgumentNullException.ThrowIfNull(url);

        return SendAsync(client, url, HttpMethod.Get, completionOption, cancellationToken);
    }

    /// <summary>
    /// Sends an HTTP DELETE request to the specified URL.
    /// </summary>
    /// <param name="client">The <see cref="HttpClient"/> used to send the request.</param>
    /// <param name="url">The URL of the request.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>
    /// A task that represents the asynchronous operation and provides the HTTP response.
    /// </returns>
    [ExcludeFromCodeCoverage]
    public static Task<HttpResponseMessage> DeleteAsync(
        this HttpClient client,
        Url url,
        CancellationToken cancellationToken = default) =>
        DeleteAsync(client, url, HttpCompletionOption.ResponseHeadersRead, cancellationToken);

    /// <summary>
    /// Sends an HTTP DELETE request to the specified URL.
    /// </summary>
    /// <param name="client">The <see cref="HttpClient"/> used to send the request.</param>
    /// <param name="url">The URL of the request.</param>
    /// <param name="completionOption">When the operation should complete (as soon as a response is available or after
    /// reading the whole response content).</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>
    /// A task that represents the asynchronous operation and provides the HTTP response.
    /// </returns>
    public static Task<HttpResponseMessage> DeleteAsync(
        this HttpClient client,
        Url url,
        HttpCompletionOption completionOption,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ArgumentNullException.ThrowIfNull(client);
        ArgumentNullException.ThrowIfNull(url);

        using var request = new HttpRequestMessage();
        request.Method = HttpMethod.Delete;
        request.RequestUri = url.ToUri();

        return SendAsync(client, url, HttpMethod.Delete, completionOption, cancellationToken);
    }

    /// <summary>
    /// Downloads the content at the specified URL and saves it to the specified file path.
    /// </summary>
    /// <param name="client">The <see cref="HttpClient"/> instance used to send the request.</param>
    /// <param name="url">The URL from which to download the content.</param>
    /// <param name="filePath">The file path to which the downloaded content will be saved.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>
    /// A task that represents the asynchronous download operation.
    /// </returns>
    [ExcludeFromCodeCoverage]
    public static Task DownloadAsync(
        this HttpClient client,
        Url url,
        string filePath,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return DownloadAsync(client, url, filePath, null, cancellationToken);
    }

    /// <summary>
    /// Downloads the content at the specified URL and saves it to the specified file path.
    /// </summary>
    /// <param name="client">The <see cref="HttpClient"/> instance used to send the request.</param>
    /// <param name="url">The URL from which to download the content.</param>
    /// <param name="filePath">The file path to which the downloaded content will be saved.</param>
    /// <param name="progressCallback">An optional callback to report download progress as a value between 0 and 1.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>
    /// A task that represents the asynchronous download operation.
    /// </returns>
    [ExcludeFromCodeCoverage]
    public static async Task DownloadAsync(
        this HttpClient client,
        Url url,
        string filePath,
        DownloadProgressCallback? progressCallback,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ArgumentNullException.ThrowIfNull(client);
        ArgumentNullException.ThrowIfNull(url);
        ArgumentException.ThrowIfNullOrWhiteSpace(filePath);

        await using var stream = File.OpenWrite(filePath);
        await DownloadAsync(client, url, stream, progressCallback, cancellationToken);
    }

    /// <summary>
    /// Downloads content from the specified URL and writes it to the provided output stream.
    /// </summary>
    /// <param name="client">The <see cref="HttpClient"/> used to send the request.</param>
    /// <param name="url">The target URL to download the content from.</param>
    /// <param name="outputStream">The stream to which the downloaded content will be written.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>
    /// A task that represents the asynchronous download operation.
    /// </returns>
    [ExcludeFromCodeCoverage]
    public static Task DownloadAsync(
        this HttpClient client,
        Url url,
        Stream outputStream,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return DownloadAsync(client, url, outputStream, null, cancellationToken);
    }

    /// <summary>
    /// Downloads content from the specified URL and writes it to the provided output stream.
    /// </summary>
    /// <param name="client">The <see cref="HttpClient"/> used to send the request.</param>
    /// <param name="url">The target URL to download the content from.</param>
    /// <param name="outputStream">The stream to which the downloaded content will be written.</param>
    /// <param name="progressCallback">An optional callback to report download progress as a value between 0 and 1.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>
    /// A task that represents the asynchronous download operation.
    /// </returns>
    public static async Task DownloadAsync(
        this HttpClient client,
        Url url,
        Stream outputStream,
        DownloadProgressCallback? progressCallback = null,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ArgumentNullException.ThrowIfNull(client);
        ArgumentNullException.ThrowIfNull(url);
        ArgumentNullException.ThrowIfNull(outputStream);

        using var response = await GetAsync(client, url, HttpCompletionOption.ResponseHeadersRead, cancellationToken);
        response.EnsureSuccessStatusCode();

        var bufferSize = !TryGetContentLength(response, out var contentLength) ||
                         contentLength < LargeDownloadSizeThreshold
            ? SmallDownloadBufferSize
            : LargeDownloadBufferSize;

        await using var stream = await response.Content.ReadAsStreamAsync(cancellationToken);
        var totalReadBytes = 0L;

        int read;
        using var owner = MemoryOwner<byte>.Allocate(bufferSize);
        while ((read = await stream.ReadAsync(owner.Memory, cancellationToken)) > 0)
        {
            await outputStream.WriteAsync(owner.Memory[..read], cancellationToken);
            totalReadBytes += read;

            if (contentLength > 0)
            {
                progressCallback?.Invoke((totalReadBytes / (float)contentLength));
            }
        }

        if (contentLength == 0 && progressCallback is not null)
        {
            progressCallback.Invoke(1);
        }

        await outputStream.FlushAsync(cancellationToken);
    }

    private static bool TryGetContentLength(HttpResponseMessage response, out long contentLength)
    {
        contentLength = 0;
        if (!response.Content.Headers.TryGetValues("Content-Length", out var contentLengthValues))
        {
            return false;
        }

        foreach (var value in contentLengthValues)
        {
            if (!long.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out var parsedContentLength) ||
                parsedContentLength <= 0)
            {
                continue;
            }

            contentLength = parsedContentLength;
            return true;
        }

        return false;
    }

    private static async Task<HttpResponseMessage> SendAsync(
        HttpClient client,
        Url url,
        HttpMethod method,
        HttpCompletionOption completionOption,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ArgumentNullException.ThrowIfNull(client);
        ArgumentNullException.ThrowIfNull(url);

        using var request = new HttpRequestMessage();
        request.Method = method;
        request.RequestUri = url.ToUri();

        return await client.SendAsync(request, completionOption, cancellationToken);
    }
}