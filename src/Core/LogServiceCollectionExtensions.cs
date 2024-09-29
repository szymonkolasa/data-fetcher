using DataFetcher.Abstractions;
using DataFetcher.Core;

using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// 
/// </summary>
public static class LogServiceCollectionExtensions
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="services"></param>
    /// <returns></returns>
    public static ILogBuilder AddLog(this IServiceCollection services)
    {
        var builder = new LogBuilder(services);

        builder.Services.TryAddSingleton<ILogService, LogService>();
        
        return builder;
    }
}