using System.Threading.Tasks;
using TrafficLight.Api.Models;

namespace TrafficLight.Api.Business.Contract
{
    public interface IMessagingService
    {
        Task SendMessage(Message message);
    }
}