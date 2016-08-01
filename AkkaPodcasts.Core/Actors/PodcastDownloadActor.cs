using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;

using Akka.Actor;

using AkkaPodcasts.Core.Messages;

namespace AkkaPodcasts.Core.Actors
{
    public class PodcastDownloadActor : ReceiveActor
    {
        public PodcastDownloadActor()
        {
            Receive<DownloadPodcastCommand>(cmd => Download(cmd));
        }

        private void Download(DownloadPodcastCommand cmd)
        {
            using (var client = new HttpClient())
            {
                ConsoleHelper.Write(ConsoleColor.Blue, $"Downloading podcast {cmd.Url}");

                var response = client.GetAsync(cmd.Url).Result;
                var stream = response.Content.ReadAsStreamAsync().Result;
                var outputStream = File.OpenWrite(cmd.Path);
                stream.CopyTo(outputStream);

                ConsoleHelper.Write(ConsoleColor.Blue, "Finished.");
                Sender.Tell(new PodcastFinished());
            }
        }
    }
}
