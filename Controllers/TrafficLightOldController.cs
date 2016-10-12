using Microsoft.AspNetCore.Mvc;
using TrafficLight.Api.Models;

namespace TrafficLight.Api.Controllers
{
    [Route("api/[controller]")]
    public class TrafficLightOldController : Controller
    {
        private static TrafficLightModel CurrentTrafficLight = new TrafficLightModel();

        // GET api/values
        [HttpGet]
        public TrafficLightModel Get()
        {
            return CurrentTrafficLight;
        }

        [HttpPut("{color}/{state}")]
        public TrafficLightModel SetLight(LightColor color, LightState state)
        {
            CurrentTrafficLight.SetLight(color, state);

            return CurrentTrafficLight;
        }

        [HttpPut("{color}")]
        public TrafficLightModel SwitchToLight(LightColor color)
        {
            CurrentTrafficLight.SwitchLight(color);

            return CurrentTrafficLight;
        }
    }
}
