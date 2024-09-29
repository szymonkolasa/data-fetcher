using DataFetcher.Abstractions;
using DataFetcher.Domain;

using Moq;

namespace DataFetcher.Core.Test;

public class LogServiceTests
{
    private readonly Mock<IDataSource> _dataSourceMock;
    private readonly Mock<ILogStore> _logStoreMock;
    private readonly Mock<IPayloadStore> _payloadStoreMock;
    private readonly LogService _sut;
    
    public LogServiceTests()
    {
        _dataSourceMock = new Mock<IDataSource>();
        _logStoreMock = new Mock<ILogStore>();
        _payloadStoreMock = new Mock<IPayloadStore>();
        _sut = new LogService(_dataSourceMock.Object, _logStoreMock.Object, _payloadStoreMock.Object);
    }

    [Fact]
    public async Task FetchDataAsync_FailedResponse_ShouldStoreDataWithoutPayload()
    {
        // Arrange
        var token = new CancellationToken();
        var response = new DataSourceResponse(false, null);

        _dataSourceMock
            .Setup(x => x.FetchAsync(token))
            .ReturnsAsync(response);
        
        // Act
        await _sut.FetchDataAsync(token);

        // Assert
        _payloadStoreMock.Verify(x => x.CreateAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
        _logStoreMock.Verify(x => x.CreateAsync(It.IsAny<LogEntry>(), token), Times.Once);
    }

    [Fact]
    public async Task FetchDataAsync_SuccessResponse_ShouldStoreDataAndPayload()
    {
        // Arrange
        var token = new CancellationToken();
        var response = new DataSourceResponse(true, "Payload");
        
        _dataSourceMock
            .Setup(x => x.FetchAsync(token))
            .ReturnsAsync(response);
        
        // Act
        await _sut.FetchDataAsync(token);
        
        // Assert
        _payloadStoreMock.Verify(x => x.CreateAsync(It.IsAny<string>(), It.IsAny<string>(), token), Times.Once);
        _logStoreMock.Verify(x => x.CreateAsync(It.IsAny<LogEntry>(), token), Times.Once);
    }

    public static IReadOnlyCollection<object[]> GetLogsAsyncInvalidDateThrowsArgumentExceptionData =
    [
        [DateTimeOffset.Now, DateTimeOffset.Now.AddMinutes(-1)],
        [DateTimeOffset.Now.AddMinutes(1), DateTimeOffset.Now]
    ];

    [Theory]
    [MemberData(nameof(GetLogsAsyncInvalidDateThrowsArgumentExceptionData))]
    public async Task GetLogsAsync_InvalidDate_ThrowsArgumentException(DateTimeOffset from, DateTimeOffset to)
    {
        // Arrange
        var token = new CancellationToken();

        // Act
        // Assert
        var exception = await Assert.ThrowsAsync<ArgumentException>(() => _sut.GetLogsAsync(from, to, token));
        Assert.Equal("From date cannot be later than to date.", exception.Message);
        
        _logStoreMock.Verify(x => x.GetLogsBetweenDatesAsync(It.IsAny<DateTimeOffset>(), It.IsAny<DateTimeOffset>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    public static IReadOnlyCollection<object[]> GetLogsAsyncValidDatesReturnsLogsData =>
    [
        [],
        [new LogResponse(Guid.NewGuid(), DateTimeOffset.Now, true)],
        [new LogResponse(Guid.NewGuid(), DateTimeOffset.Now, true), new LogResponse(Guid.NewGuid(), DateTimeOffset.Now.AddMinutes(1), false)]
    ];

    [Theory]
    [MemberData(nameof(GetLogsAsyncValidDatesReturnsLogsData))]
    public async Task GetLogsAsync_ValidDates_ReturnsLogs(params LogResponse[] expectedLogs)
    {
        // Arrange
        var token = new CancellationToken();
        var from = DateTimeOffset.Now;
        var to = DateTimeOffset.Now.AddMonths(1);

        var data = expectedLogs
            .Select(x => new LogEntry(x.Id, x.FetchDate, x.IsSuccess))
            .ToList();

        _logStoreMock
            .Setup(x => x.GetLogsBetweenDatesAsync(from, to, token))
            .ReturnsAsync(data);
        
        // Act
        var response = await _sut.GetLogsAsync(from, to, token);
        
        // Assert
        Assert.NotNull(response);
        Assert.All(expectedLogs, x => Assert.Contains(x, response));
        
        _logStoreMock.Verify(x => x.GetLogsBetweenDatesAsync(from, to, token), Times.Once);
    }

    [Fact]
    public async Task GetPayloadByLogIdAsync_FailedResponse_ReturnsEmptyPayload()
    {
        // Arrange
        var id = Guid.NewGuid();
        var token = new CancellationToken();
        
        _logStoreMock
            .Setup(x => x.FindByIdAsync(id, token))
            .ReturnsAsync(new LogEntry(id, DateTimeOffset.Now, false));
        
        // Act
        var response = await _sut.GetPayloadByLogIdAsync(id, token);

        // Assert
        Assert.Empty(response);
        _logStoreMock.Verify(x => x.FindByIdAsync(id, token), Times.Once);
        _payloadStoreMock.Verify(x => x.FindByFileNameAsync(id.ToString(), token), Times.Never);
    }
}