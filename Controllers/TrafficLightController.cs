using Microsoft.AspNetCore.Mvc;
using TrafficLight.Api.Models;
using TrafficLight.Api.Business.Contract;
using TrafficLight.Api.Business.Logic;
using System;
using Microsoft.AspNetCore.SignalR;
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
        private ITrafficLightService _trafficLightSvc;
        private readonly IHubContext<TrafficLightHub> _hub;
        private readonly IMessagingService _messagingSvc;

        public TrafficLightController(
            IHubContext<TrafficLightHub> hub, 
            ITrafficLightService trafficLightSvc,
            IMessagingService messagingSvc)
        {
            _hub = hub;
            _trafficLightSvc = trafficLightSvc;
            _messagingSvc = messagingSvc;
        }

        /// <summary>
        /// Gets the current state of the traffic light
        /// </summary>
        /// <returns>The current state of the traffic light</returns>
        [HttpGet]
        public TrafficLightState Get()
        {
            return _trafficLightSvc.Get();
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
            _trafficLightSvc.Set(state);

            await _hub.Clients.All.InvokeAsync("UpdateLight", state);

            if (state == TrafficLightState.Broken)
            {
                await _messagingSvc.SendMessage(
                    new Message
                    {
                        Text =  "Le feu est cassé :'(",
                        Type = "Broken"
                    });
            }

            return _trafficLightSvc.Get();
        }

        /// <summary>
        /// Turns the lights off
        /// </summary>
        /// <returns>The resulting state of the traffic light</returns>
        [HttpDelete]
        public TrafficLightState SwitchOff()
        {
            _trafficLightSvc.Set(TrafficLightState.Off);
            _hub.Clients.All.InvokeAsync("UpdateLight", TrafficLightState.Off);

            return _trafficLightSvc.Get();
        }
    }
}