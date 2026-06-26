using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace DefaultNamespace;

public class OrderProcessor
{
    private readonly ILogger<OrderProcessor>  _logger;

    public OrderProcessor(ILogger<OrderProcessor> logger)
    {
        _logger = logger;
    }
    
    [Function(nameof(OrderProcessor))]
    public void RunAsync(
        [ServiceBusTrigger("topic.1", "subscription.3", Connection = "ServiceBusConnectionString")] string messageBody,
        FunctionContext context)
    {
        _logger.LogInformation("Processing incoming message from Service Bus Topic...");
        _logger.LogInformation("Message Content: {Body}", messageBody);
    }
}