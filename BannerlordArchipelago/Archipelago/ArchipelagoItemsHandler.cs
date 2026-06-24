using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.Library;

namespace BannerlordArchipelago.Archipelago
{
    public static class ArchipelagoItems
    {
        public const string DragonBannerPiece = "Dragon Banner Piece";
    }

    public static class ReceivedItemsTracker
    {
        private static readonly Dictionary<string, int> _itemCounts = new Dictionary<string, int>();

        public static void OnItemReceived(string itemName)
        {
            if (_itemCounts.ContainsKey(itemName))
                _itemCounts[itemName]++;
            else
                _itemCounts[itemName] = 1;

            HandleItem(itemName);
        }

        public static int GetCount(string itemName)
        {
            return _itemCounts.TryGetValue(itemName, out int count) ? count : 0;
        }

        public static void Reset()
        {
            _itemCounts.Clear();
        }

        private static void HandleItem(string itemName)
        {
            switch (itemName)
            {
                case ArchipelagoItems.DragonBannerPiece:
                    InformationManager.DisplayMessage(new InformationMessage(
                        $"Received a Dragon Banner Piece! " +
                        $"({GetCount(ArchipelagoItems.DragonBannerPiece)}/{ArchipelagoClient.ServerData.DragonBannerPiecesRequired})",
                        Colors.Cyan
                    ));
                    break;

                    // Add other items here as you build them out
            }
        }
    }
    
}
