namespace DataFetcher.Abstractions;

/// <summary>
/// An interface for log management.
/// </summary>
public interface ILogService
{
    /// <summary>
    /// Loads data from requested service.
    /// </summary>
    /// <param name="cancellationToken">Cancels the request on demand.</param>
    /// <returns>An information if the request has completed.</returns>
    Task FetchDataAsync(CancellationToken cancellationToken);
    
    /// <summary>
    /// Gets the list of fetched logs.
    /// </summary>
    /// <param name="from">The minimal date.</param>
    /// <param name="to">The maximum date.</param>
    /// <param name="cancellationToken">Cancels the request on demand.</param>
    /// <returns>A collection of stored logs between dates.</returns>
    /// <exception cref="ArgumentException">When <paramref name="from"/> is greater than <paramref name="to"/>.</exception>
    Task<IReadOnlyCollection<LogResponse>> GetLogsAsync(DateTimeOffset from, DateTimeOffset to, CancellationToken cancellationToken);

    /// <summary>
    /// Returns stored payload for requested log entry.
    /// </summary>
    /// <param name="id">The unique identifier of entry.</param>
    /// <param name="cancellationToken">Cancels the request on demand.</param>
    /// <returns>The payload of stored entry.</returns>
    /// <exception cref="LogNotFoundException">When log entry with <paramref name="id"/> was not found.</exception>
    Task<string> GetPayloadByLogIdAsync(Guid id, CancellationToken cancellationToken);
}