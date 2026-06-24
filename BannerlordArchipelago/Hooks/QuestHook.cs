using BannerlordArchipelago;
using BannerlordArchipelago.Data;
using HarmonyLib;
using StoryMode.StoryModePhases;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Library;

namespace BannerlordArchipelago.Hooks
{
    [HarmonyPatch(typeof(QuestBase), nameof(QuestBase.CompleteQuestWithSuccess))]
    public class QuestCompleteSuccessPatch
    {
        static void Postfix(QuestBase __instance)
        {
            QuestLocationMapper.OnQuestCompleted(__instance);
        }
    }
}
[HarmonyPatch(typeof(QuestBase), nameof(QuestBase.CompleteQuestWithSuccess))]
public class QuestCompleteSuccessPatch
{
    static void Postfix(QuestBase __instance)
    {
        QuestLocationMapper.OnQuestCompleted(__instance);
    }
}
public static class QuestLocationMapper
    {
        public static void OnQuestCompleted(QuestBase quest)
        {
            string typeName = quest.GetType().Name;
            Main.APClient.SendLocationCheck(typeName);

        }
}