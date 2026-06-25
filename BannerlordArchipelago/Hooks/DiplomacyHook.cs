using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem.GameComponents;
using BannerlordArchipelago.Archipelago;

namespace BannerlordArchipelago.Hooks
{
    [HarmonyPatch(typeof(DefaultDiplomacyModel), "GetCharmExperienceFromRelationGain")]
    public static class SuppressAPCharmPatch
    {
        public static void Postfix(ref int __result)
        {
            if (ArchipelagoItems.SuppressCharmGain)
                __result = 0;
        }
    }
}
