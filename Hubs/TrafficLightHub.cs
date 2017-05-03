using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.SignalR.Hubs;
using TrafficLight.Api.Models;

namespace TrafficLight.Api.Hubs
{
    public class TrafficLightHub : Hub<IBroadcaster>
    {
        public override Task OnConnected()
        {
            // Set connection id for just connected client only
            return Clients.Client(Context.ConnectionId).SetConnectionId(Context.ConnectionId);
        }
    }

    public interface IBroadcaster
    {
        Task SetConnectionId(string connectionId);

        Task UpdateLight(TrafficLightState state);
    }
}