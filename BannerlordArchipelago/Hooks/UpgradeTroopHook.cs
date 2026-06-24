using BannerlordArchipelago.Archipelago;
using HarmonyLib;
using System;
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

                InformationManager.DisplayMessage(new InformationMessage(
                    $"[AP Debug] Player upgrade. From: {character.Name} (T{character.Tier}) -> To: {characterObject.Name} (T{targetTier}), " +
                    $"Culture: {cultureName}, Unlocked: {tiersUnlocked}, Required: {targetTier - 1}",
                    Colors.Green
                ));

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
}