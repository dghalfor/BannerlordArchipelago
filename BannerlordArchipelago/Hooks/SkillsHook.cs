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
                    if (ReceivedItemsTracker.IsReady)
                    {
                        string locationName = $"{skill.StringId} Skill Level {threshold}";
                        Main.APClient.SendLocationCheck(locationName);
                    }
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

        public static void ResetHeroProgressionForRando(Hero hero)
        {
            InformationManager.DisplayMessage(new InformationMessage(
                    $"[AP] ResetHeroProgression", Colors.Red));
            var developer = hero.HeroDeveloper;

            foreach (CharacterAttribute attribute in Game.Current.ObjectManager
                .GetObjectTypeList<CharacterAttribute>())
            {
                int current = hero.GetAttributeValue(attribute);
                int delta = StartingAttributeValue - current;
                if (delta != 0)
                {
                    developer.AddAttribute(attribute, delta, false);
                }
            }

            foreach (SkillObject skill in Game.Current.ObjectManager
                .GetObjectTypeList<SkillObject>())
            {
                int currentFocus = developer.GetFocus(skill);
                int focusDelta = StartingFocusValue - currentFocus;
                if (focusDelta != 0)
                {
                    developer.AddFocus(skill, focusDelta, false);
                }

                developer.SetInitialSkillLevel(skill, StartingSkillLevel);
            }
        }
    }
}