using System.Linq;
using Crawler.Settings;
using ff14bot;
using ff14bot.Managers;
using ff14bot.Objects;

namespace Crawler.Utilities
{
    public static class TargetingProvider
    {
        public static BattleCharacter GetTarget()
        {
            if (!CrawlerSettings.Instance.UseAutoTargeting)
            {
                return null;
            }
            var aggroedEnemies = GameObjectManager.GetObjectsOfType<BattleCharacter>()
                        .Where(x => x.TaggerType == 2)
                        .ToList().OrderByDescending(x => x.Distance2D(Core.Me));
            if (aggroedEnemies.Any())
            {
                return aggroedEnemies.First();
            }

            var enemies = GameObjectManager.GetObjectsOfType<BattleCharacter>()
                .Where(x => x.IsValid && x.InLineOfSight() && x.IsAlive && x.IsTargetable && x.CanAttack && x.Distance2D(Core.Me) <= 40f)
                .ToList().OrderBy(x => x.Distance2D(Core.Me));
            if (enemies.Any())
            {
                return enemies.First();
            }

            return null;
        }
    }
}