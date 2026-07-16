using BannerlordArchipelago.Archipelago;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.CampaignSystem.ViewModelCollection.GameMenu.Recruitment;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace BannerlordArchipelago.Hooks
{
    [HarmonyPatch(typeof(RecruitmentVM), "OnRecruit")]
    public static class RecruitmentOnRecruitPatch
    {
        // Set by the RecruitAll wrapper below; null means "not in a batch", so
        // single clicks still show their own message immediately.
        public static List<string> BatchedBlockReasons = null;

        public static bool Prefix(RecruitVolunteerTroopVM recruitTroop)
        {
            try
            {
                var character = recruitTroop?.Character;
                if (character?.Culture?.StringId == null) return true;

                if (!TierGate.TryGate(character, character.Tier, out string reason))
                {
                    if (BatchedBlockReasons != null)
                        BatchedBlockReasons.Add(character.Name.ToString());
                    else
                        InformationManager.DisplayMessage(new InformationMessage(reason, Colors.Red));
                    return false;
                }
                return true;
            }
            catch (Exception e)
            {
                InformationManager.DisplayMessage(new InformationMessage(
                    $"[AP Debug] Exception in RecruitmentOnRecruitPatch: {e.Message}", Colors.Red));
                return true;
            }
        }
    }

    [HarmonyPatch(typeof(RecruitmentVM), "ExecuteRecruitAll")]
    public static class RecruitmentExecuteRecruitAllPatch
    {
        public static void Prefix()
        {
            RecruitmentOnRecruitPatch.BatchedBlockReasons = new List<string>();
        }

        public static void Postfix()
        {
            var blocked = RecruitmentOnRecruitPatch.BatchedBlockReasons;
            RecruitmentOnRecruitPatch.BatchedBlockReasons = null;

            if (blocked != null && blocked.Count > 0)
            {
                var names = string.Join(", ", blocked.Distinct());
                InformationManager.DisplayMessage(new InformationMessage(
                    $"Skipped {blocked.Count} troop(s) needing higher tier unlocks: {names}",
                    Colors.Red
                ));
            }
        }
    }
}