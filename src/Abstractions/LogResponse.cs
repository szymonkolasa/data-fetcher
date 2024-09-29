namespace DataFetcher.Abstractions;

/// <summary>
/// Represents the log entry information.
/// </summary>
/// <param name="Id">The unique identifier of log entry.</param>
/// <param name="FetchDate">The date when log has been fetched.</param>
/// <param name="IsSuccess">Set to <c>true</c> when the data has been fetched successfully, otherwise <c>false</c>.</param>
public record LogResponse(Guid Id, DateTimeOffset FetchDate, bool IsSuccess);