using TaleWorlds.CampaignSystem;
using TaleWorlds.Library;

namespace BannerlordArchipelago.Archipelago.UI
{
    public class TroopStatusVM : ViewModel
    {
        private readonly CharacterObject _troop;
        private readonly CultureTierStatusVM _parent;

        public TroopStatusVM(CharacterObject troop, CultureTierStatusVM parent)
        {
            _troop = troop;
            _parent = parent;
        }

        [DataSourceProperty]
        public string TroopName => _troop.Name.ToString();

        [DataSourceProperty]
        public int Tier => _troop.Tier;

        [DataSourceProperty]
        public bool IsUnlocked => Tier <= _parent.UnlockedTier;

        public void Refresh() => OnPropertyChanged(nameof(IsUnlocked));
    }
}