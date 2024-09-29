namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// 
/// </summary>
internal sealed class LogBuilder(IServiceCollection services) : ILogBuilder
{
    /// <inheritdoc />
    public IServiceCollection Services { get; } = services;
}