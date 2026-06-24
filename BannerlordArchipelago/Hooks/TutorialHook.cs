using Archipelago;
using BannerlordArchipelago.Data;
using HarmonyLib;
using StoryMode.StoryModePhases;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BannerlordArchipelago.Hooks
{
    [HarmonyPatch(typeof(TutorialPhase), nameof(TutorialPhase.SetTutorialQuestPhase))]
    public static class TutorialPatch
    {
        public static void Postfix(TutorialPhase __instance, TutorialQuestPhase tutorialQuestPhase)
        {
            Main.APClient.SendLocationCheck(tutorialQuestPhase.ToString());
        }
    }
}
