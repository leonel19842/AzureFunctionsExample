using System;
using System.Data;
using System.Data.SqlClient;
using System.Xml.Linq;
using System.Xml.XPath;
using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;


namespace MessagingFunctions
{
    public static class QueueAzureBusTrigger
    {
        [FunctionName("QueueAzureBusTrigger")]
        [StorageAccount("AzureWebJobsServicebus")]
        public static void Run([ServiceBusTrigger("%QueueName%")] string myQueueItem, string messageId, ILogger log)
        {
            log.LogInformation($"C# ServiceBus queue trigger function processed message: {myQueueItem} with Id: {messageId}");
            InsertDataToBd(myQueueItem);
            
        }

        private static void InsertDataToBd(string myQueueItem)
        {
            XDocument document = XDocument.Parse(myQueueItem);
            string messageId = document.XPathSelectElement("mail").Attribute("Identificador").Value.ToString();
            string sender = document.XPathSelectElement("mail/from").Value.ToString();
            string recipient = document.XPathSelectElement("mail/to").Value.ToString();
            string connectionString = Environment.GetEnvironmentVariable("SqlConnectionString");
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand())
                {
                    command.Connection = connection;            // <== lacking
                    command.CommandType = CommandType.Text;
                    command.CommandText = "INSERT into MessagesProcessed (MessageId, MessageText, Sender,Recipient) VALUES (@messageId, @messageText, @sender, @recipient)";
                    command.Parameters.AddWithValue("@messageId", new Guid(messageId));
                    command.Parameters.AddWithValue("@messageText", document.Document.ToString());
                    command.Parameters.AddWithValue("@sender", sender);
                    command.Parameters.AddWithValue("@recipient", recipient);

                    try
                    {
                        connection.Open();
                        int recordsAffected = command.ExecuteNonQuery();
                    }
                    catch (SqlException ex)
                    {
                        throw new Exception("Error al insertar en BD: " + ex.Message);
                    }
                    finally
                    {
                        connection.Close();
                    }
                }
            }
        }
    }
}
