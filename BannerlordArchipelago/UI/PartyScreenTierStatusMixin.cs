using Bannerlord.UIExtenderEx.Attributes;
using Bannerlord.UIExtenderEx.ViewModels;
using System;
using System.Linq;
using System.Text.RegularExpressions;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.ViewModelCollection.Party; // TODO verify namespace via decompile
using TaleWorlds.Library;

namespace BannerlordArchipelago.Archipelago.UI
{
    [ViewModelMixin]
    public class PartyScreenTierStatusMixin : BaseViewModelMixin<PartyVM> // TODO verify exact VM class name
    {
        public static event Action OnTierItemReceived;

        public static void RaiseTierItemReceived()
        {
            OnTierItemReceived?.Invoke();
        }

        private readonly MBBindingList<CultureTierStatusVM> _cultureTierStatuses
            = new MBBindingList<CultureTierStatusVM>();

        public PartyScreenTierStatusMixin(PartyVM vm) : base(vm)
        {
            RefreshTierStatuses();
            OnTierItemReceived += RefreshValues;
        }

        [DataSourceProperty]
        public MBBindingList<CultureTierStatusVM> CultureTierStatuses => _cultureTierStatuses;

        public void RefreshTierStatuses()
        {
            InformationManager.DisplayMessage(new InformationMessage(
    $"[AP-Debug] test",
    Colors.Yellow
));
            _cultureTierStatuses.Clear();

            var allCultures = TaleWorlds.CampaignSystem.Campaign.Current.ObjectManager
                .GetObjectTypeList<CultureObject>();

            foreach (var culture in allCultures)
            {
                int maxTier = CultureTierInfo.GetMaxTier(culture);
                int troopCount = CultureTierInfo.GetTroopsForCulture(culture).Count;

                InformationManager.DisplayMessage(new InformationMessage(
                    $"[AP-Debug] {culture.StringId}: IsMainCulture={culture.IsMainCulture}, Troops={troopCount}, MaxTier={maxTier}",
                    Colors.Yellow
                ));

                if (culture.IsMainCulture && maxTier > 0)
                    _cultureTierStatuses.Add(new CultureTierStatusVM(culture));
            }
        }

        private void RefreshValues()
        {
            foreach (var status in _cultureTierStatuses)
                status.Refresh();
        }

        public override void OnFinalize()
        {
            OnTierItemReceived -= RefreshValues;
            base.OnFinalize();
        }
    }
}