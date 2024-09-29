using DataFetcher.Abstractions;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;

namespace DataFetcher.Api.AzureFunctions;

public class LogFunctions(ILogService service)
{
    [Function(nameof(FetchLogAsync))]
    public Task FetchLogAsync(
        [TimerTrigger("0 */1 * * * *")] TimerInfo timerInfo,
        FunctionContext context) => service.FetchDataAsync(context.CancellationToken);

    [Function(nameof(GetLogsAsync))]
    public async Task<IActionResult> GetLogsAsync(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "logs")] HttpRequestData req,
        DateTimeOffset from,
        DateTimeOffset to,
        FunctionContext context)
    {
        try
        {
            return new OkObjectResult(await service.GetLogsAsync(from, to, context.CancellationToken));
        }
        catch (ArgumentException e)
        {
            return new BadRequestObjectResult(e.Message);
        }
    }

    [Function(nameof(GetPayloadByLogIdAsync))]
    public async Task<IActionResult> GetPayloadByLogIdAsync(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "logs/{id:guid}/payload")] HttpRequestData req,
        Guid id,
        FunctionContext context)
    {
        try
        {
            return new OkObjectResult(await service.GetPayloadByLogIdAsync(id, context.CancellationToken));
        }
        catch (LogNotFoundException)
        {
            return new NotFoundResult();
        }
    }
}