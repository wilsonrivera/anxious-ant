using System.Net;
using System.Security.Cryptography;

namespace AnxiousAnt.Http;

public class HttpClientExtensionsTests
{
    #region Head

    [Fact]
    public async Task HeadAsync_ShouldThrowWhenGivenNullClient()
    {
        // Arrange
        Func<Task> act = () => HttpClientExtensions.HeadAsync(null!, new Url());

        // Assert
        await act.ShouldThrowAsync<ArgumentNullException>();
    }

    [Fact]
    public async Task HeadAsync_ShouldThrowWhenGivenNullUrl()
    {
        // Arrange
        using var client = new HttpClient(CreateHandler());
        // ReSharper disable once AccessToDisposedClosure
        Func<Task> act = () => client.HeadAsync(null!);

        // Assert
        await act.ShouldThrowAsync<ArgumentNullException>();
    }

    [Fact]
    public async Task HeadAsync_ShouldReturnExpectedResponse()
    {
        // Arrange
        var handler = CreateHandler();
        using var client = new HttpClient(handler);
        A.CallTo(handler)
            .Where(call => call.Method.Name == "SendAsync")
            .WithReturnType<Task<HttpResponseMessage>>()
            .Returns(Task.FromResult(new HttpResponseMessage { StatusCode = HttpStatusCode.NoContent, }));

        // Act
        using var response = await client.HeadAsync(Url.Parse("https://example.com"));

        // Assert
        A.CallTo(handler)
            .Where(call =>
                call.Method.Name == "SendAsync" &&
                call.GetArgument<HttpRequestMessage>(0)!.Method == HttpMethod.Head
            )
            .WithReturnType<Task<HttpResponseMessage>>()
            .MustHaveHappenedOnceExactly();

        response.StatusCode.ShouldBe(HttpStatusCode.NoContent);
    }

    #endregion

    #region Get

    [Fact]
    public async Task GetAsync_ShouldThrowWhenGivenNullClient()
    {
        // Arrange
        Func<Task> act = () => HttpClientExtensions.GetAsync(null!, new Url());

        // Assert
        await act.ShouldThrowAsync<ArgumentNullException>();
    }

    [Fact]
    public async Task GetAsync_ShouldThrowWhenGivenNullUrl()
    {
        // Arrange
        using var client = new HttpClient(CreateHandler());
        // ReSharper disable once AccessToDisposedClosure
        Func<Task> act = () => client.GetAsync((Url)null!);

        // Assert
        await act.ShouldThrowAsync<ArgumentNullException>();
    }

    [Fact]
    public async Task GetAsync_ShouldReturnExpectedResponse()
    {
        // Arrange
        var handler = CreateHandler();
        using var client = new HttpClient(handler);
        A.CallTo(handler)
            .Where(call => call.Method.Name == "SendAsync")
            .WithReturnType<Task<HttpResponseMessage>>()
            .Returns(Task.FromResult(new HttpResponseMessage { StatusCode = HttpStatusCode.NoContent, }));

        // Act
        using var response = await client.GetAsync(Url.Parse("https://example.com"));

        // Assert
        A.CallTo(handler)
            .Where(call =>
                call.Method.Name == "SendAsync" &&
                call.GetArgument<HttpRequestMessage>(0)!.Method == HttpMethod.Get
            )
            .WithReturnType<Task<HttpResponseMessage>>()
            .MustHaveHappenedOnceExactly();

        response.StatusCode.ShouldBe(HttpStatusCode.NoContent);
    }

    #endregion

    #region Delete

    [Fact]
    public async Task DeleteAsync_ShouldThrowWhenGivenNullClient()
    {
        // Arrange
        Func<Task> act = () => HttpClientExtensions.DeleteAsync(null!, new Url());

        // Assert
        await act.ShouldThrowAsync<ArgumentNullException>();
    }

    [Fact]
    public async Task DeleteAsync_ShouldThrowWhenGivenNullUrl()
    {
        // Arrange
        using var client = new HttpClient(CreateHandler());
        // ReSharper disable once AccessToDisposedClosure
        Func<Task> act = () => client.DeleteAsync((Url)null!);

        // Assert
        await act.ShouldThrowAsync<ArgumentNullException>();
    }

    [Fact]
    public async Task DeleteAsync_ShouldReturnExpectedResponse()
    {
        // Arrange
        var handler = CreateHandler();
        using var client = new HttpClient(handler);
        A.CallTo(handler)
            .Where(call => call.Method.Name == "SendAsync")
            .WithReturnType<Task<HttpResponseMessage>>()
            .Returns(Task.FromResult(new HttpResponseMessage { StatusCode = HttpStatusCode.NoContent, }));

        // Act
        using var response = await client.DeleteAsync(Url.Parse("https://example.com"));

        // Assert
        A.CallTo(handler)
            .Where(call =>
                call.Method.Name == "SendAsync" &&
                call.GetArgument<HttpRequestMessage>(0)!.Method == HttpMethod.Delete
            )
            .WithReturnType<Task<HttpResponseMessage>>()
            .MustHaveHappenedOnceExactly();

        response.StatusCode.ShouldBe(HttpStatusCode.NoContent);
    }

    #endregion

    #region Download

    [Fact]
    public async Task DownloadAsync_ShouldThrowWhenGivenNullClient()
    {
        // Arrange
        Func<Task> act = () => HttpClientExtensions.DownloadAsync(null!, new Url(), "");

        // Assert
        await act.ShouldThrowAsync<ArgumentNullException>();
    }

    [Fact]
    public async Task DownloadAsync_ShouldThrowWhenGivenNullUrl()
    {
        // Arrange
        using var client = new HttpClient(CreateHandler());
        // ReSharper disable once AccessToDisposedClosure
        Func<Task> act = () => client.DownloadAsync(null!, "");

        // Assert
        await act.ShouldThrowAsync<ArgumentNullException>();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("        ")]
    public async Task DownloadAsync_ShouldThrowWhenGivenNullEmptyOrWhiteSpaceFilePath(string? filePath)
    {
        // Arrange
        using var client = new HttpClient(CreateHandler());
        // ReSharper disable once AccessToDisposedClosure
        Func<Task> act = () => client.DownloadAsync(new Url(), filePath!);

        // Assert
        await act.ShouldThrowAsync<ArgumentException>();
    }

    [Fact]
    public async Task DownloadAsync_ShouldThrowWhenGivenNullStream()
    {
        // Arrange
        using var client = new HttpClient(CreateHandler());
        // ReSharper disable once AccessToDisposedClosure
        Func<Task> act = () => client.DownloadAsync(new Url(), (Stream)null!, CancellationToken.None);

        // Assert
        await act.ShouldThrowAsync<ArgumentNullException>();
    }

    [Fact]
    public async Task DownloadAsync_ShouldReportDownloadCompletedWhenContentLengthIsNotAvailable()
    {
        // Arrange
        var handler = CreateHandler();
        using var client = new HttpClient(handler);
        using var stream = new MemoryStream();

        A.CallTo(handler)
            .Where(call => call.Method.Name == "SendAsync")
            .WithReturnType<Task<HttpResponseMessage>>()
            .Returns(Task.FromResult(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.NoContent,
                Content = new ByteArrayContent(RandomNumberGenerator.GetBytes(10 * 1024 * 1024))
            }));

        var downloadCallback = A.Fake<DownloadProgressCallback>();

        // Act
        await client.DownloadAsync(
            Url.Parse("https://example.com"),
            stream,
            downloadCallback,
            CancellationToken.None
        );

        // Assert
        A.CallTo(downloadCallback).MustHaveHappenedOnceExactly();
        A.CallTo(downloadCallback)
            .WhenArgumentsMatch(args => (float)args[0]! >= 1f)
            .MustHaveHappenedOnceExactly();
    }

    [Fact]
    public async Task DownloadAsync_ShouldReportDownloadCompletedWhenContentLengthIsInvalid()
    {
        // Arrange
        var handler = CreateHandler();
        using var client = new HttpClient(handler);
        using var stream = new MemoryStream();

        A.CallTo(handler)
            .Where(call => call.Method.Name == "SendAsync")
            .WithReturnType<Task<HttpResponseMessage>>()
            .Returns(Task.FromResult(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.NoContent,
                Content = new ByteArrayContent(RandomNumberGenerator.GetBytes(10 * 1024 * 1024))
                {
                    Headers = { ContentLength = -1 }
                }
            }));

        var downloadCallback = A.Fake<DownloadProgressCallback>();

        // Act
        await client.DownloadAsync(
            Url.Parse("https://example.com"),
            stream,
            downloadCallback,
            CancellationToken.None
        );

        // Assert
        A.CallTo(downloadCallback).MustHaveHappenedOnceExactly();
        A.CallTo(downloadCallback)
            .WhenArgumentsMatch(args => (float)args[0]! >= 1f)
            .MustHaveHappenedOnceExactly();
    }

    [Fact]
    public async Task DownloadAsync_ShouldReportDownloadProgress()
    {
        // Arrange
        var handler = CreateHandler();
        using var client = new HttpClient(handler);
        using var stream = new MemoryStream();

        A.CallTo(handler)
            .Where(call => call.Method.Name == "SendAsync")
            .WithReturnType<Task<HttpResponseMessage>>()
            .Returns(Task.FromResult(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.NoContent,
                Content = new ByteArrayContent(RandomNumberGenerator.GetBytes(10 * 1024 * 1024))
                {
                    Headers = { ContentLength = 10 * 1024 * 1024 }
                }
            }));

        var downloadCallback = A.Fake<DownloadProgressCallback>();

        // Act
        await client.DownloadAsync(
            Url.Parse("https://example.com"),
            stream,
            downloadCallback,
            CancellationToken.None
        );

        // Assert
        A.CallTo(downloadCallback).MustHaveHappenedOnceOrMore();
        A.CallTo(downloadCallback)
            .WhenArgumentsMatch(args => (float)args[0]! >= 1f)
            .MustHaveHappenedOnceExactly();
    }

    #endregion

    private static HttpClientHandler CreateHandler() =>
        A.Fake<HttpClientHandler>(static opts =>
            opts.WithArgumentsForConstructor([])
                .CallsBaseMethods()
        );
}