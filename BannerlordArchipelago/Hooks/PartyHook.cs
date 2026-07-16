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


    [HarmonyPatch(typeof(PartyScreenLogic), "TransferTroop", typeof(PartyScreenLogic.PartyCommand), typeof(bool))]
    public static class TransferTroopPatch
    {
        public static bool Prefix(PartyScreenLogic __instance, PartyScreenLogic.PartyCommand command)
        {
            try
            {
                if (command.Type != PartyScreenLogic.TroopType.Member) return true;

                var destSide = PartyScreenLogic.PartyRosterSide.Right - command.RosterSide;
                var destRoster = __instance.MemberRosters[(int)destSide];

                // Only gate when troops are actually flowing INTO the player's real roster.
                if (MobileParty.MainParty == null || !ReferenceEquals(destRoster, MobileParty.MainParty.MemberRoster))
                    return true;

                var character = command.Character;
                if (character?.Culture?.StringId == null) return true;

                if (!TierGate.TryGate(character, character.Tier, out string reason))
                {
                    InformationManager.DisplayMessage(new InformationMessage(reason, Colors.Red));
                    return false;
                }
                return true;
            }
            catch (Exception e)
            {
                InformationManager.DisplayMessage(new InformationMessage(
                    $"[AP Debug] Exception in TransferTroopPatch: {e.Message}", Colors.Red));
                return true;
            }
        }
    }

    [HarmonyPatch(typeof(PartyScreenLogic), "RecruitPrisoner")]
    public static class RecruitPrisonerPatch
    {
        public static bool Prefix(PartyScreenLogic __instance, PartyScreenLogic.PartyCommand command)
        {
            try
            {
                var destRoster = __instance.MemberRosters[(int)command.RosterSide];
                if (MobileParty.MainParty == null || !ReferenceEquals(destRoster, MobileParty.MainParty.MemberRoster))
                    return true;

                var character = command.Character;
                if (character?.Culture?.StringId == null) return true;

                if (!TierGate.TryGate(character, character.Tier, out string reason))
                {
                    InformationManager.DisplayMessage(new InformationMessage(reason, Colors.Red));
                    return false;
                }
                return true;
            }
            catch (Exception e)
            {
                InformationManager.DisplayMessage(new InformationMessage(
                    $"[AP Debug] Exception in RecruitPrisonerPatch: {e.Message}", Colors.Red));
                return true;
            }
        }
    }
}