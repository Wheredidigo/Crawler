using System.Threading.Tasks;
using Crawler.Utilities;
using ff14bot;
using ff14bot.Behavior;
using ff14bot.Managers;
using ff14bot.Objects;
using TreeSharp;

namespace Crawler.Logic
{
    public static class Tank
    {
        static Tank()
        {
            Composite = new Decorator(r => PartyDescriptors.IsTank[Core.Me.CurrentJob], new ActionRunCoroutine(ctx => Run()));
        }
        public static Composite Execute()
        {
            return Composite;
        }

        private static readonly Composite Composite;

        private static async Task<bool> Run()
        {
            if (Core.Me.IsDead || Core.Me.IsMounted) return true;

            if (!Core.Me.InCombat)
            {
                if (RoutineManager.Current.PreCombatBuffBehavior != null)
                {
                    if (await RoutineManager.Current.PreCombatBuffBehavior.ExecuteCoroutine())
                    {
                        Logger.Log("We just pre-combat buffed!");
                        return true;
                    }
                }
                BattleCharacter target;
                if (Core.Me.CurrentTarget == null)
                {
                    target = TargetingProvider.GetTarget();
                    target?.Target();
                }
                else
                {
                    target = Core.Me.CurrentTarget as BattleCharacter;
                }

                if (RoutineManager.Current.PullBuffBehavior != null && target != null)
                {
                    if (await RoutineManager.Current.PullBuffBehavior.ExecuteCoroutine())
                    {
                        Logger.Log("We just buffed our pulled!");
                        return true;
                    }
                }

                if (RoutineManager.Current.PullBehavior != null && target != null)
                {
                    if (await RoutineManager.Current.PullBehavior.ExecuteCoroutine())
                    {
                        Logger.Log("We just pulled!");
                        return true;
                    }
                }

            }
            else
            {
                var target = Core.Me.CurrentTarget as BattleCharacter;
                if (target == null)
                {
                    target = TargetingProvider.GetTarget();
                    target?.Target();
                }

                if (target?.CurrentHealth == 0)
                {
                    Core.Me.ClearTarget();
                    return false;
                }

                if (RoutineManager.Current.HealBehavior != null)
                {
                    if (await RoutineManager.Current.HealBehavior.ExecuteCoroutine()) return true;
                }

                if (RoutineManager.Current.CombatBuffBehavior != null)
                {
                    if (await RoutineManager.Current.CombatBuffBehavior.ExecuteCoroutine()) return true;
                }

                if (RoutineManager.Current.CombatBehavior != null)
                {
                    if (await RoutineManager.Current.CombatBehavior.ExecuteCoroutine()) return true;
                }
            }

            return false;
        }
    }
}