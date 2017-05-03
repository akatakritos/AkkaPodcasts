using System;
using System.Collections.Generic;
using System.Linq;

namespace AkkaPodcasts.Core.Messages
{
    public class DownloadFeedCommand
    {
        public Uri Url { get; set; }

        public DownloadFeedCommand(Uri url)
        {
            Url = url;
        }
    }
}