namespace AnxiousAnt.Http;

/// <summary>
/// Represents a callback delegate used to report the progress of a download operation.
/// </summary>
/// <param name="progress">A float value between 0 and 1 indicating the completion percentage of the download.</param>
public delegate void DownloadProgressCallback(float progress);