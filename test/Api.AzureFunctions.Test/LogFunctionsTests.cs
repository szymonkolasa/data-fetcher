using DataFetcher.Abstractions;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;

using Moq;

namespace DataFetcher.Api.AzureFunctions.Test;

public class LogFunctionsTests
{
    private readonly Mock<ILogService> _serviceMock;
    private readonly Mock<FunctionContext> _functionContextMock;
    private readonly LogFunctions _sut;
    
    public LogFunctionsTests()
    {
        _serviceMock = new Mock<ILogService>();
        _functionContextMock = new Mock<FunctionContext>();
        _sut = new LogFunctions(_serviceMock.Object);
    }
    
    [Fact]
    public async Task FetchLogAsync_CallsFetchDataAsync()
    {
        // Arrange
        var token = new CancellationToken();
        var timerInfo = new Mock<TimerInfo>().Object;
        
        _functionContextMock
            .SetupGet(x => x.CancellationToken)
            .Returns(token);

        // Act
        await _sut.FetchLogAsync(timerInfo, _functionContextMock.Object);

        // Assert
        _serviceMock.Verify(x => x.FetchDataAsync(token), Times.Once);
    }

    public static IReadOnlyCollection<object[]> GetLogsAsyncInvalidParametersReturnsBadRequestData =
    [
        [DateTimeOffset.Now, DateTimeOffset.Now.AddMinutes(-1)],
        [DateTimeOffset.Now.AddMinutes(1), DateTimeOffset.Now]
    ];

    [Theory]
    [MemberData(nameof(GetLogsAsyncInvalidParametersReturnsBadRequestData))]
    public async Task GetLogsAsync_InvalidParameters_ReturnsBadRequest(DateTimeOffset from, DateTimeOffset to)
    {
        // Arrange
        var token = new CancellationToken();
        var request = new Mock<HttpRequestData>(_functionContextMock.Object).Object;
        
        _functionContextMock
            .SetupGet(x => x.CancellationToken)
            .Returns(token);

        _serviceMock
            .Setup(x => x.GetLogsAsync(from, to, token))
            .ThrowsAsync(new ArgumentException("From date cannot be later than to date."));
        
        // Act
        var response = await _sut.GetLogsAsync(request, from, to, _functionContextMock.Object);
        
        // Assert
        Assert.IsType<BadRequestObjectResult>(response);
        var badRequest = (BadRequestObjectResult)response;
        Assert.Equal(StatusCodes.Status400BadRequest, badRequest.StatusCode);
        Assert.Equal("From date cannot be later than to date.", badRequest.Value);
        
        _serviceMock.Verify(x => x.GetLogsAsync(It.IsAny<DateTimeOffset>(), It.IsAny<DateTimeOffset>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    public static IReadOnlyCollection<object[]> GetLogsAsyncValidParametersReturnsLogsData =
    [
        [],
        [new LogResponse(Guid.NewGuid(), DateTimeOffset.Now, true)],
        [new LogResponse(Guid.NewGuid(), DateTimeOffset.Now, true), new LogResponse(Guid.NewGuid(), DateTimeOffset.Now, false)]
    ];
    
    [Theory]
    [MemberData(nameof(GetLogsAsyncValidParametersReturnsLogsData))]
    public async Task GetLogsAsync_ValidParameters_ReturnsLogs(params LogResponse[] expectedResponse)
    {
        // Arrange
        var from = DateTimeOffset.Now;
        var to = DateTimeOffset.Now.AddYears(1);
        var token = new CancellationToken();
        var request = new Mock<HttpRequestData>(_functionContextMock.Object).Object;
        
        _functionContextMock
            .SetupGet(x => x.CancellationToken)
            .Returns(token);

        _serviceMock
            .Setup(x => x.GetLogsAsync(from, to, token))
            .ReturnsAsync(expectedResponse);
        
        // Act
        var response = await _sut.GetLogsAsync(request, from, to, _functionContextMock.Object);
        
        // Assert
        Assert.IsType<OkObjectResult>(response);
        var okResult = (OkObjectResult)response;
        Assert.IsAssignableFrom<IReadOnlyCollection<LogResponse>>(okResult.Value);
        var responseData = (IEnumerable<LogResponse>)okResult.Value!;
        Assert.Equal(StatusCodes.Status200OK, okResult.StatusCode);
        Assert.All(expectedResponse, x => Assert.Contains(x, responseData));
        
        _serviceMock.Verify(x => x.GetLogsAsync(It.IsAny<DateTimeOffset>(), It.IsAny<DateTimeOffset>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetPayloadByLogIdAsync_UnknownId_ReturnsNotFound()
    {
        // Arrange
        var token = new CancellationToken();
        var id = Guid.NewGuid();
        var request = new Mock<HttpRequestData>(_functionContextMock.Object).Object;
        
        _functionContextMock
            .SetupGet(x => x.CancellationToken)
            .Returns(token);
        
        _serviceMock
            .Setup(x => x.GetPayloadByLogIdAsync(id, token))
            .ThrowsAsync(new LogNotFoundException());
        
        // Act
        var response = await _sut.GetPayloadByLogIdAsync(request, id, _functionContextMock.Object);
        
        // Assert
        Assert.IsType<NotFoundResult>(response);
        var notFoundResponse = (NotFoundResult)response;
        Assert.Equal(StatusCodes.Status404NotFound, notFoundResponse.StatusCode);
        
        _serviceMock.Verify(x => x.GetPayloadByLogIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetPayloadByLogIdAsync_ValidId_ReturnsPayload()
    {
        // Arrange
        var token = new CancellationToken();
        var id = Guid.NewGuid();
        var payload = "Hello, world!";
        var request = new Mock<HttpRequestData>(_functionContextMock.Object).Object;
        
        _functionContextMock
            .SetupGet(x => x.CancellationToken)
            .Returns(token);

        _serviceMock
            .Setup(x => x.GetPayloadByLogIdAsync(id, token))
            .ReturnsAsync(payload);
        
        // Act
        var response = await _sut.GetPayloadByLogIdAsync(request, id, _functionContextMock.Object);
        
        // Assert
        Assert.IsType<OkObjectResult>(response);
        var okResult = (OkObjectResult)response;
        Assert.Equal(StatusCodes.Status200OK, okResult.StatusCode);
        Assert.Equal(payload, okResult.Value);
        
        _serviceMock.Verify(x => x.GetPayloadByLogIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Once);
    }
}