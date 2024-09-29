using Azure;
using Azure.Data.Tables;

namespace DataFetcher.LogStores.Azure.Data.Tables;

public class LogEntryEntity : ITableEntity
{
    public string PartitionKey { get; set; }
    public string RowKey { get; set; }
    public DateTimeOffset? Timestamp { get; set; }
    public ETag ETag { get; set; }
    public Guid Id { get; set; }
    public DateTimeOffset FetchDate { get; set; }
    public bool IsSuccess { get; set; }
}