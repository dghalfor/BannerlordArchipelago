using BannerlordArchipelago.Utils;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.CampaignSystem;
using TaleWorlds.ObjectSystem;

namespace BannerlordArchipelago.Archipelago
{
    public static class CultureTierInfo
    {
        private static readonly Dictionary<string, int> _maxTierCache = new Dictionary<string, int>();
        private static readonly Dictionary<string, List<CharacterObject>> _troopsCache
            = new Dictionary<string, List<CharacterObject>>();

        public static int GetMaxTier(CultureObject culture)
        {
            return GetTroopsForCulture(culture)
                .Select(c => c.Tier)
                .DefaultIfEmpty(0)
                .Max();
        }

        public static List<CharacterObject> GetTroopsForCulture(CultureObject culture)
        {
            if (culture == null) return new List<CharacterObject>();

            if (_troopsCache.TryGetValue(culture.StringId, out var cached))
                return cached;

            var troops = MBObjectManager.Instance.GetObjectTypeList<CharacterObject>()
                .Where(c => c.Culture == culture
                    && c.Occupation == Occupation.Soldier // TODO verify via decompile
                    && !c.IsHero)
                .OrderBy(c => c.Tier)
                .ThenBy(c => c.Name.ToString())
                .ToList();

            _troopsCache[culture.StringId] = troops;
            return troops;
        }
        public static void DiagnoseBanditOccupations()
        {
            var banditCultureIds = new[] { "looters", "sea_raiders", "mountain_bandits", "forest_bandits", "desert_bandits", "steppe_bandits", "southern_pirates" };

            var all = MBObjectManager.Instance.GetObjectTypeList<CharacterObject>()
                .Where(c => !c.IsHero && banditCultureIds.Contains(c.Culture?.StringId))
                .ToList();

            foreach (var c in all)
            {
                APLog.LogToFile($"Bandit troop: {c.StringId}, Culture: {c.Culture?.StringId}, Occupation: {c.Occupation}, Tier: {c.Tier}, IsBasicTroop: {c.IsBasicTroop}, IsSoldier: {c.IsSoldier}");
            }
        }
        public static void DiagnoseTroopBranches()
        {
            var allCultures = MBObjectManager.Instance.GetObjectTypeList<CultureObject>();

            foreach (var culture in allCultures)
            {
                var troops = GetTroopsForCulture(culture);
                if (troops.Count == 0) continue;

                var rootOf = TroopTreeUtil.BuildRootMap(troops);
                var mainRoots = new HashSet<CharacterObject> { culture.BasicTroop, culture.EliteBasicTroop };
                var branches = troops.GroupBy(t => rootOf[t]);

                APLog.LogToFile($"--- Culture: {culture.StringId} ---");
                foreach (var branch in branches)
                {
                    bool isMain = mainRoots.Contains(branch.Key);
                    APLog.LogToFile($"Branch root: {branch.Key?.StringId} ({branch.Key?.Name}), IsMainLine: {isMain}, TroopCount: {branch.Count()}, Tiers: {string.Join(",", branch.Select(t => t.Tier).Distinct().OrderBy(x => x))}");
                }
            }
        }

        public static void ClearCache()
        {
            _maxTierCache.Clear();
            _troopsCache.Clear();
        }
    

    public static void DiagnoseAllOccupations()
        {
            var all = MBObjectManager.Instance.GetObjectTypeList<CharacterObject>()
                .Where(c => !c.IsHero)
                .ToList();

            APLog.LogToFile("--- Distinct Occupation values in use ---");
            foreach (var grp in all.GroupBy(c => c.Occupation).OrderByDescending(g => g.Count()))
            {
                APLog.LogToFile($"Occupation: {grp.Key}, Count: {grp.Count()}");
            }
        }

        public static void DiagnoseBanditsAndMercenaries()
        {
            var banditCultureIds = new[]
            {
        "looters", "sea_raiders", "mountain_bandits", "forest_bandits",
        "desert_bandits", "steppe_bandits", "southern_pirates"
    };

            var all = MBObjectManager.Instance.GetObjectTypeList<CharacterObject>()
                .Where(c => !c.IsHero)
                .ToList();

            APLog.LogToFile("--- Bandit culture troops (occupation filter removed) ---");
            foreach (var c in all.Where(c => banditCultureIds.Contains(c.Culture?.StringId)))
            {
                var upgradeTargets = c.UpgradeTargets;
                int upgradeCount = upgradeTargets != null ? upgradeTargets.Count() : 0;
                APLog.LogToFile($"Bandit troop: {c.StringId} ({c.Name}), Culture: {c.Culture?.StringId}, " +
                           $"Occupation: {c.Occupation}, Tier: {c.Tier}, UpgradeTargets: {(c.UpgradeTargets != null ? c.UpgradeTargets.Count() : 0)}");
            }

            APLog.LogToFile("--- Troops with 'merc' in StringId or Name ---");
            foreach (var c in all.Where(c =>
                (c.StringId?.ToLowerInvariant().Contains("merc") ?? false) ||
                (c.Name?.ToString().ToLowerInvariant().Contains("merc") ?? false)))
            {
                APLog.LogToFile($"Mercenary-ish troop: {c.StringId} ({c.Name}), Culture: {c.Culture?.StringId}, " +
                           $"Occupation: {c.Occupation}, Tier: {c.Tier}, IsHero: {c.IsHero}");
            }

            APLog.LogToFile("--- All cultures referenced by any non-hero CharacterObject ---");
            foreach (var grp in all.GroupBy(c => c.Culture?.StringId ?? "(null)").OrderBy(g => g.Key))
            {
                APLog.LogToFile($"Culture: {grp.Key}, TroopCount: {grp.Count()}");
            }
        }

    }
    public static class BanditCultureInfo
    {
        public static readonly string[] IncludedBanditCultures = new[]
        {
        "mountain_bandits", "forest_bandits", "desert_bandits",
        "steppe_bandits", "southern_pirates", "sea_raiders"
        // looters excluded entirely - too irregular a shape
    };

        private static readonly HashSet<string> ExcludedTroopIds = new HashSet<string>
    {
        // party-leader heroes, tier 0, no upgrade path - not real troops
        "lord_looters_1", "lord_sea_raiders_1", "lord_mountain_bandits_1",
        "lord_forest_bandits_1", "lord_desert_bandits_1", "lord_steppe_bandits_1",
        // scripted one-off encounter enemies, not recruitable
        "storymode_quest_raider", "naval_storyline_alley_fight_enemy", "sea_hound_captivity"
    };

        public static List<CharacterObject> GetExportableBanditTroops()
        {
            return MBObjectManager.Instance.GetObjectTypeList<CharacterObject>()
                .Where(c => !c.IsHero
                    && c.Occupation == Occupation.Bandit
                    && IncludedBanditCultures.Contains(c.Culture?.StringId)
                    && !ExcludedTroopIds.Contains(c.StringId)
                    && c.Tier >= 3) // tier2 is each chain's base spawn troop, not an "upgrade"
                .OrderBy(c => c.Tier)
                .ThenBy(c => c.Name.ToString())
                .ToList();
        }

        public static int GetMaxBanditTier() =>
            GetExportableBanditTroops().Select(t => t.Tier).DefaultIfEmpty(0).Max();
    }

    public static class MercenaryInfo
    {
        // Empty for now - room to add company_of_trouble_character here later
        // if it turns out to be a quest-only singleton rather than a real recruit
        private static readonly HashSet<string> ExcludedTroopIds = new HashSet<string>();

        public static List<CharacterObject> GetExportableMercenaryTroops()
        {
            return MBObjectManager.Instance.GetObjectTypeList<CharacterObject>()
                .Where(c => !c.IsHero
                    && c.Occupation == Occupation.Mercenary
                    && !ExcludedTroopIds.Contains(c.StringId)
                    && c.Tier >= 3) // drops mercenary_1/Watchman, the numbered chain's base tier
                .OrderBy(c => c.Tier)
                .ThenBy(c => c.Name.ToString())
                .ToList();
        }

        public static int GetMaxMercenaryTier() =>
            GetExportableMercenaryTroops().Select(t => t.Tier).DefaultIfEmpty(0).Max();
    }
    public static class TroopTreeUtil
    {
        public static Dictionary<CharacterObject, CharacterObject> BuildRootMap(IEnumerable<CharacterObject> troops)
        {
            var troopList = troops.ToList();
            var childToParent = new Dictionary<CharacterObject, CharacterObject>();

            foreach (var troop in troopList)
            {
                if (troop.UpgradeTargets == null) continue;
                foreach (var child in troop.UpgradeTargets)
                    childToParent[child] = troop;
            }

            var rootOf = new Dictionary<CharacterObject, CharacterObject>();
            foreach (var troop in troopList)
            {
                var current = troop;
                var visited = new HashSet<CharacterObject>(); // guard against bad data loops
                while (childToParent.TryGetValue(current, out var parent) && visited.Add(current))
                    current = parent;
                rootOf[troop] = current;
            }
            return rootOf;
        }
    }
}