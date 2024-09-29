using DataFetcher.Core;
using DataFetcher.PayloadStores.Azure.Storage.Blobs;

using Microsoft.Extensions.Azure;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// 
/// </summary>
public static class AzureStorageBlobsLogBuilderExtensions
{
    public static ILogBuilder AddAzureStorageBlobsPayloadStore(this ILogBuilder builder)
    {
        builder.Services.TryAddSingleton<IPayloadStore, AzureBlobStoragePayloadStore>();
        return builder;
    }
}