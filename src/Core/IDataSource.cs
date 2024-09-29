namespace DataFetcher.Core;

/// <summary>
/// 
/// </summary>
public interface IDataSource
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<DataSourceResponse> FetchAsync(CancellationToken cancellationToken);
}