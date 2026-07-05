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
                if (ReceivedItemsTracker.IsReady) {
                    InformationManager.DisplayMessage(new InformationMessage(
                    $"[AP] SkillLevelPatch exception: {e.Message}", Colors.Red));
                }
            }
        }
    }

    public static class SkillHelper
    {
        private const int StartingAttributeValue = 3;
        private const int StartingFocusValue = 0;
        private const int StartingSkillLevel = 10;
        private static void LogToFile(string msg)
        {
            try
            {
                System.IO.File.AppendAllText(
                    "E:/ap_debug.log",  // drive root - no subfolder to get wrong
                    $"{DateTime.Now:HH:mm:ss.fff} {msg}\n");
            }
            catch (Exception ex)
            {
                // temporarily don't swallow - if this itself throws, we need to see it
                try { System.IO.File.AppendAllText("E:/ap_debug_error.log", ex.ToString()); } catch { }
            }
        }

        public static void ResetHeroProgressionForRando(Hero hero)
        {
            InformationManager.DisplayMessage(new InformationMessage(
                    $"[AP] ResetHeroProgression", Colors.Red));
            LogToFile("ResetHeroProgressionForRando: start");
            var developer = hero.HeroDeveloper;

            foreach (CharacterAttribute attribute in Game.Current.ObjectManager
                .GetObjectTypeList<CharacterAttribute>())
            {
                int current = hero.GetAttributeValue(attribute);
                int delta = StartingAttributeValue - current;
                LogToFile($"About to AddAttribute: {attribute.StringId}, current={current}, delta={delta}");
                if (delta != 0)
                {
                    developer.AddAttribute(attribute, delta, false);
                }
                LogToFile($"AddAttribute succeeded: {attribute.StringId}");
            }
            LogToFile("Attribute loop complete");

            foreach (SkillObject skill in Game.Current.ObjectManager
                .GetObjectTypeList<SkillObject>())
            {
                int currentFocus = developer.GetFocus(skill);
                int focusDelta = StartingFocusValue - currentFocus;
                LogToFile($"About to AddFocus: {skill.StringId}, current={currentFocus}, delta={focusDelta}");
                if (focusDelta != 0)
                {
                    developer.AddFocus(skill, focusDelta, false);
                }
                LogToFile($"AddFocus succeeded: {skill.StringId}");

                LogToFile($"About to SetInitialSkillLevel: {skill.StringId}, target={StartingSkillLevel}");
                developer.SetInitialSkillLevel(skill, StartingSkillLevel);
                LogToFile($"SetInitialSkillLevel succeeded: {skill.StringId}");
            }
            LogToFile("ResetHeroProgressionForRando: complete");
        }
    }
}