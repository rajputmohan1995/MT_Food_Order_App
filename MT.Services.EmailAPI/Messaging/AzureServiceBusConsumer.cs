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
    private readonly string registerNewUserQueue;
    private readonly IConfiguration _configuration;
    private readonly EmailService _emailService;

    private ServiceBusProcessor _emailCartProcessor;
    private ServiceBusProcessor _registerNewUserProcessor;

    public AzureServiceBusConsumer(IConfiguration configuration, EmailService emailService)
    {
        _configuration = configuration;
        _emailService = emailService;

        serviceBusConnectionString = _configuration.GetValue<string>("ServiceBusConnectionString");
        emailCartQueue = _configuration.GetValue<string>("TopicAndQueueNames:EmailShoppingCartQueue");
        registerNewUserQueue = _configuration.GetValue<string>("TopicAndQueueNames:RegisterUserQueue");

        var client = new ServiceBusClient(serviceBusConnectionString);
        _emailCartProcessor = client.CreateProcessor(emailCartQueue, new ServiceBusProcessorOptions());
        _registerNewUserProcessor = client.CreateProcessor(registerNewUserQueue, new ServiceBusProcessorOptions());
    }

    public async Task Start()
    {
        _emailCartProcessor.ProcessMessageAsync += OnEmailCartRequestReceived;
        _emailCartProcessor.ProcessErrorAsync += OnErrorHandler;
        await _emailCartProcessor.StartProcessingAsync();

        _registerNewUserProcessor.ProcessMessageAsync += OnRegisterNewUserRequestReceived;
        _registerNewUserProcessor.ProcessErrorAsync += OnErrorHandler;
        await _registerNewUserProcessor.StartProcessingAsync();
    }
    public async Task Stop()
    {
        await _emailCartProcessor.StopProcessingAsync();
        await _emailCartProcessor.DisposeAsync();

        await _registerNewUserProcessor.StopProcessingAsync();
        await _registerNewUserProcessor.DisposeAsync();
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

    private async Task OnRegisterNewUserRequestReceived(ProcessMessageEventArgs args)
    {
        var message = args.Message;
        var body = Encoding.UTF8.GetString(message.Body);

        try
        {
            UserDTO objMessage = JsonConvert.DeserializeObject<UserDTO>(body);
            await _emailService.RegisterUserAndLogAsync(objMessage);
            await args.CompleteMessageAsync(args.Message);
        }
        catch
        {
            throw;
        }
    }

    private Task OnErrorHandler(ProcessErrorEventArgs args)
    {
        Console.WriteLine(args.Exception.ToString());
        return Task.CompletedTask;
    }
}