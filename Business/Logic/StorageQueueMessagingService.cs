using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;
using Newtonsoft.Json;
using TrafficLight.Api.Business.Contract;
using TrafficLight.Api.Configuration;
using TrafficLight.Api.Models;

namespace TrafficLight.Api.Business.Logic
{
    public class StorageQueueMessagingService : IMessagingService
    {
        private readonly AzureSettings _azureSettings;

        public StorageQueueMessagingService(IOptions<AzureSettings> azureSection)
        {
            _azureSettings = azureSection.Value;
        }

        public async Task SendMessage(Message message)
        {
            var storageAccount = CloudStorageAccount.Parse(_azureSettings.StorageConnectionString);
            var queueClient = storageAccount.CreateCloudQueueClient();
            var queue = queueClient.GetQueueReference("proactive-messages");
            await queue.CreateIfNotExistsAsync();
            await queue.AddMessageAsync(new CloudQueueMessage(SerializeMessage(message)));
        }

        private static string SerializeMessage(Message message)
        {
            return JsonConvert.SerializeObject(message);
        }
    }
}