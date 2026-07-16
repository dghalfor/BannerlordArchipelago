using System.Collections.Generic;
using TaleWorlds.CampaignSystem;

namespace BannerlordArchipelago.Archipelago
{
    public static class ProgressiveTierStatus
    {
        private static readonly Dictionary<string, string> _cultureToItem = new Dictionary<string, string>
        {
            { "sturgia", ArchipelagoItems.ProgressiveSturgiaTroopTier },
            { "empire", ArchipelagoItems.ProgressiveEmpireTroopTier },
            { "battania", ArchipelagoItems.ProgressiveBattaniaTroopTier },
            { "vlandia", ArchipelagoItems.ProgressiveVlandiaTroopTier },
            { "khuzait", ArchipelagoItems.ProgressiveKhuzaitTroopTier },
            { "aserai", ArchipelagoItems.ProgressiveAseraiTroopTier },
            { "nord", ArchipelagoItems.ProgressiveNordTroopTier },
        };

        public static int GetReceivedCount(CultureObject culture)
        {
            if (culture == null) return 0;
            return _cultureToItem.TryGetValue(culture.StringId, out var itemName)
                ? ReceivedItemsTracker.GetCount(itemName)
                : 0;
        }
    }
}