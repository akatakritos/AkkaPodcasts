using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using Akka.Actor;
using Akka.Routing;

using AkkaPodcasts.Core;
using AkkaPodcasts.Core.Actors;
using AkkaPodcasts.Core.Messages;

namespace AkkaPodcasts
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            var path = Directory.GetCurrentDirectory();
            var podcastFeed = new Uri("https://blog.nerdery.com/category/nerdcast/feed/");


            var system = ActorSystem.Create("PodcastSystem");

            // A roundrobin router forwards messages to its children in a round robin
            // format so that messages are farily evenly spread out
            //
            // There are 3 PodcastDownloadActor workers in the pool
            var router = system.ActorOf(new RoundRobinPool(3).Props(Props.Create<PodcastDownloadActor>()));

            var downloader = system.ActorOf(Props.Create(() => new FeedDownloadActor(router)));

            // kick off the process by telling the feed downloader to start
            downloader.Tell(new DownloadFeedCommand(podcastFeed, path));

            system.WhenTerminated.Wait();

            Console.WriteLine("Done. Press any key");
            Console.ReadKey();
        }
    }

}
