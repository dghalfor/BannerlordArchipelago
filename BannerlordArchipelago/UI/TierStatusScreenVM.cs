using BannerlordArchipelago.Utils;
using System;
using System.Linq;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Library;

namespace BannerlordArchipelago.Archipelago.UI
{
    public class TierStatusScreenVM : ViewModel
    {
        public TierStatusScreenVM()
        {
            RefreshCultures();
        }

        [DataSourceProperty]
        public MBBindingList<CultureTierStatusVM> Cultures { get; } = new MBBindingList<CultureTierStatusVM>();

        public void RefreshCultures()
        {
            Cultures.Clear();

            var allCultures = TaleWorlds.CampaignSystem.Campaign.Current.ObjectManager
                .GetObjectTypeList<CultureObject>();

            foreach (var culture in allCultures)
            {
                int maxTier = CultureTierInfo.GetMaxTier(culture);
                int troopCount = CultureTierInfo.GetTroopsForCulture(culture).Count;

                APLog.LogToFile($"Culture: {culture.StringId}, IsMainCulture: {culture.IsMainCulture}, Troops: {troopCount}, MaxTier: {maxTier}");

                if (culture.IsMainCulture && maxTier > 0)
                    Cultures.Add(new CultureTierStatusVM(culture));
            }
        }
        public void RefreshValues()
        {
            foreach (var c in Cultures)
                c.Refresh();
        }

        // Bound to a close button in the XML below
        public void ExecuteClose()
        {
            OnClose?.Invoke();
        }

        public event System.Action OnClose;
    }
}