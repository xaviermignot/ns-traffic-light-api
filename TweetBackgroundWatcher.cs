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

        public string ProactiveMessageTrack { get; set; }

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

                if (TryGetTrafficLightStateFromTrack(matchingTrack, out TrafficLightState lightState))
                {
                    _trafficLightSvc.Value.Set(lightState);
                    _hub.Clients.All.UpdateLight(lightState);
                    return;
                }

                if (matchingTrack.Equals(this.ProactiveMessageTrack, StringComparison.OrdinalIgnoreCase)
                    && _trafficLightSvc.Value.Get() == TrafficLightState.Broken)
                {
                    // Send message in storage queue   
                }
            };

            _stream.StartStreamMatchingAnyCondition();
        }

        public void StopWatching()
        {
            _stream.StopStream();
        }

        private bool TryGetTrafficLightStateFromTrack(string track, out TrafficLightState state)
        {
            if (track.Equals(this.RedLightTrack, StringComparison.OrdinalIgnoreCase))
            {
                state = TrafficLightState.Red;
                return true;
            }

            if (track.Equals(this.OrangeLightTrack, StringComparison.OrdinalIgnoreCase))
            {
                state = TrafficLightState.Orange;
                return true;
            }

            if (track.Equals(this.GreenLightTrack, StringComparison.OrdinalIgnoreCase))
            {
                state = TrafficLightState.Green;
                return true;
            }

            state = TrafficLightState.Off;
            return false;
        }
    }
}
