using Microsoft.Azure.ServiceBus;
using SBShared.Models;
using System;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace SBReceiver
{
    class Program
    {
        const string connectionString = "Endpoint=sb://mpiwkoservicebus.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=ulBFMeshAAK3VgXMPtlLZ6x+SKAfNcL3mIqIh+8CNm0=";
        const string queueName = "personqueue";
        static IQueueClient queueClient;

        static async Task Main(string[] args)
        {
            queueClient = new QueueClient(connectionString, queueName);

            var messageHandlerOptions = new MessageHandlerOptions(ExceptionReceiverHandler)
            {
                MaxConcurrentCalls = 1, //it says we are going to process one message at a time,
                                        //you could process multiple and if you have lots of processors, lots of thread available to you
                                        //there might be a case where you can bump this up to e.g. 10/20/50/..
                AutoComplete = false  // this will not mark the message, this means when it tiggeres there is a new message; we are not going to say its complete.
                                       // We are goin to wait until we read the message and as long as it  successfully read the message 
                                       // we are going to say; yes complete this message and thats where the 30second lock timer comes in to do it before
                                       // it gets unlocked and allowed to get pulled down by somebody else
            };

            queueClient.RegisterMessageHandler(ProcessMessagesAsync, messageHandlerOptions);

            Console.ReadLine();

            await queueClient.CloseAsync();
        }

        private static async Task ProcessMessagesAsync(Message message, CancellationToken token)
        {
            var jsonString = Encoding.UTF8.GetString(message.Body);
            PersonModel person = JsonSerializer.Deserialize<PersonModel>(jsonString);
            Console.WriteLine($"Person Received: { person.FirstName } {person.LastName }");

            // we are passing the token in and saying the item that has this 30s lock token is completed, so the msg from the queue is being removed
            await queueClient.CompleteAsync(message.SystemProperties.LockToken);
        }

        private static Task ExceptionReceiverHandler(ExceptionReceivedEventArgs arg)
        {
            Console.WriteLine($"Message handler exception: { arg.Exception}");
            return Task.CompletedTask;
        }
    }
}
