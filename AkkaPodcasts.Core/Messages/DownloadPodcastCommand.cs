using System;
using System.Collections.Generic;
using System.Linq;

namespace AkkaPodcasts.Core.Messages
{
    public class DownloadPodcastCommand
    {
        public Uri Url { get; private set; }
        public string Path { get; private set; }

        public DownloadPodcastCommand(Uri url, string path)
        {
            Url = url;
            Path = path;
        }
    }
}