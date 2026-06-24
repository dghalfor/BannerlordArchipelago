using BannerlordArchipelago.Archipelago;
using HarmonyLib;
using StoryMode.StoryModePhases;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Library;

namespace BannerlordArchipelago.Hooks
{

    [HarmonyPatch(typeof(FirstPhase), nameof(FirstPhase.MergeDragonBanner))]
    public class MergeDragonBannerPatch
    {
        static bool Prefix()
        {
            int piecesReceived = ReceivedItemsTracker.GetCount(ArchipelagoItems.DragonBannerPiece);

            int piecesRequired = ArchipelagoClient.ServerData.DragonBannerPiecesRequired;

            if (piecesReceived < piecesRequired)
            {
                // Optionally notify the player why it's blocked
                InformationManager.DisplayMessage(new InformationMessage(
                    $"You need {piecesRequired} Dragon Banner Pieces to merge the banner. " +
                    $"You have {piecesReceived}/{piecesRequired}.",
                    Colors.Red
                ));
                return false; // Block the original method
            }

            return true; // Allow the merge
        }
    }
}
