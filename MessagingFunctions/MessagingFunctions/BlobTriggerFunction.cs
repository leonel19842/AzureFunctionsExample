using System;
using System.IO;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.Amqp.Framing;

namespace MessagingFunctions
{
    public static class BlobTriggerFunction
    {
        [FunctionName("BlobTriggerFunction")]
        [StorageAccount("BlobStorageConnectionString")]
        public static void Run([BlobTrigger("%MyBlobContainer%/{name}")] Stream myBlob, string name, ILogger log)
        {
            string connectionString = Environment.GetEnvironmentVariable("AzureWebJobsServicebus");
            string queueName = Environment.GetEnvironmentVariable("QueueName");
            log.LogInformation($"C# Blob trigger function Processed blob\n Name:{name} \n Size: {myBlob.Length} Bytes.");
            myBlob.Seek(0, SeekOrigin.Begin);
            SendMessageAsync(connectionString, queueName, myBlob);
        }

        private static void SendMessageAsync(string serviceBusConnectionString, string queueName, Stream blobContent)
        {
            MemoryStream memoryStream = new MemoryStream();
            blobContent.CopyTo(memoryStream);
            blobContent.Close();
            blobContent.Dispose();
            memoryStream.Seek(0, SeekOrigin.Begin);
            byte[] messageBody = memoryStream.ToArray();
            Message message = new Message(messageBody);
            // create a Service Bus client 
            QueueClient queueClient = new QueueClient(serviceBusConnectionString, queueName, ReceiveMode.PeekLock, null);
            queueClient.SendAsync(message);
        }
    }
}
