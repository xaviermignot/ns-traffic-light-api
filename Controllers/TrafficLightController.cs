using Microsoft.AspNetCore.Mvc;
using TrafficLight.Api.Models;
using TrafficLight.Api.Business.Contract;
using TrafficLight.Api.Business.Logic;
using System;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.SignalR.Infrastructure;
using TrafficLight.Api.Hubs;
using Microsoft.Extensions.Options;
using TrafficLight.Api.Configuration;
using Microsoft.WindowsAzure.Storage;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage.Queue;
using System.Text;

namespace TrafficLight.Api.Controllers
{
    /// <summary>
    /// Traffic light management controller
    /// </summary>
    [Route("api/[controller]")]
    public class TrafficLightController : Controller
    {
        private Lazy<ITrafficLightService> _trafficLightSvc = new Lazy<ITrafficLightService>(() => new TrafficLightService());
        private readonly IHubContext _hub;
        private readonly AzureSettings _azureSettings;

        public TrafficLightController(IConnectionManager signalRConnectionManager, IOptions<AzureSettings> azureSection)
        {
            _hub = signalRConnectionManager.GetHubContext<TrafficLightHub>();
            _azureSettings = azureSection.Value;
        }

        /// <summary>
        /// Gets the current state of the traffic light
        /// </summary>
        /// <returns>The current state of the traffic light</returns>
        [HttpGet]
        public TrafficLightState Get()
        {
            return _trafficLightSvc.Value.Get();
        }

        /// <summary>
        ///  Turns a light on
        /// </summary>
        /// <param name="state">The light to turn on</param>
        /// <returns>The resulting state of the traffic light</returns>
        [HttpPut("{state}")]
        public async Task<TrafficLightState> SwitchOn(TrafficLightState state)
        {
            //TODO: return bad request if state == Off
            _trafficLightSvc.Value.Set(state);

            _hub.Clients.All.UpdateLight(state);

            if (state == TrafficLightState.Broken)
            {
                var storageAccount = CloudStorageAccount.Parse(_azureSettings.StorageConnectionString);
                var queueClient = storageAccount.CreateCloudQueueClient();
                var queue = queueClient.GetQueueReference("proactive-messages");
                await queue.CreateIfNotExistsAsync();
                await queue.AddMessageAsync(new CloudQueueMessage("Le feu est cassé :'("));
            }

            return _trafficLightSvc.Value.Get();
        }

        /// <summary>
        /// Turns the lights off
        /// </summary>
        /// <returns>The resulting state of the traffic light</returns>
        [HttpDelete]
        public TrafficLightState SwitchOff()
        {
            _trafficLightSvc.Value.Set(TrafficLightState.Off);
            _hub.Clients.All.UpdateLight(TrafficLightState.Off);

            return _trafficLightSvc.Value.Get();
        }
    }
}