using Microsoft.AspNetCore.Mvc;
using TrafficLight.Api.Models;

namespace TrafficLight.Api.Controllers
{
    [Route("api/[controller]")]
    public class TrafficLightController : Controller
    {
        private static TrafficLightModel CurrentTrafficLight = new TrafficLightModel();

        // GET api/values
        [HttpGet]
        public TrafficLightModel Get()
        {
            return CurrentTrafficLight;
        }

        [HttpPut("set/{color}/{state}")]
        public TrafficLightModel SetLight(LightColor color, LightState state)
        {
            CurrentTrafficLight.SetLight(color, state);

            return CurrentTrafficLight;
        }

        [HttpPost("switch/{color}")]
        public TrafficLightModel SwitchLight(LightColor color)
        {
            CurrentTrafficLight.SwitchLight(color);

            return CurrentTrafficLight;
        }
    }
}
