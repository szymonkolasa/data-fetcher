namespace DataFetcher.Core;

/// <summary>
/// 
/// </summary>
public interface IPayloadStore
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="fileName"></param>
    /// <param name="payload"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task CreateAsync(string fileName, string payload, CancellationToken cancellationToken);
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="fileName"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<string> FindByFileNameAsync(string fileName, CancellationToken cancellationToken);
}