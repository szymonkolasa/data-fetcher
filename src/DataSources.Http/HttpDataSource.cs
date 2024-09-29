using DataFetcher.Core;

namespace DataFetcher.DataSources.Http;

/// <summary>
/// 
/// </summary>
/// <param name="factory"></param>
public class HttpDataSource(IHttpClientFactory factory) : IDataSource
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<DataSourceResponse> FetchAsync(CancellationToken cancellationToken)
    {
        string? payload = null;
        var client = factory.CreateClient(nameof(HttpDataSource));
        var request = await client.GetAsync(String.Empty, cancellationToken);
        if (request.IsSuccessStatusCode)
        {
            payload = await request.Content.ReadAsStringAsync(cancellationToken);
        }
        
        return new DataSourceResponse(request.IsSuccessStatusCode, payload);
    }
}