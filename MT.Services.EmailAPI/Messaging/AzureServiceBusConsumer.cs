using Azure.Messaging.ServiceBus;
using MT.Services.EmailAPI.Messaging.Interface;
using MT.Services.EmailAPI.Models;
using MT.Services.EmailAPI.Services;
using Newtonsoft.Json;
using System.Text;

namespace MT.Services.EmailAPI.Messaging;

public class AzureServiceBusConsumer : IAzureServiceBusConsumer
{
    private readonly string serviceBusConnectionString;
    private readonly string emailCartQueue;
    private readonly IConfiguration _configuration;
    private readonly EmailService _emailService;

    private ServiceBusProcessor _emailCartProcessor;

    public AzureServiceBusConsumer(IConfiguration configuration, EmailService emailService)
    {
        _configuration = configuration;
        _emailService = emailService;

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
            await _emailService.EmailCartAndLogAsync(objMessage);
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