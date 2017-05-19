using System;
using System.Linq;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.SignalR.Infrastructure;
using Microsoft.Extensions.Options;
using TrafficLight.Api.Business.Contract;
using TrafficLight.Api.Business.Logic;
using TrafficLight.Api.Configuration;
using TrafficLight.Api.Hubs;
using TrafficLight.Api.Models;
using TrafficLight.Api.Tasks.Contract;
using Tweetinvi;
using Tweetinvi.Streaming;

namespace TrafficLight.Api.Tasks.Logic
{
    internal class TweetBackgroundWatcher : ITweetBackgroundWatcher
    {
        private readonly IHubContext _hub;

        private readonly ITrafficLightService _trafficLightSvc;

        private readonly IMessagingService _messagingSvc;

        private readonly TwitterSettings _twitterSettings;

        private IFilteredStream _stream;

        public TweetBackgroundWatcher(
            IConnectionManager connectionManager,
            ITrafficLightService trafficLightSvc,
            IMessagingService messagingSvc,
            IOptions<TwitterSettings> twitterSettings)
        {
            _hub = connectionManager.GetHubContext<TrafficLightHub>();
            _trafficLightSvc = trafficLightSvc;
            _messagingSvc = messagingSvc;
            _twitterSettings = twitterSettings.Value;
        }

        public void StartWatching()
        {
            _stream = Stream.CreateFilteredStream();
            _stream.AddTrack(_twitterSettings.RedLight);
            _stream.AddTrack(_twitterSettings.OrangeLight);
            _stream.AddTrack(_twitterSettings.GreenLight);
            _stream.AddTrack(_twitterSettings.ProactiveMessage);

            _stream.MatchingTweetReceived += (sender, args) =>
            {
                var matchingTrack = args.MatchingTracks?.FirstOrDefault();

                // Change the light from Twitter only if the light is not "broken"
                if (_trafficLightSvc.Get() != TrafficLightState.Broken
                    && TryGetTrafficLightStateFromTrack(matchingTrack, out TrafficLightState lightState))
                {
                    _trafficLightSvc.Set(lightState);
                    _hub.Clients.All.UpdateLight(lightState);
                    return;
                }

                // Send proactive message though storage queue
                if (matchingTrack.Equals(_twitterSettings.ProactiveMessage, StringComparison.OrdinalIgnoreCase))
                {
                    _messagingSvc.SendMessage(args.Tweet.Text);
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
            if (track.Equals(_twitterSettings.RedLight, StringComparison.OrdinalIgnoreCase))
            {
                state = TrafficLightState.Red;
                return true;
            }

            if (track.Equals(_twitterSettings.OrangeLight, StringComparison.OrdinalIgnoreCase))
            {
                state = TrafficLightState.Orange;
                return true;
            }

            if (track.Equals(_twitterSettings.GreenLight, StringComparison.OrdinalIgnoreCase))
            {
                state = TrafficLightState.Green;
                return true;
            }

            state = TrafficLightState.Off;
            return false;
        }
    }
}
