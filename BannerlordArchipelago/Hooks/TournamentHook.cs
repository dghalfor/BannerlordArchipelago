using HarmonyLib;
using SandBox.Tournaments.MissionLogics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace BannerlordArchipelago.Hooks
{
    [HarmonyPatch(typeof(CampaignEventDispatcher), nameof(CampaignEventDispatcher.OnTournamentFinished))]
    public static class TournamentWinPatch
    {
        public static void Prefix(CharacterObject winner, MBReadOnlyList<CharacterObject> participants, Town town, ItemObject prize)
        {
            if (winner != CharacterObject.PlayerCharacter) return;

            string locationName = $"Tournament Win - {town.Name}";
            Main.APClient.SendLocationCheck(locationName);
        }
    }
}
