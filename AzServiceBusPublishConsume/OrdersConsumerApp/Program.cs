
using Azure.Messaging.ServiceBus;

var client = new ServiceBusClient("Endpoint=sb://127.0.0.1;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=SAS_KEY_VALUE;UseDevelopmentEmulator=true;");
var processor = client.CreateProcessor("queue.1");
try
{
    processor.ProcessMessageAsync += MessageHandler;
    processor.ProcessErrorAsync += ErrorHandler;

    await processor.StartProcessingAsync();
    Console.WriteLine("Wait for a min and then press any key to end the processing!");
    Console.ReadKey();

    Console.WriteLine("Stopping the receiver");
    await processor.StartProcessingAsync();
    Console.WriteLine("Stopped receiving messages");

}
finally
{
    await client.DisposeAsync();
}

async Task MessageHandler(ProcessMessageEventArgs args)
{
    var body = args.Message.Body.ToString();
    Console.WriteLine($"Received: {body}");
    await args.CompleteMessageAsync(args.Message);
}

Task ErrorHandler(ProcessErrorEventArgs args)
{
    Console.WriteLine(args.Exception.ToString() );
    return Task.CompletedTask;
}