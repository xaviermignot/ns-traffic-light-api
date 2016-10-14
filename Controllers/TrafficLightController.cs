using Microsoft.AspNetCore.Mvc;
using TrafficLight.Api.Models;

namespace TrafficLight.Api.Controllers
{
    /// <summary>
    /// Traffic light management controller
    /// </summary>
    [Route("api/[controller]")]
    public class TrafficLightController : Controller
    {
        /// <summary>
        /// Static instance of the only traffic light managed by the api
        /// </summary>
        private static TrafficLightState CurrentTrafficLightState;

        /// <summary>
        /// Gets the current state of the traffic light
        /// </summary>
        /// <returns>The current state of the traffic light</returns>
        [HttpGet]
        public TrafficLightState Get()
        {
            return CurrentTrafficLightState;
        }

        /// <summary>
        ///  Turns a light on
        /// </summary>
        /// <param name="state">The light to turn on</param>
        /// <returns>The resulting state of the traffic light</returns>
        [HttpPut("{state}")]
        public TrafficLightState SwitchOn(TrafficLightState state)
        {
            //TODO: return bad request if state == Off
            CurrentTrafficLightState = state;

            return CurrentTrafficLightState;
        }

        /// <summary>
        /// Turns the lights off
        /// </summary>
        /// <returns>The resulting state of the traffic light</returns>
        [HttpDelete]
        public TrafficLightState SwitchOff()
        {
            CurrentTrafficLightState = TrafficLightState.Off;

            return CurrentTrafficLightState;
        }
    }
}
