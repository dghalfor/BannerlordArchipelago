using BannerlordArchipelago.Archipelago;
using HarmonyLib;
using SandBox.Tournaments.MissionLogics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.GameComponents;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace BannerlordArchipelago.Hooks
{

    public static class TournamentPassData
    {
        // Map from town name → required pass item name
        public static readonly Dictionary<string, string> TownPass = new Dictionary<string, string>
        {
            // Aserai
            { "Askar",      "Tournament Pass - Aserai" },
            { "Hubyar",     "Tournament Pass - Aserai" },
            { "Husn Fulq",  "Tournament Pass - Aserai" },
            { "Iyakis",     "Tournament Pass - Aserai" },
            { "Qasira",     "Tournament Pass - Aserai" },
            { "Quyaz",      "Tournament Pass - Aserai" },
            { "Razih",      "Tournament Pass - Aserai" },
            { "Sanala",     "Tournament Pass - Aserai" },
            // Battania
            { "Car Banseth","Tournament Pass - Battania" },
            { "Dunglanys",  "Tournament Pass - Battania" },
            { "Marunath",   "Tournament Pass - Battania" },
            { "Pen Cannoc", "Tournament Pass - Battania" },
            { "Seonon",     "Tournament Pass - Battania" },
            // Khuzait
            { "Akkalat",    "Tournament Pass - Khuzait" },
            { "Baltakhand", "Tournament Pass - Khuzait" },
            { "Chaikand",   "Tournament Pass - Khuzait" },
            { "Makeb",      "Tournament Pass - Khuzait" },
            { "Odokh",      "Tournament Pass - Khuzait" },
            { "Ortongard",  "Tournament Pass - Khuzait" },
            // Sturgia
            { "Balgard",    "Tournament Pass - Sturgia" },
            { "Omor",       "Tournament Pass - Sturgia" },
            { "Revyl",      "Tournament Pass - Sturgia" },
            { "Sibir",      "Tournament Pass - Sturgia" },
            { "Tyal",       "Tournament Pass - Sturgia" },
            { "Varcheg",    "Tournament Pass - Sturgia" },
            { "Varnovapol", "Tournament Pass - Sturgia" },
            // Vlandia
            { "Charas",     "Tournament Pass - Vlandia" },
            { "Galend",     "Tournament Pass - Vlandia" },
            { "Jaculan",    "Tournament Pass - Vlandia" },
            { "Ocs Hall",   "Tournament Pass - Vlandia" },
            { "Ostican",    "Tournament Pass - Vlandia" },
            { "Pravend",    "Tournament Pass - Vlandia" },
            { "Rovalt",     "Tournament Pass - Vlandia" },
            { "Sargot",     "Tournament Pass - Vlandia" },
            // Northern Empire
            { "Amprela",    "Tournament Pass - Northern Empire" },
            { "Argoron",    "Tournament Pass - Northern Empire" },
            { "Diathma",    "Tournament Pass - Northern Empire" },
            { "Epicrotea",  "Tournament Pass - Northern Empire" },
            { "Myzea",      "Tournament Pass - Northern Empire" },
            { "Saneopa",    "Tournament Pass - Northern Empire" },
            // Southern Empire
            { "Danustica",  "Tournament Pass - Southern Empire" },
            { "Lycaron",    "Tournament Pass - Southern Empire" },
            { "Onira",      "Tournament Pass - Southern Empire" },
            { "Phycaon",    "Tournament Pass - Southern Empire" },
            { "Poros",      "Tournament Pass - Southern Empire" },
            { "Syronea",    "Tournament Pass - Southern Empire" },
            { "Vostrum",    "Tournament Pass - Southern Empire" },
            // Western Empire
            { "Amitatys",   "Tournament Pass - Western Empire" },
            { "Jalmarys",   "Tournament Pass - Western Empire" },
            { "Lageta",     "Tournament Pass - Western Empire" },
            { "Ortysia",    "Tournament Pass - Western Empire" },
            { "Rhotae",     "Tournament Pass - Western Empire" },
            { "Zeonica",    "Tournament Pass - Western Empire" },
        };
    }
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
    [HarmonyPatch(typeof(DefaultSettlementAccessModel), "CanMainHeroJoinTournament")]
    public static class TournamentJoinPatch
    {
        public static bool Prefix(Settlement settlement, ref bool disableOption, ref TextObject disabledText, ref bool __result)
        {
            string townName = settlement.Town?.Name?.ToString();
            if (townName == null) return true;

            if (!TournamentPassData.TownPass.TryGetValue(townName, out string requiredPass)) return true;

            if (ReceivedItemsTracker.GetCount(requiredPass) <= 0)
            {
                InformationManager.DisplayMessage(new InformationMessage(
                    $"Cannot join tournament: {requiredPass} is required.", Colors.Yellow));
                return false;
            }
            return true;
        }
    }
}
