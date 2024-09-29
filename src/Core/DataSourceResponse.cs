namespace DataFetcher.Core;

/// <summary>
/// 
/// </summary>
/// <param name="IsSuccess"></param>
/// <param name="Payload"></param>
public record DataSourceResponse(bool IsSuccess, string? Payload);