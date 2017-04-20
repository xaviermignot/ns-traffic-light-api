using TrafficLight.Api.Business.Contract;
using TrafficLight.Api.Models;

namespace TrafficLight.Api.Business.Logic
{
    /// <summary>
    /// Implementation of the <see cref="ITrafficLightService"/> interface for managing a single traffic light
    /// </summary>
    public class TrafficLightService : ITrafficLightService
    {
        /// <summary>
        /// The only instance of traffic light managed by the service
        /// </summary>
        private static TrafficLightState CurrentTrafficLightState;

        /// <inheritdoc/>
        public TrafficLightState Get()
        {
            return CurrentTrafficLightState;
        }

        /// <inheritdoc/>
        public void Set(TrafficLightState state)
        {
            CurrentTrafficLightState = state;
        }
    }
}
