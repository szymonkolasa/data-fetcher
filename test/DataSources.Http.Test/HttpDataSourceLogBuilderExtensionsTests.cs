using DataFetcher.Core;

using Microsoft.Extensions.DependencyInjection;

namespace DataFetcher.DataSources.Http.Test;

public class HttpDataSourceLogBuilderExtensionsTests
{
    [Fact]
    public void AddHttpDataSource_AddsServices()
    {
        // Arrange
        IServiceCollection services = new ServiceCollection();
        var builder = services.AddLog();

        // Act
        var builderResult = builder.AddHttpDataSource(client => { client.BaseAddress = new Uri("http://localhost:5001"); });

        // Assert
        Assert.NotNull(builderResult);
        var collection = services.ToList();
        Assert.Single(collection, x => x.ServiceType == typeof(IDataSource));
    }
}