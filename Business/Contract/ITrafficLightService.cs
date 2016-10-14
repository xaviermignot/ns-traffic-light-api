using TrafficLight.Api.Models;

namespace TrafficLight.Api.Business.Contract
{
    interface ITrafficLightService
    {
        TrafficLightState Get();

        void Set(TrafficLightState state);
    }
}
