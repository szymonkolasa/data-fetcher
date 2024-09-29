using System.Text;

using Azure.Storage.Blobs;

using DataFetcher.Core;

namespace DataFetcher.PayloadStores.Azure.Storage.Blobs;

public class AzureBlobStoragePayloadStore(BlobServiceClient client) : IPayloadStore
{
    private const string ContainerName = "payload-store";

    private BlobContainerClient _container
    {
        get
        {
            var container = client.GetBlobContainerClient(ContainerName);
            container.CreateIfNotExists();
            return container;
        }
    }
    
    public Task CreateAsync(string fileName, string payload, CancellationToken cancellationToken)
    {
        var blobName = $"{fileName}.txt";
        var blob = _container.GetBlobClient(blobName);

        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(payload));
        return blob.UploadAsync(stream, cancellationToken);
    }

    public async Task<string> FindByFileNameAsync(string fileName, CancellationToken cancellationToken)
    {
        var blob = _container.GetBlobClient($"{fileName}.txt");
        var content = await blob.DownloadContentAsync(cancellationToken);
        return Encoding.UTF8.GetString(content.Value.Content);
    }
}