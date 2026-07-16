using System.Collections.Generic;
using System.Linq;
using TaleWorlds.CampaignSystem;

namespace BannerlordArchipelago.Data
{
    public static class QuestLocationLookup
    {
        /// <summary>
        /// Finds all notables currently offering an issue of the given type
        /// (e.g. "GangLeaderNeedsSpecialWeaponsIssue"). Only considers
        /// pre-acceptance issues (Hero.Issue), not started QuestBase instances —
        /// this reflects "where is this location currently obtainable". Matching
        /// is purely on the type — two instances of the same issue type with
        /// different live Title text (due to per-notable token substitution)
        /// both show up here, since only the type identifies the location.
        /// </summary>
        public static List<QuestLocationEntry> GetLocationsOfferingIssueType(string issueTypeName)
        {
            var results = new List<QuestLocationEntry>();

            foreach (var hero in Hero.AllAliveHeroes)
            {
                var issue = hero.Issue;
                if (issue == null) continue;
                if (issue.GetType().Name != issueTypeName) continue;

                results.Add(new QuestLocationEntry
                {
                    HeroName = hero.Name?.ToString() ?? issueTypeName,
                    SettlementName = hero.CurrentSettlement?.Name?.ToString(),
                });
            }

            return results;
        }

        /// <summary>
        /// Every issue type currently active anywhere in the world, deduped by
        /// type (not by title — two instances with differing live title text
        /// still count as one entry here since they're the same underlying
        /// location type), paired with a representative title for matching/display
        /// and a count of how many notables currently offer that type.
        /// </summary>
        public static List<ActiveIssueInfo> GetActiveIssuesWithTitles()
        {
            var byType = new Dictionary<string, ActiveIssueInfo>();

            foreach (var hero in Hero.AllAliveHeroes)
            {
                var issue = hero.Issue;
                if (issue == null) continue;

                string typeName = issue.GetType().Name;

                if (!byType.TryGetValue(typeName, out var info))
                {
                    info = new ActiveIssueInfo
                    {
                        TypeName = typeName,
                        Title = issue.Title?.ToString() ?? typeName,
                        Count = 0,
                    };
                    byType[typeName] = info;
                }

                info.Count++;
            }

            return byType.Values.OrderBy(x => x.Title).ToList();
        }
    }

    public class ActiveIssueInfo
    {
        public string TypeName;
        public string Title;
        public int Count;
    }

    public class QuestLocationEntry
    {
        public string HeroName;
        public string SettlementName;

        public string DisplayText =>
            string.IsNullOrEmpty(SettlementName) ? HeroName : $"{HeroName} ({SettlementName})";
    }
}