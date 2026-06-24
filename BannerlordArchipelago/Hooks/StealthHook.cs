using HarmonyLib;
using StoryMode.Quests.TutorialPhase;
using StoryMode.StoryModePhases;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BannerlordArchipelago.Hooks
{
    [HarmonyPatch(typeof(VillagersInNeed), nameof(VillagersInNeed.OnHeadmanRescued))]
    public static class StealthPatch
    {
        public static void Postfix()
        {
            Main.APClient.SendLocationCheck("VillagersInNeed_OnHeadmanRescued");
        }

    }
}
