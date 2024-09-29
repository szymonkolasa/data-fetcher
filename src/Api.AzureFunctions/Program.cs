using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;

var host = new HostBuilder()
    .ConfigureFunctionsWebApplication()
    .ConfigureServices((host, services) =>
    {
        services.AddAzureClients(builder =>
        {
            builder.AddBlobServiceClient(host.Configuration.GetConnectionString("Azure:Blobs"));
            builder.AddTableServiceClient(host.Configuration.GetConnectionString("Azure:Tables"));
        });
        
        services.AddApplicationInsightsTelemetryWorkerService();
        services.ConfigureFunctionsApplicationInsights();

        services
            .AddLog()
            .AddHttpDataSource(client =>
            {
                client.BaseAddress = new Uri("https://official-joke-api.appspot.com/random_joke");
            })
            .AddAzureStorageBlobsPayloadStore()
            .AddAzureDataTablesLogStore();
    })
    .Build();

host.Run();