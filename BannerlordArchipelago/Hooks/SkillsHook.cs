using BannerlordArchipelago.Archipelago;
using HarmonyLib;
using System;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace BannerlordArchipelago.Hooks
{
    [HarmonyPatch(typeof(Hero), "SetSkillValue")]
    public static class SkillLevelPatch
    {
        public static void Postfix(Hero __instance, SkillObject skill, int value)
        {
            try
            {
                if (__instance != Hero.MainHero) return;

                for (int threshold = 25; threshold <= value; threshold += 25)
                {
                    string locationName = $"{skill.StringId} Skill Level {threshold}";
                    Main.APClient.SendLocationCheck(locationName);
                }
            }
            catch (Exception e)
            {
                InformationManager.DisplayMessage(new InformationMessage(
                    $"[AP] SkillLevelPatch exception: {e.Message}", Colors.Red));
            }
        }
    }
}