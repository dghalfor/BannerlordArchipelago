using System;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Library;

namespace BannerlordArchipelago.Archipelago.UI
{
    public class CultureTierStatusVM : ViewModel
    {
        private readonly CultureObject _culture;

        public CultureTierStatusVM(CultureObject culture)
        {
            _culture = culture;
            PopulateTroops();
        }

        [DataSourceProperty]
        public string CultureName => _culture.Name.ToString();

        [DataSourceProperty]
        public int MaxTier => CultureTierInfo.GetMaxTier(_culture);

        [DataSourceProperty]
        public int UnlockedTier
        {
            get
            {
                int received = ProgressiveTierStatus.GetReceivedCount(_culture);
                int unlocked = received + 1; // 0 items received = tier 1 already unlocked
                int max = MaxTier;
                return max > 0 ? Math.Min(unlocked, max) : unlocked;
            }
        }

        [DataSourceProperty]
        public string TierText => $"Tier {UnlockedTier}/{MaxTier}";

        [DataSourceProperty]
        public bool IsMaxed => UnlockedTier >= MaxTier;

        [DataSourceProperty]
        public MBBindingList<TroopStatusVM> Troops { get; } = new MBBindingList<TroopStatusVM>();

        private void PopulateTroops()
        {
            Troops.Clear();
            foreach (var troop in CultureTierInfo.GetTroopsForCulture(_culture))
                Troops.Add(new TroopStatusVM(troop, this));
        }

        public void Refresh()
        {
            OnPropertyChanged(nameof(UnlockedTier));
            OnPropertyChanged(nameof(TierText));
            OnPropertyChanged(nameof(IsMaxed));
            foreach (var troop in Troops)
                troop.Refresh();
        }
    }
}