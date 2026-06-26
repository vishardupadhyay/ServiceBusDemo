using Azure.Messaging.ServiceBus;
using Microsoft.AspNetCore.Mvc;

namespace DefaultNamespace;

[ApiController]
[Route("[controller]")]
public class OrdersController : ControllerBase
{
    private readonly IConfiguration _configuration;
    private readonly ServiceBusClient _serviceBusClient;
    private readonly ILogger<OrdersController> _logger;
    
    public OrdersController(IConfiguration configuration, ServiceBusClient serviceBusClient, ILogger<OrdersController> logger)
    {
        _configuration = configuration;
        _serviceBusClient = serviceBusClient;
        _logger = logger;
    }
    
    [HttpPost("publish")]
    public async Task<IActionResult> Publish([FromBody] OrderDetails orderDetails)
    {
        if (orderDetails == null)
        {
            return BadRequest();
        }
        
        var topicName = _configuration["ServiceBus:TopicName"] ?? "mytopic";
        
        ServiceBusSender sender = _serviceBusClient.CreateSender(topicName);
        
        try
        {
            string messageBody = System.Text.Json.JsonSerializer.Serialize(orderDetails);
            ServiceBusMessage serviceBusMessage = new ServiceBusMessage(messageBody)
            {
                ContentType = "application/json",
                MessageId = Guid.NewGuid().ToString()
            };
            
            await sender.SendMessageAsync(serviceBusMessage);

            _logger.LogInformation($"Successfully pushed message to : {sender.EntityPath}.");
            return Ok(new { Status = "Success", MessageId = serviceBusMessage.MessageId });
        }
        catch (Exception ex)
        {
            _logger.LogError("Error");
            return StatusCode(500, new { Message = ex.Message });
        }
    }
}