using Azure.Data.Tables;

using DataFetcher.Core;
using DataFetcher.Domain;

namespace DataFetcher.LogStores.Azure.Data.Tables;

public class AzureDataTablesLogStore(TableServiceClient client) : ILogStore
{
    private const string TableName = "Logs";

    private TableClient _table
    {
        get
        {
            client.CreateTableIfNotExists(TableName);
            return client.GetTableClient(TableName);
        }
    }
    
    public async Task<LogEntry?> FindByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var entity = await _table.GetEntityIfExistsAsync<LogEntryEntity>(id.ToString(), id.ToString(), cancellationToken: cancellationToken);
        if (entity.Value is null)
        {
            return null;
        }

        var value = entity.Value;
        return new LogEntry(value.Id, value.FetchDate, value.IsSuccess);
    }

    public Task CreateAsync(LogEntry logEntry, CancellationToken cancellationToken)
    {
        var entry = new LogEntryEntity
        {
            Id = logEntry.Id,
            FetchDate = logEntry.FetchDate,
            IsSuccess = logEntry.IsSuccess,
            PartitionKey = logEntry.Id.ToString(),
            RowKey = logEntry.Id.ToString(),
        };
        return _table.AddEntityAsync(entry, cancellationToken);
    }

    public async Task<IReadOnlyCollection<LogEntry>> GetLogsBetweenDatesAsync(DateTimeOffset from, DateTimeOffset to, CancellationToken cancellationToken)
    {
        List<LogEntry> response = [];
        await foreach (var item in _table.QueryAsync<LogEntryEntity>(e => e.FetchDate >= from && e.FetchDate <= to, cancellationToken: cancellationToken))
        {
            response.Add(new LogEntry(item.Id, item.FetchDate, item.IsSuccess));
        }

        return response;
    }
}