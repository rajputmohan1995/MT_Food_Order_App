using Azure.Messaging.ServiceBus;
using MT.Services.EmailAPI.Messaging.Interface;
using MT.Services.EmailAPI.Models;
using Newtonsoft.Json;
using System.Text;

namespace MT.Services.EmailAPI.Messaging;

public class AzureServiceBusConsumer : IAzureServiceBusConsumer
{
    public readonly string serviceBusConnectionString;
    public readonly string emailCartQueue;
    public readonly IConfiguration _configuration;
    private ServiceBusProcessor _emailCartProcessor;

    public AzureServiceBusConsumer(IConfiguration configuration)
    {
        _configuration = configuration;

        serviceBusConnectionString = _configuration.GetValue<string>("ServiceBusConnectionString");
        emailCartQueue = _configuration.GetValue<string>("TopicAndQueueNames:EmailShoppingCartQueue");

        var client = new ServiceBusClient(serviceBusConnectionString);
        _emailCartProcessor = client.CreateProcessor(emailCartQueue, new ServiceBusProcessorOptions());
    }

    public async Task Start()
    {
        _emailCartProcessor.ProcessMessageAsync += OnEmailCartRequestReceived;
        _emailCartProcessor.ProcessErrorAsync += OnEmailCartErrorHandler;
        await _emailCartProcessor.StartProcessingAsync();
    }
    public async Task Stop()
    {
        await _emailCartProcessor.StopProcessingAsync();
        await _emailCartProcessor.DisposeAsync();
    }

    private async Task OnEmailCartRequestReceived(ProcessMessageEventArgs args)
    {
        var message = args.Message;
        var body = Encoding.UTF8.GetString(message.Body);

        try
        {
            ShoppingCartDTO objMessage = JsonConvert.DeserializeObject<ShoppingCartDTO>(body);
            await args.CompleteMessageAsync(args.Message);
        }
        catch
        {
            throw;
        }
    }
    private Task OnEmailCartErrorHandler(ProcessErrorEventArgs args)
    {
        Console.WriteLine(args.Exception.ToString());
        return Task.CompletedTask;
    }
}