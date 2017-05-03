using System;
using System.Linq;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.SignalR.Infrastructure;
using TrafficLight.Api.Business.Contract;
using TrafficLight.Api.Business.Logic;
using TrafficLight.Api.Hubs;
using TrafficLight.Api.Models;
using Tweetinvi;
using Tweetinvi.Streaming;

namespace TrafficLight.Api
{
    internal class TweetBackgroundWatcher
    {
        internal static Lazy<TweetBackgroundWatcher> LazyInstance;

        private readonly IHubContext _hub;

        private Lazy<ITrafficLightService> _trafficLightSvc = new Lazy<ITrafficLightService>(() => new TrafficLightService());

        public string RedLightTrack { get; set; }

        public string OrangeLightTrack { get; set; }

        public string GreenLightTrack { get; set; }

        private IFilteredStream _stream;

        private TweetBackgroundWatcher(IConnectionManager connectionManager)
        {
            _hub = connectionManager.GetHubContext<TrafficLightHub>();
        }

        public static void Initialize(IConnectionManager connectionManager)
        {
            LazyInstance = new Lazy<TweetBackgroundWatcher>(
                () => new TweetBackgroundWatcher(connectionManager));
        }

        public void StartWatching()
        {
            _stream = Stream.CreateFilteredStream();
            _stream.AddTrack(RedLightTrack);
            _stream.AddTrack(OrangeLightTrack);
            _stream.AddTrack(GreenLightTrack);

            _stream.MatchingTweetReceived += (sender, args) =>
            {
                var matchingTrack = args.MatchingTracks?.FirstOrDefault();
                var lightState = GetTrafficLightStateFromTrack(matchingTrack);
                _trafficLightSvc.Value.Set(lightState);
                _hub.Clients.All.UpdateLight(lightState);
            };

            _stream.StartStreamMatchingAnyCondition();
        }

        public void StopWatching()
        {
            _stream.StopStream();
        }

        private TrafficLightState GetTrafficLightStateFromTrack(string track)
        {
            if (track.Equals(this.RedLightTrack, StringComparison.Ordinal))
            {
                return TrafficLightState.Red;
            }

            if (track.Equals(this.OrangeLightTrack, StringComparison.OrdinalIgnoreCase))
            {
                return TrafficLightState.Orange;
            }

            if (track.Equals(this.GreenLightTrack, StringComparison.OrdinalIgnoreCase))
            {
                return TrafficLightState.Green;
            }

            throw new NotSupportedException($"The {track} value is not supported by the TweetBackgroundWatcher class");
        }
    }
}
