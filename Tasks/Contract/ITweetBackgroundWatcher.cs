namespace TrafficLight.Api.Tasks.Contract
{
    public interface ITweetBackgroundWatcher
    {
        void StartWatching();
        
        void StopWatching();
    }
}