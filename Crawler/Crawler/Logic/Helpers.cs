using System.Threading.Tasks;
using Buddy.Coroutines;
using Crawler.Settings;
using ff14bot;
using ff14bot.Helpers;
using ff14bot.Managers;
using ff14bot.RemoteAgents;
using ff14bot.RemoteWindows;
using TreeSharp;

namespace Crawler.Logic
{
    public static class Helpers
    {
        static Helpers()
        {
            Composite = new ActionRunCoroutine(ctx => Run());
        }
        public static Composite Execute()
        {
            return Composite;
        }

        private static readonly Composite Composite;

        private static async Task<bool> Run()
        {
            GameSettingsManager.FaceTargetOnAction = CrawlerSettings.Instance.UseAutoFacing;

            if (Core.Me.InCombat) return true;

            if (CrawlerSettings.Instance.SkipDialog && Talk.DialogOpen)
            {
                Talk.Next();
                return true;
            }

            if (CrawlerSettings.Instance.AcceptQuests && JournalAccept.IsOpen)
            {
                JournalAccept.Accept();
                return true;
            }

            if (CrawlerSettings.Instance.SkipCutscenes && QuestLogManager.InCutscene)
            {
                if (AgentCutScene.Instance.CanSkip && !SelectString.IsOpen)
                {
                    AgentCutScene.Instance.PromptSkip();
                    if (await Coroutine.Wait(600, () => SelectString.IsOpen))
                    {
                        SelectString.ClickSlot(0);
                        await Coroutine.Sleep(1000);
                        return true;
                    }
                }
            }

            return false;
        }
    }
}