namespace DataFetcher.Domain;

/// <summary>
/// 
/// </summary>
/// <param name="Id"></param>
/// <param name="FetchDate"></param>
/// <param name="IsSuccess"></param>
public record LogEntry(Guid Id, DateTimeOffset FetchDate, bool IsSuccess);