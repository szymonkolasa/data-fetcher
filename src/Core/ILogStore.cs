using DataFetcher.Domain;

namespace DataFetcher.Core;

public interface ILogStore
{
    Task<LogEntry?> FindByIdAsync(Guid id, CancellationToken cancellationToken);
    
    Task CreateAsync(LogEntry logEntry, CancellationToken cancellationToken);
    
    Task<IReadOnlyCollection<LogEntry>> GetLogsBetweenDatesAsync(DateTimeOffset from, DateTimeOffset to, CancellationToken cancellationToken);
}