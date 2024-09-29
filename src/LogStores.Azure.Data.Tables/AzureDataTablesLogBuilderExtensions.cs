using DataFetcher.Core;
using DataFetcher.LogStores.Azure.Data.Tables;

using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// 
/// </summary>
public static class AzureDataTablesLogBuilderExtensions
{
    public static ILogBuilder AddAzureDataTablesLogStore(this ILogBuilder builder)
    {
        builder.Services.TryAddSingleton<ILogStore, AzureDataTablesLogStore>();
        return builder;
    }
}