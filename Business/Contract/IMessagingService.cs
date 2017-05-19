using System.Threading.Tasks;

namespace TrafficLight.Api.Business.Contract
{
    public interface IMessagingService
    {
        Task SendMessage(string messsage);
    }
}