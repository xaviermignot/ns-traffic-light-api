using System;
using System.Linq;
using TrafficLight.Api.Business.Contract;
using TrafficLight.Api.Business.Logic;
using TrafficLight.Api.Models;
using Tweetinvi;
using Tweetinvi.Streaming;

namespace TrafficLight.Api
{
    internal class TweetBackgroundWatcher
    {
        internal static Lazy<TweetBackgroundWatcher> LazyInstance = new Lazy<TweetBackgroundWatcher>(
            () => new TweetBackgroundWatcher());

        private Lazy<ITrafficLightService> _trafficLightSvc = new Lazy<ITrafficLightService>(() => new TrafficLightService());

        public string RedLightTrack { get; set; }

        public string OrangeLightTrack { get; set; }

        public string GreenLightTrack { get; set; }

        private IFilteredStream _stream;

        public void StartWatching()
        {
            _stream = Stream.CreateFilteredStream();
            _stream.AddTrack(RedLightTrack);
            _stream.AddTrack(OrangeLightTrack);
            _stream.AddTrack(GreenLightTrack);

            _stream.MatchingTweetReceived += (sender, args) =>
            {
                var matchingTrack = args.MatchingTracks?.FirstOrDefault();

                if (matchingTrack.Equals(this.RedLightTrack, StringComparison.OrdinalIgnoreCase))
                {
                    _trafficLightSvc.Value.Set(TrafficLightState.Red);
                }

                if (matchingTrack.Equals(this.OrangeLightTrack, StringComparison.OrdinalIgnoreCase))
                {
                    _trafficLightSvc.Value.Set(TrafficLightState.Orange);
                }

                if (matchingTrack.Equals(this.GreenLightTrack, StringComparison.OrdinalIgnoreCase))
                {
                    _trafficLightSvc.Value.Set(TrafficLightState.Green);
                }
            };

            _stream.StartStreamMatchingAnyCondition();
        }

        public void StopWatching()
        {
            _stream.StopStream();
        }
    }
}
