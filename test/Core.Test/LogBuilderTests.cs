using Microsoft.Extensions.DependencyInjection;

namespace DataFetcher.Core.Test;

public class LogBuilderTests
{
    [Fact]
    public void Constructor_CreatesInstance()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        var builder = new LogBuilder(services);

        // Assert
        Assert.NotNull(builder);
        Assert.Equal(services, builder.Services);
    }
}