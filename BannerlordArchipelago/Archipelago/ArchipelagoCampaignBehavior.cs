using BannerlordArchipelago.Settings;
using System;
using System.Collections.Generic;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.Library;

namespace BannerlordArchipelago
{
    public class ArchipelagoCampaignBehavior : CampaignBehaviorBase
    {
        public override void RegisterEvents()
        {
            CampaignEvents.OnNewGameCreatedEvent.AddNonSerializedListener(this, OnNewGameCreated);
            CampaignEvents.OnGameLoadedEvent.AddNonSerializedListener(this, OnGameLoaded);
            CampaignEvents.DailyTickEvent.AddNonSerializedListener(this, OnDailyTick);
        }

        public static int _savedItemIndex = 0;
        private bool _connectCalled = false;

        public override void SyncData(IDataStore dataStore)
        {
            if (dataStore.IsSaving)
            {
                _savedItemIndex = ArchipelagoClient.ServerData.Index;
                InformationManager.DisplayMessage(new InformationMessage(
                    $"[AP] Saving index: {_savedItemIndex}", Colors.Yellow));
            }

            dataStore.SyncData("AP_ItemIndex", ref _savedItemIndex);

            if (!dataStore.IsSaving)
            {
                InformationManager.DisplayMessage(new InformationMessage(
                    $"[AP] Loaded index: {_savedItemIndex}", Colors.Yellow));
            }
        }

        private void OnNewGameCreated(CampaignGameStarter starter)
        {
            _checkedFoodVarieties.Clear();
            _connectCalled = false;
            TryConnect();
        }

        private void OnGameLoaded(CampaignGameStarter starter)
        {
            _checkedFoodVarieties.Clear();
            if (!_connectCalled)
                TryConnect();
        }

        private void TryConnect()
        {
            var s = ArchipelagoSettings.Instance;

            if (string.IsNullOrWhiteSpace(s.SlotName))
            {
                InformationManager.DisplayMessage(new InformationMessage(
                    "[AP] No slot name configured. Open Mod Options and fill in your Archipelago connection settings.",
                    Colors.Yellow
                ));
                return;
            }

            ArchipelagoClient.ServerData = new ArchipelagoData(
                uri: $"{s.Host}:{s.Port}",
                slotName: s.SlotName,
                password: s.Password
                
            );

            Main.APClient.Connect();
        }

        private static readonly HashSet<int> _checkedFoodVarieties = new HashSet<int>();

        private void OnDailyTick()
        {
            try
            {
                if (MobileParty.MainParty?.ItemRoster == null) return;

                int variety = MobileParty.MainParty.ItemRoster.FoodVariety;

                // One location per distinct variety count reached, never re-sent
                if (_checkedFoodVarieties.Add(variety) && variety > 0)
                {
                    string locationName = $"Fed Party with {variety} Food Types";
                    Main.APClient.SendLocationCheck(locationName);
                }
            }
            catch (Exception e)
            {
                InformationManager.DisplayMessage(new InformationMessage(
                    $"[AP] FoodSanity exception: {e.Message}", Colors.Red));
            }
        }
    }
}