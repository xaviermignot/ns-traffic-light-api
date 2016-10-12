using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TrafficLight.Api.Models;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace TrafficLight.Api.Controllers
{
    [Route("api/[controller]")]
    public class TrafficLightController : Controller
    {
        private static TrafficLightState CurrentTrafficLightState;

        // GET api/values
        [HttpGet]
        public TrafficLightState Get()
        {
            return CurrentTrafficLightState;
        }

        [HttpPut("{state}")]
        public TrafficLightState Set(TrafficLightState state)
        {
            CurrentTrafficLightState = state;

            return CurrentTrafficLightState;
        }
    }
}
