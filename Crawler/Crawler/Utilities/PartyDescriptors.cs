﻿using System.Collections.Generic;
using ff14bot.Enums;

namespace Crawler.Utilities
{
    public static class PartyDescriptors
    {
        private static Dictionary<ClassJobType, bool> _isTank;
        public static Dictionary<ClassJobType, bool> IsTank => _isTank ?? (_isTank = new Dictionary<ClassJobType, bool>
        {
            {ClassJobType.Adventurer, false},
            {ClassJobType.Gladiator, true},
            {ClassJobType.Pugilist, false},
            {ClassJobType.Marauder, true},
            {ClassJobType.Lancer, false},
            {ClassJobType.Archer, false},
            {ClassJobType.Conjurer, false},
            {ClassJobType.Thaumaturge, false},
            {ClassJobType.Carpenter, false},
            {ClassJobType.Blacksmith, false},
            {ClassJobType.Armorer, false},
            {ClassJobType.Goldsmith, false},
            {ClassJobType.Leatherworker, false},
            {ClassJobType.Weaver, false},
            {ClassJobType.Alchemist, false},
            {ClassJobType.Culinarian, false},
            {ClassJobType.Miner, false},
            {ClassJobType.Botanist, false},
            {ClassJobType.Fisher, false},
            {ClassJobType.Paladin, true},
            {ClassJobType.Monk, false},
            {ClassJobType.Warrior, true},
            {ClassJobType.Dragoon, false},
            {ClassJobType.Bard, false},
            {ClassJobType.WhiteMage, false},
            {ClassJobType.BlackMage, false},
            {ClassJobType.Arcanist, false},
            {ClassJobType.Summoner, false},
            {ClassJobType.Scholar, false},
            {ClassJobType.Rogue, false},
            {ClassJobType.Ninja, false},
            {ClassJobType.Machinist, false},
            {ClassJobType.DarkKnight, true},
            {ClassJobType.Astrologian, false},
            {ClassJobType.Samurai, false},
            {ClassJobType.RedMage, false},
            {ClassJobType.Gunbreaker, true},
            {ClassJobType.Dancer, false}
        });

        private static Dictionary<ClassJobType, bool> _isHealer;
        public static Dictionary<ClassJobType, bool> IsHealer => _isHealer ?? (_isHealer = new Dictionary<ClassJobType, bool>
        {
            {ClassJobType.Adventurer, false},
            {ClassJobType.Gladiator, false},
            {ClassJobType.Pugilist, false},
            {ClassJobType.Marauder, false},
            {ClassJobType.Lancer, false},
            {ClassJobType.Archer, false},
            {ClassJobType.Conjurer, true},
            {ClassJobType.Thaumaturge, false},
            {ClassJobType.Carpenter, false},
            {ClassJobType.Blacksmith, false},
            {ClassJobType.Armorer, false},
            {ClassJobType.Goldsmith, false},
            {ClassJobType.Leatherworker, false},
            {ClassJobType.Weaver, false},
            {ClassJobType.Alchemist, false},
            {ClassJobType.Culinarian, false},
            {ClassJobType.Miner, false},
            {ClassJobType.Botanist, false},
            {ClassJobType.Fisher, false},
            {ClassJobType.Paladin, false},
            {ClassJobType.Monk, false},
            {ClassJobType.Warrior, false},
            {ClassJobType.Dragoon, false},
            {ClassJobType.Bard, false},
            {ClassJobType.WhiteMage, true},
            {ClassJobType.BlackMage, false},
            {ClassJobType.Arcanist, true},
            {ClassJobType.Summoner, false},
            {ClassJobType.Scholar, true},
            {ClassJobType.Rogue, false},
            {ClassJobType.Ninja, false},
            {ClassJobType.Machinist, false},
            {ClassJobType.DarkKnight, false},
            {ClassJobType.Astrologian, true},
            {ClassJobType.Samurai, false},
            {ClassJobType.RedMage, false},
            {ClassJobType.Gunbreaker, false},
            {ClassJobType.Dancer, false}
        });

        private static Dictionary<ClassJobType, bool> _isDamage;
        public static Dictionary<ClassJobType, bool> IsDamage => _isDamage ?? (_isDamage = new Dictionary<ClassJobType, bool>
        {
            {ClassJobType.Adventurer, false},
            {ClassJobType.Gladiator, false},
            {ClassJobType.Pugilist, true},
            {ClassJobType.Marauder, false},
            {ClassJobType.Lancer, true},
            {ClassJobType.Archer, true},
            {ClassJobType.Conjurer, false},
            {ClassJobType.Thaumaturge, true},
            {ClassJobType.Carpenter, false},
            {ClassJobType.Blacksmith, false},
            {ClassJobType.Armorer, false},
            {ClassJobType.Goldsmith, false},
            {ClassJobType.Leatherworker, false},
            {ClassJobType.Weaver, false},
            {ClassJobType.Alchemist, false},
            {ClassJobType.Culinarian, false},
            {ClassJobType.Miner, false},
            {ClassJobType.Botanist, false},
            {ClassJobType.Fisher, false},
            {ClassJobType.Paladin, false},
            {ClassJobType.Monk, true},
            {ClassJobType.Warrior, false},
            {ClassJobType.Dragoon, true},
            {ClassJobType.Bard, true},
            {ClassJobType.WhiteMage, false},
            {ClassJobType.BlackMage, true},
            {ClassJobType.Arcanist, false},
            {ClassJobType.Summoner, true},
            {ClassJobType.Scholar, false},
            {ClassJobType.Rogue, true},
            {ClassJobType.Ninja, true},
            {ClassJobType.Machinist, true},
            {ClassJobType.DarkKnight, false},
            {ClassJobType.Astrologian, false},
            {ClassJobType.Samurai, true},
            {ClassJobType.RedMage, true},
            {ClassJobType.Gunbreaker, false},
            {ClassJobType.Dancer, true}
        });

        private static Dictionary<ClassJobType, bool> _isChocobo;
        public static Dictionary<ClassJobType, bool> IsChocobo => _isChocobo ?? (_isChocobo = new Dictionary<ClassJobType, bool>
        {
            {ClassJobType.Adventurer, true},
            {ClassJobType.Gladiator, false},
            {ClassJobType.Pugilist, false},
            {ClassJobType.Marauder, false},
            {ClassJobType.Lancer, false},
            {ClassJobType.Archer, false},
            {ClassJobType.Conjurer, false},
            {ClassJobType.Thaumaturge, false},
            {ClassJobType.Carpenter, false},
            {ClassJobType.Blacksmith, false},
            {ClassJobType.Armorer, false},
            {ClassJobType.Goldsmith, false},
            {ClassJobType.Leatherworker, false},
            {ClassJobType.Weaver, false},
            {ClassJobType.Alchemist, false},
            {ClassJobType.Culinarian, false},
            {ClassJobType.Miner, false},
            {ClassJobType.Botanist, false},
            {ClassJobType.Fisher, false},
            {ClassJobType.Paladin, false},
            {ClassJobType.Monk, false},
            {ClassJobType.Warrior, false},
            {ClassJobType.Dragoon, false},
            {ClassJobType.Bard, false},
            {ClassJobType.WhiteMage, false},
            {ClassJobType.BlackMage, false},
            {ClassJobType.Arcanist, false},
            {ClassJobType.Summoner, false},
            {ClassJobType.Scholar, false},
            {ClassJobType.Rogue, false},
            {ClassJobType.Ninja, false},
            {ClassJobType.Machinist, false},
            {ClassJobType.DarkKnight, false},
            {ClassJobType.Astrologian, false},
            {ClassJobType.Samurai, false},
            {ClassJobType.RedMage, false},
            {ClassJobType.Gunbreaker, false},
            {ClassJobType.Dancer, false}
        });
    }
}