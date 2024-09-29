using DataFetcher.Abstractions;
using DataFetcher.Domain;

namespace DataFetcher.Core;

/// <summary>
/// 
/// </summary>
/// <param name="dataSource"></param>
/// <param name="logStore"></param>
/// <param name="payloadStore"></param>
public class LogService(IDataSource dataSource, ILogStore logStore, IPayloadStore payloadStore) : ILogService
{
    /// <inheritdoc />
    public async Task FetchDataAsync(CancellationToken cancellationToken)
    {
        var id = Guid.NewGuid();
        var response = await dataSource.FetchAsync(cancellationToken);

        if (response.IsSuccess)
        {
            await payloadStore.CreateAsync(id.ToString(), response.Payload!, cancellationToken); 
        }
        
        await logStore.CreateAsync(new LogEntry(id, DateTimeOffset.Now, response.IsSuccess), cancellationToken);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyCollection<LogResponse>> GetLogsAsync(DateTimeOffset from, DateTimeOffset to, CancellationToken cancellationToken)
    {
        if (from > to)
        {
            throw new ArgumentException("From date cannot be later than to date.");
        }
        
        var data = await logStore.GetLogsBetweenDatesAsync(from, to, cancellationToken);
        return data
            .Select(x => new LogResponse(x.Id, x.FetchDate, x.IsSuccess))
            .ToList();
    }

    /// <inheritdoc />
    public async Task<string> GetPayloadByLogIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var entry = await logStore.FindByIdAsync(id, cancellationToken);

        if (entry is null)
        {
            throw new LogNotFoundException();
        }

        return entry switch
        {
            { IsSuccess: false } => string.Empty,
            _ => await payloadStore.FindByFileNameAsync(id.ToString(), cancellationToken)
        };
    }
}