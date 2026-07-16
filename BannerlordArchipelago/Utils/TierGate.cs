using BannerlordArchipelago.Archipelago;
using TaleWorlds.CampaignSystem;
using System.Collections.Generic;

public static class TierGate
{
    private static readonly HashSet<string> BanditCultures = new HashSet<string>
    {
        "mountain_bandits", "forest_bandits", "desert_bandits",
        "steppe_bandits", "southern_pirates", "sea_raiders"
        // looters excluded entirely - too irregular a shape
    };

    public static bool TryGate(CharacterObject character, int targetTier, out string reason)
    {
        reason = null;
        if (character?.Culture?.StringId == null) return true;

        string cultureId = character.Culture.StringId;
        string itemName = BanditCultures.Contains(cultureId)
            ? "Progressive Bandit Troop Tier"
            : $"Progressive {cultureId} Troop Tier";

        int tiersUnlocked = ReceivedItemsTracker.GetCount(itemName);

        if (tiersUnlocked < targetTier - 1)
        {
            reason = $"Cannot acquire {character.Name} (Tier {targetTier}). " +
                      $"You need {targetTier - 1} '{itemName}' but have {tiersUnlocked}.";
            return false;
        }
        return true;
    }
}