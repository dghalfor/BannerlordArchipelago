using BannerlordArchipelago.Archipelago;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Library;
using TaleWorlds.ObjectSystem;

using System.Collections.Generic;
using TaleWorlds.Library;
using BannerlordArchipelago.Utils;
using BannerlordArchipelago.Data;
using BannerlordArchipelago.UI;

public static class APDebugCommands
{
    [CommandLineFunctionality.CommandLineArgumentFunction("diagnosebandits", "apdebug")]
    public static string DiagnoseBanditOccupations(List<string> args)
    {
        try
        {
            CultureTierInfo.DiagnoseBanditOccupations();
            return "Bandit occupation diagnostic written to ap_debug.log";
        }
        catch (System.Exception ex)
        {
            return $"Diagnostic failed: {ex}";
        }
    }
    [CommandLineFunctionality.CommandLineArgumentFunction("diagnoseTroops", "apdebug")]
    public static string DiagnoseTroops(List<string> args)
    {
        try
        {
            CultureTierInfo.DiagnoseTroopBranches();
            return "troop occupation diagnostic written to ap_debug.log";
        }
        catch (System.Exception ex)
        {
            return $"Diagnostic failed: {ex}";
        }
    }
    [CommandLineFunctionality.CommandLineArgumentFunction("diagnosebanditsmercs", "apdebug")]
    public static string DiagnoseBanditsMercs(List<string> args)
    {
        try
        {
            CultureTierInfo.DiagnoseAllOccupations();
            CultureTierInfo.DiagnoseBanditsAndMercenaries();
            return "Bandit/mercenary diagnostic written to ap_debug.log";
        }
        catch (Exception ex)
        {
            return $"Diagnostic failed: {ex}";
        }
    }

    // Strips everything but letters/digits and lowercases, so "Gang Leader
    // Needs Special Weapons", "gangleaderneedsspecialweapons", and
    // "GangLeaderNeedsSpecialWeaponsIssue" all normalize to the same string
    // and can be matched against each other regardless of spacing/casing.
    private static string Normalize(string s) =>
        new string(s.Where(char.IsLetterOrDigit).ToArray()).ToLowerInvariant();

    [CommandLineFunctionality.CommandLineArgumentFunction("findquest", "ap")]
    public static string ShowQuestLocations(List<string> args)
    {
        if (args.Count < 1)
            return "Usage: ap.findquest <name or partial name>  (run ap.listquest to see current options)";

        // Console args are space-split, so rejoin them — the player is typing
        // a human phrase like "gang leader needs special weapons", not a token.
        string query = string.Join(" ", args);
        string normalizedQuery = Normalize(query);

        var active = QuestLocationLookup.GetActiveIssuesWithTitles();

        var matches = active
            .Where(a => Normalize(a.Title).Contains(normalizedQuery) || Normalize(a.TypeName).Contains(normalizedQuery))
            .ToList();

        if (matches.Count == 0)
            return $"No active issue matches '{query}'. Run ap.listquest to see current options.";

        if (matches.Count > 1)
        {
            string options = string.Join(", ", matches.Select(m => m.Title));
            return $"'{query}' matches more than one active issue — be more specific: {options}";
        }

        var match = matches[0];
        var entries = QuestLocationLookup.GetLocationsOfferingIssueType(match.TypeName);
        var displayTexts = entries.Select(e => e.DisplayText).ToList();

        QuestLocationTooltipManager.Show(match.Title, displayTexts);

        return displayTexts.Count > 0
            ? $"Showing {displayTexts.Count} location(s) offering '{match.Title}'."
            : $"No notables are currently offering '{match.Title}'.";
    }

    [CommandLineFunctionality.CommandLineArgumentFunction("hidequest", "ap")]
    public static string HideQuestLocations(List<string> args)
    {
        QuestLocationTooltipManager.Hide();
        return "Hidden.";
    }

    [CommandLineFunctionality.CommandLineArgumentFunction("listquest", "ap")]
    public static string ListActiveIssueTypes(List<string> args)
    {
        var active = QuestLocationLookup.GetActiveIssuesWithTitles();
        return active.Count > 0
            ? string.Join("\n", active.Select(a => $"{a.Title} ({a.Count} location{(a.Count == 1 ? "" : "s")})"))
            : "No issues are currently active anywhere in the world.";
    }
}



public static class APExportCommands
{
    [CommandLineFunctionality.CommandLineArgumentFunction("exporttroopdata", "apdebug")]
    public static string ExportTroopData(List<string> args)
    {
        try
        {
            var exporter = new ArchipelagoTroopExporter();
            var result = exporter.Export();
            return $"Exported {result.RegionCount} regions, {result.LocationCount} locations, {result.ItemCount} items to {result.OutputDir}";
        }
        catch (Exception ex)
        {
            return $"Export failed: {ex}";
        }
    }
}

public class ExportResult
{
    public int RegionCount;
    public int LocationCount;
    public int ItemCount;
    public string OutputDir;
}

public class ArchipelagoTroopExporter
{
    // Tiers start at 2 because Tier1 troops are the culture's starting/basic
    // troop and aren't gated behind a Progressive item - matches your sample JSON.
    private const int MinGatedTier = 2;
    private const int MaxTier = 6;
    public enum TroopCategory { MainLine, Militia, Excluded, Unclassified }

    public static class TroopClassifier
    {
        private static readonly HashSet<string> EliteMainLineRoots = new HashSet<string>
    {
        "vlandian_squire", "aserai_youth", "sturgian_warrior_son",
        "battanian_highborn_youth", "imperial_vigla_recruit",
        "khuzait_noble_son", "nord_ungmann"
    };

        public static bool IsMinorFactionShape(List<CharacterObject> branchTroops)
        {
            var tiers = branchTroops.Select(t => t.Tier).Distinct().OrderBy(t => t).ToList();
            return branchTroops.Count == 3 && tiers.Count == 3
                && tiers[0] == 2 && tiers[1] == 3 && tiers[2] == 4;
        }

        public static TroopCategory ClassifyBranch(
            CharacterObject root, List<CharacterObject> branchTroops, HashSet<CharacterObject> mainRoots)
        {
            string id = root?.StringId ?? "";

            if (mainRoots.Contains(root)) return TroopCategory.MainLine;
            if (EliteMainLineRoots.Contains(id)) return TroopCategory.MainLine;

            if (id.StartsWith("guard_") || id.StartsWith("tutorial_npc") ||
                id.Contains("contender") || id.StartsWith("conspiracy_") ||
                id == "borrowed_troop")
                return TroopCategory.Excluded;

            if (id.Contains("militia")) return TroopCategory.Militia;

            if (IsMinorFactionShape(branchTroops)) return TroopCategory.MainLine; // routed to minor-faction bucket downstream

            return TroopCategory.Unclassified;
        }
    }
    public ExportResult Export()
    {
        var regions = new Dictionary<string, object>();
        var locations = new List<object>();
        var items = new List<object>();

        var allCultures = Campaign.Current.ObjectManager.GetObjectTypeList<CultureObject>();

        foreach (var culture in allCultures)
        {
            var data = BuildExportData(culture);
            if (data.MaxTier <= 0) continue;

            items.Add(new
            {
                name = $"Progressive {data.CultureId} Troop Tier",
                count = data.MaxTier - 1,
                progression = true
            });

            // Main-line region chain
            for (int tier = MinGatedTier; tier <= data.MaxTier; tier++)
            {
                string regionKey = $"{data.CultureId}Tier{tier}";
                bool isLast = tier == data.MaxTier;

                regions[regionKey] = new
                {
                    connects_to = isLast ? Array.Empty<string>() : new[] { $"{data.CultureId}Tier{tier + 1}" },
                    requires = TierRequirement(data.CultureId, tier)
                };

                if (data.MainLineTroopsByTier.TryGetValue(tier, out var troops))
                {
                    foreach (var troop in troops)
                        locations.Add(new { name = $"Upgraded to {troop.Name}", category = new[] { "TroopUpgrade" }, region = regionKey });
                }
            }

            // Minor-faction parallel chain, gated by the same item
            if (data.HasMinorFaction)
            {
                int minorMaxTier = data.MinorFactionTroopsByTier.Keys.Max();

                for (int tier = MinGatedTier; tier <= minorMaxTier; tier++)
                {
                    string regionKey = $"{data.CultureId}MinorFactionTier{tier}";
                    bool isLast = tier == minorMaxTier;

                    regions[regionKey] = new
                    {
                        connects_to = isLast ? Array.Empty<string>() : new[] { $"{data.CultureId}MinorFactionTier{tier + 1}" },
                        requires = TierRequirement(data.CultureId, tier)
                    };
                    // Tier 2 intentionally has no locations here (Sprout/Boar Novice/etc.
                    // are the branch's recruit-level entry, not an "upgrade").

                    if (data.MinorFactionTroopsByTier.TryGetValue(tier, out var troops))
                    {
                        foreach (var troop in troops)
                            locations.Add(new { name = $"Upgraded to {troop.Name}", category = new[] { "TroopUpgrade" }, region = regionKey });
                    }
                }
            }

        }
        // --- Bandits: single combined progression across 6 cultures ---
        var banditTroops = BanditCultureInfo.GetExportableBanditTroops();
        int banditMaxTier = BanditCultureInfo.GetMaxBanditTier();
        if (banditMaxTier >= MinGatedTier)
        {
            items.Add(new { name = "Progressive Bandit Troop Tier", count = banditMaxTier - 1, progression = true });

            for (int tier = MinGatedTier; tier <= banditMaxTier; tier++)
            {
                string regionKey = $"banditTier{tier}";
                bool isLast = tier == banditMaxTier;
                regions[regionKey] = new
                {
                    connects_to = isLast ? Array.Empty<string>() : new[] { $"banditTier{tier + 1}" },
                    requires = TierRequirement("Bandit", tier)
                };

                foreach (var troop in banditTroops.Where(t => t.Tier == tier))
                    locations.Add(new { name = $"Upgraded to {troop.Name}", category = new[] { "TroopUpgrade" }, region = regionKey });
            }
        }

        // --- Mercenaries: single combined progression across eastern/western/numbered chains ---
        var mercTroops = MercenaryInfo.GetExportableMercenaryTroops();
        int mercMaxTier = MercenaryInfo.GetMaxMercenaryTier();
        if (mercMaxTier >= MinGatedTier)
        {
            items.Add(new { name = "Progressive Mercenary Troop Tier", count = mercMaxTier - 1, progression = true });

            for (int tier = MinGatedTier; tier <= mercMaxTier; tier++)
            {
                string regionKey = $"mercenaryTier{tier}";
                bool isLast = tier == mercMaxTier;
                regions[regionKey] = new
                {
                    connects_to = isLast ? Array.Empty<string>() : new[] { $"mercenaryTier{tier + 1}" },
                    requires = TierRequirement("Mercenary", tier)
                };

                foreach (var troop in mercTroops.Where(t => t.Tier == tier))
                    locations.Add(new { name = $"Upgraded to {troop.Name}", category = new[] { "TroopUpgrade" }, region = regionKey });
            }
        }
        string outDir = Path.Combine(BasePath.Name, "Modules", "BannerlordArchipelago", "ap_export");
        Directory.CreateDirectory(outDir);

        var settings = new JsonSerializerSettings { Formatting = Formatting.Indented };
        File.WriteAllText(Path.Combine(outDir, "regions.json"), JsonConvert.SerializeObject(regions, settings));
        File.WriteAllText(Path.Combine(outDir, "locations.json"), JsonConvert.SerializeObject(locations, settings));
        File.WriteAllText(Path.Combine(outDir, "items.json"), JsonConvert.SerializeObject(items, settings));

        return new ExportResult { RegionCount = regions.Count, LocationCount = locations.Count, ItemCount = items.Count };
    }

    private static string TierRequirement(string cultureId, int tier)
    {
        int count = tier - MinGatedTier + 1;
        return count <= 1
            ? $"|Progressive {cultureId} Troop Tier|"
            : $"|Progressive {cultureId} Troop Tier:{count}|";
    }
    public class CultureExportData
    {
        public string CultureId;
        public int MaxTier;
        public Dictionary<int, List<CharacterObject>> MainLineTroopsByTier = new Dictionary<int, List<CharacterObject>>();
        public Dictionary<int, List<CharacterObject>> MinorFactionTroopsByTier = new Dictionary<int, List<CharacterObject>>();
        public bool HasMinorFaction => MinorFactionTroopsByTier.Count > 0;
    }

    public static CultureExportData BuildExportData(CultureObject culture)
    {
        var allTroops = CultureTierInfo.GetTroopsForCulture(culture);
        var rootOf = TroopTreeUtil.BuildRootMap(allTroops);
        var mainRoots = new HashSet<CharacterObject> { culture.BasicTroop, culture.EliteBasicTroop };

        var data = new CultureExportData { CultureId = culture.StringId, MaxTier = CultureTierInfo.GetMaxTier(culture) };

        foreach (var branch in allTroops.GroupBy(t => rootOf[t]))
        {
            var branchList = branch.ToList();
            var category = TroopClassifier.ClassifyBranch(branch.Key, branchList, mainRoots);

            if (category != TroopCategory.MainLine)
            {
                if (category == TroopCategory.Unclassified)
                    APLog.LogToFile($"[Unclassified - excluded] {branch.Key?.StringId} ({branch.Key?.Name}), Culture: {culture.StringId}");
                continue;
            }

            bool isMinorFaction = TroopClassifier.IsMinorFactionShape(branchList);
            var targetDict = isMinorFaction ? data.MinorFactionTroopsByTier : data.MainLineTroopsByTier;

            // Skip the lowest tier of whichever tree this is: tier 1 for the
            // main recruit line, tier 2 for a minor-faction branch (its root
            // troop is the "recruit" for that tree, not an upgrade into it).
            int skipAtOrBelow = isMinorFaction ? 2 : 1;

            foreach (var troop in branchList)
            {
                if (troop.Tier <= skipAtOrBelow) continue;
                if (!targetDict.TryGetValue(troop.Tier, out var list))
                    targetDict[troop.Tier] = list = new List<CharacterObject>();
                list.Add(troop);
            }
        }

        return data;
    }
    public static List<CharacterObject> GetExportableTroopsForCulture(CultureObject culture)
    {
        var allTroops = CultureTierInfo.GetTroopsForCulture(culture); // existing Occupation-filtered list
        var rootOf = TroopTreeUtil.BuildRootMap(allTroops);
        var mainRoots = new HashSet<CharacterObject> { culture.BasicTroop, culture.EliteBasicTroop };

        var result = new List<CharacterObject>();

        foreach (var branch in allTroops.GroupBy(t => rootOf[t]))
        {
            var branchList = branch.ToList();
            var category = TroopClassifier.ClassifyBranch(branch.Key, branchList, mainRoots);

            if (category == TroopCategory.MainLine)
            {
                result.AddRange(branchList);
            }
            else if (category == TroopCategory.Unclassified)
            {
                APLog.LogToFile($"[Unclassified - excluded by default] {branch.Key?.StringId} ({branch.Key?.Name}), " +
                          $"Culture: {culture.StringId}, TroopCount: {branchList.Count}, " +
                          $"Tiers: {string.Join(",", branchList.Select(t => t.Tier).Distinct().OrderBy(t => t))}");
            }
            // Militia / Excluded: silently dropped, no log needed - pattern is well understood
        }

        return result.OrderBy(t => t.Tier).ThenBy(t => t.Name.ToString()).ToList();
    }
    
}
