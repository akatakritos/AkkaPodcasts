using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Xml.Linq;

using Akka.Actor;

using AkkaPodcasts.Core.Messages;

namespace AkkaPodcasts.Core.Actors
{
    public class FeedDownloadActor : ReceiveActor
    {
        private readonly IActorRef _router;
        private readonly string _path;
        private int _pendingDownloads = 0;

        public FeedDownloadActor(IActorRef router, string path)
        {
            _router = router;
            _path = path;

            Receive<DownloadFeedCommand>(cmd => DownloadFeed(cmd));
            Receive<string>(feed => ParseFeed(feed));
            Receive<PodcastFinished>(msg => PodcastFinished(msg));
        }

        private void DownloadFeed(DownloadFeedCommand cmd)
        {
            ConsoleHelper.Write(ConsoleColor.DarkYellow, "Downloading feed");
            var client = new HttpClient();

            // PipeTo is how to do async/await in Actors
            // you can't await because that will allow the actor to process multiple
            // messages concurrently, which would eliminate the state and thread safety
            // benefits you get from the system
            //
            // It sends the result of the async operation as a new message into the queue for the actor
            client.GetStringAsync(cmd.Url).PipeTo(Self);
        }

        private void ParseFeed(string feed)
        {
            ConsoleHelper.Write(ConsoleColor.DarkYellow, "Got Feed");
            _pendingDownloads = 0;

            var root = XElement.Load(new StringReader(feed));
            foreach (var item in root.Descendants("item").Take(10))
            {
                var title = item.Element("title").Value;
                var url = item.Element("enclosure").Attribute("url").Value;

                ConsoleHelper.Write(ConsoleColor.DarkYellow, $"{title} -> {url}");
                var filename = Path.GetFileName(url);

                _pendingDownloads++;

                // send download commands to the router, which will pass them on to
                // a child for processing. A child that replies to its Sender will have that
                // reply message routed back here
                _router.Tell(new DownloadPodcastCommand(new Uri(url), Path.Combine(_path, filename)));
            }

        }

        private void PodcastFinished(PodcastFinished cmd)
        {
            _pendingDownloads--;
            if (_pendingDownloads <= 0)
            {
                // When there are no more pending podcasts, we are finished
                ConsoleHelper.Write(ConsoleColor.DarkYellow, "Finished");
                Context.System.Terminate();
            }
        }
    }
}