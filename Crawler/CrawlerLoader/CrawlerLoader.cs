using ff14bot.AClasses;
using ff14bot.Behavior;
using TreeSharp;

namespace CrawlerLoader
{
    public class CrawlerLoader : BotBase
    {
        public override string Name => "Crawler";
        public override PulseFlags PulseFlags => PulseFlags.All;
        public override Composite Root { get; } = new ActionAlwaysFail();
    }
}