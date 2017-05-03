using System;
using System.Collections.Generic;
using System.Linq;

using Akka.Actor;
using Akka.Event;
using Akka.TestKit;
using Akka.TestKit.Xunit2;

using AkkaPodcasts.Core.Actors;
using AkkaPodcasts.Core.Messages;

using NFluent;

using Xunit;

namespace AkkaPodcasts.Tests.Actors
{
    public class FeedDownloadActorTests : TestKit
    {
        [Fact]
        public void ParsePodcast()
        {
            const string xml = "<?xml version=\"1.0\" encoding=\"UTF-8\"?><rss><item><title>Foo</title><enclosure url=\"http://www.example.com/podcast.mp3\"/></item></rss>";

            var subject = Sys.ActorOf(Props.Create(() => new FeedDownloadActor(TestActor, @"C:\")));
            subject.Tell(xml);

            var msg = ExpectMsg<DownloadPodcastCommand>();
            Check.That(msg.Url).IsEqualTo(new Uri("http://www.example.com/podcast.mp3"));
        }
    }
}
