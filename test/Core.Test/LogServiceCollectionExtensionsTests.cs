using DataFetcher.Abstractions;

using Microsoft.Extensions.DependencyInjection;

namespace DataFetcher.Core.Test;

public class LogServiceCollectionExtensionsTests
{
    [Fact]
    public void AddLog_AddsServicesAndReturnsBuilder()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        var builder = services.AddLog();

        // Assert
        Assert.NotNull(builder);
        var collection = services.ToList();
        Assert.Equal(typeof(ILogService), collection[0].ServiceType);
        Assert.Equal(typeof(LogService), collection[0].ImplementationType);
        Assert.Equal(ServiceLifetime.Singleton, collection[0].Lifetime);
    }

    [Fact]
    public void AddLog_MultipleCalls_RetainsTypes()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddLog();
        
        // Act
        services.AddLog();
        
        // Assert
        var collection = services.ToList();
        Assert.Single(collection);
        Assert.Single(collection, x => x.ServiceType == typeof(ILogService));
        Assert.Single(collection, x => x.ImplementationType == typeof(LogService));
        Assert.Single(collection, x => x.Lifetime == ServiceLifetime.Singleton);
    }
}