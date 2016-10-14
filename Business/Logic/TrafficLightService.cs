using TrafficLight.Api.Business.Contract;
using TrafficLight.Api.Models;

namespace TrafficLight.Api.Business.Logic
{
    public class TrafficLightService : ITrafficLightService
    {
        private static TrafficLightState CurrentTrafficLightState;

        public TrafficLightState Get()
        {
            return CurrentTrafficLightState;
        }

        public void Set(TrafficLightState state)
        {
            CurrentTrafficLightState = state;
        }
    }
}
