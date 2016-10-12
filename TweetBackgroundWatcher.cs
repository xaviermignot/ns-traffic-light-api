using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tweetinvi;
using Tweetinvi.Models;
using Tweetinvi.Streaming;

namespace TrafficLight.Api
{
    internal class TweetBackgroundWatcher
    {
        internal static Lazy<TweetBackgroundWatcher> LazyInstance = new Lazy<TweetBackgroundWatcher>(
            () => new TweetBackgroundWatcher());

        public ICollection<ITweet> Tweets = new List<ITweet>();

        private IFilteredStream _stream;

        public void StartWatching()
        {
            _stream = Stream.CreateFilteredStream();
            _stream.AddTrack("#experiences");

            _stream.MatchingTweetReceived += (sender, args) => 
            {
                Tweets.Add(args.Tweet);
            };

            _stream.StartStreamMatchingAllConditions();
        }

        public void StopWatching()
        {
            _stream.StopStream();
        }
    }
}
