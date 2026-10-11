using BannerlordArchipelago;
using HarmonyLib;
using System.Collections.Generic;
using TaleWorlds.CampaignSystem;

namespace BannerlordArchipelago.Hooks
{
    // NOTE: only ONE copy of this patch class should exist. Your old file had it twice
    // (once inside the namespace, once outside); if both get patched the postfix runs
    // twice per quest, which would double-count in total mode.
    [HarmonyPatch(typeof(QuestBase), nameof(QuestBase.CompleteQuestWithSuccess))]
    public class QuestCompleteSuccessPatch
    {
        static void Postfix(QuestBase __instance)
        {
            QuestLocationMapper.OnQuestCompleted(__instance);
        }
    }

    public enum QuestCategory
    {
        None,
        Merchant,
        Village,
        Lord,
        Gang
    }

    public static class QuestLocationMapper
    {
        // Optional explicit overrides by quest class name, for quests the giver-based
        // classification gets wrong. Key = quest.GetType().Name.
        private static readonly Dictionary<string, QuestCategory> CategoryOverrides =
            new Dictionary<string, QuestCategory>
            {
                // { "SomeQuestClassName", QuestCategory.Gang },
            };

        public static void OnQuestCompleted(QuestBase quest)
        {
            if (quest == null || Main.APClient == null)
                return;

            QuestCategory category = Classify(quest);

            if (category != QuestCategory.None)
            {
                string key = category.ToString().ToLowerInvariant();

                // Total mode: specific quest locations don't exist, send the next counter.
                if (Main.APClient.GetSlotLong($"{key}_quest_use_total") != 0)
                {
                    int total = (int)Main.APClient.GetSlotLong($"{key}_quest_total");
                    Main.APClient.SendNextQuestTotal(category.ToString(), total);
                    return;
                }
            }

            // Specific mode (or unclassified quest): existing behavior.
            string typeName = quest.GetType().Name;
            Main.APClient.SendLocationCheck(typeName);
        }

        private static QuestCategory Classify(QuestBase quest)
        {
            if (CategoryOverrides.TryGetValue(quest.GetType().Name, out var overridden))
                return overridden;

            Hero giver = quest.QuestGiver;
            if (giver == null)
                return QuestCategory.None;

            // Order matters: gang leaders and merchants are also notables.
            if (giver.IsGangLeader) return QuestCategory.Gang;
            if (giver.IsMerchant || giver.IsArtisan) return QuestCategory.Merchant;
            if (giver.IsRuralNotable) return QuestCategory.Village; // headmen + landlords
            if (giver.IsLord) return QuestCategory.Lord;

            return QuestCategory.None;
        }
    }
}