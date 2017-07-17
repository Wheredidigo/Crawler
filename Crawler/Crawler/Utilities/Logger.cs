using System;
using System.Windows.Media;
using ff14bot.Helpers;

namespace Crawler.Utilities
{
    internal static class Logger
    {
        internal static void Log(string text)
        {
            Logging.Write(Colors.Red, $"[Crawler: {DateTime.UtcNow}] {text}");
        }
    }
}