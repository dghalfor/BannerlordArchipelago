using BannerlordArchipelago.Archipelago;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Reflection;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.CampaignBehaviors;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace BannerlordArchipelago.Hooks
{
    [HarmonyPatch(typeof(PartyScreenLogic), "UpgradeTroop")]
    public static class UpgradeTroopPatch
    {
        public static bool Prefix(object command)
        {
            try
            {
                var commandType = command.GetType();
                var character = (CharacterObject)commandType.GetProperty("Character").GetValue(command);
                var upgradeTargetIndex = (int)commandType.GetProperty("UpgradeTarget").GetValue(command);
                var characterObject = character.UpgradeTargets[upgradeTargetIndex];

                if (character?.Culture?.StringId == null) return true;

                int targetTier = characterObject.Tier;
                string cultureName = character.Culture.StringId;
                string itemName = $"Progressive {cultureName} Troop Tier";
                int tiersUnlocked = ReceivedItemsTracker.GetCount(itemName);

                if (tiersUnlocked < targetTier - 1)
                {
                    InformationManager.DisplayMessage(new InformationMessage(
                        $"Cannot upgrade to {characterObject.Name} (Tier {targetTier}). " +
                        $"You need {targetTier - 1} '{itemName}' but have {tiersUnlocked}.",
                        Colors.Red
                    ));
                    return false;
                }

                return true;
            }
            catch (Exception e)
            {
                InformationManager.DisplayMessage(new InformationMessage(
                    $"[AP Debug] Exception in UpgradeTroopPatch: {e.Message}",
                    Colors.Red
                ));
                return true;
            }
        }
    }

    [HarmonyPatch(typeof(PartyScreenLogic), "DoneLogic")]
    public static class DoneLogicPatch
    {
        public static void Prefix(PartyScreenLogic __instance, out List<Tuple<CharacterObject, CharacterObject, int>> __state)
        {
            // Snapshot the history before DoneLogic clears it
            __state = new List<Tuple<CharacterObject, CharacterObject, int>>(
                __instance.CurrentData.UpgradedTroopsHistory
            );
        }

        public static void Postfix(bool __result, List<Tuple<CharacterObject, CharacterObject, int>> __state)
        {
            if (!__result) return; // Gold check failed or delegate rejected — nothing committed

            foreach (var (_, toCharacter, count) in __state)
            {
                if (toCharacter == null) continue;
                for (int i = 0; i < count; i++)
                {
                    string locationName = $"Upgraded to {toCharacter.Name}";
                    Main.APClient.SendLocationCheck(locationName);
                }
            }
        }
    }
}