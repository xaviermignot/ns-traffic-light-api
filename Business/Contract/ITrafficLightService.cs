using TrafficLight.Api.Models;

namespace TrafficLight.Api.Business.Contract
{
    /// <summary>
    /// Describe the contract of the  traffic light management service
    /// </summary>
    public interface ITrafficLightService
    {
        /// <summary>
        /// Returns the current state of the traffic light
        /// </summary>
        /// <returns>The current state of the traffic light</returns>
        TrafficLightState Get();

        /// <summary>
        /// Changes the state of the traffic light
        /// </summary>
        /// <param name="state">The new state of the traffic light</param>
        void Set(TrafficLightState state);
    }
}
