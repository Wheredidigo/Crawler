using System;
using System.Threading.Tasks;
using ff14bot.Behavior;
using TreeSharp;

namespace Crawler.Logic
{
    public static class Hooks
    {
        private static readonly Composite Composite;
        private static readonly Composite TreeStart = new HookExecutor("TreeStart");

        static Hooks()
        {
            Composite = new ActionRunCoroutine(ctx => Run());
        }

        public static Composite Execute()
        {
            return Composite;
        }

        private static async Task<bool> Run()
        {
            try
            {
                if (await TreeStart.ExecuteCoroutine()) return true;
            }
            catch (Exception)
            {
                //Ignore Exception on purpose   
            }
            return false;
        }
    }
}