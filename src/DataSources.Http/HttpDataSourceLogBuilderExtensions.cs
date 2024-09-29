using DataFetcher.Core;
using DataFetcher.DataSources.Http;

using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// 
/// </summary>
public static class HttpDataSourceLogBuilderExtensions
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="configureClient"></param>
    /// <returns></returns>
    public static ILogBuilder AddHttpDataSource(this ILogBuilder builder, Action<HttpClient> configureClient)
    {
        builder.Services.AddHttpClient(nameof(HttpDataSource), configureClient);
        builder.Services.TryAddSingleton<IDataSource, HttpDataSource>();
        return builder;
    }
}