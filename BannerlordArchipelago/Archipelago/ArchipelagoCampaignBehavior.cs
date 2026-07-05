using BannerlordArchipelago.Archipelago;
using BannerlordArchipelago.Hooks;
using BannerlordArchipelago.Settings;
using System;
using System.Collections.Generic;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.CharacterCreationContent;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace BannerlordArchipelago
{
    public class ArchipelagoCampaignBehavior : CampaignBehaviorBase
    {
        public override void RegisterEvents()
        {
            CampaignEvents.OnNewGameCreatedEvent.AddNonSerializedListener(this, OnNewGameCreated);
            CampaignEvents.OnCharacterCreationIsOverEvent.AddNonSerializedListener(this, OnCharacterCreationIsOver);
            CampaignEvents.OnGameLoadedEvent.AddNonSerializedListener(this, OnGameLoaded);
            CampaignEvents.DailyTickEvent.AddNonSerializedListener(this, OnDailyTick);
            CampaignEvents.HourlyTickEvent.AddNonSerializedListener(this, OnHourlyTick);
            CampaignEvents.HeroLevelledUp.AddNonSerializedListener(this, OnHeroLevelledUp);
        }

        public static int _savedItemIndex = 0;
        private bool _connectCalled = false;
        private bool _pendingProgressionReset;

        public override void SyncData(IDataStore dataStore)
        {
            if (dataStore.IsSaving)
            {
                _savedItemIndex = ArchipelagoClient.ServerData.Index;
                InformationManager.DisplayMessage(new InformationMessage(
                    $"[AP] Saving index: {_savedItemIndex}", Colors.Yellow));
            }

            dataStore.SyncData("AP_ItemIndex", ref _savedItemIndex);

            dataStore.SyncData("APBannerlord_PendingProgressionReset", ref _pendingProgressionReset);

            if (!dataStore.IsSaving)
            {
                InformationManager.DisplayMessage(new InformationMessage(
                    $"[AP] Loaded index: {_savedItemIndex}", Colors.Yellow));
            }
        }

        private void OnNewGameCreated(CampaignGameStarter starter)
        {
            _checkedFoodVarieties.Clear();
            ReceivedItemsTracker.Reset();
            ArchipelagoCampaignBehavior._savedItemIndex = 0;
            // We gotta clear the stats a hero is set with at the beginning, this will force that to occur once the session has actually launched
            _pendingProgressionReset = true;
            _connectCalled = false;
            TryConnect();
        }

        private void OnCharacterCreationIsOver()
        {
            InformationManager.DisplayMessage(new InformationMessage("AP: OnCharacterCreationIsOver fired"));

            var h = Hero.MainHero;
            InformationManager.DisplayMessage(new InformationMessage(
                $"AP: Before reset - Vigor={h.GetAttributeValue(DefaultCharacterAttributes.Vigor)}, " +
                $"OneHanded lvl={h.GetSkillValue(DefaultSkills.OneHanded)}"));

            SkillHelper.ResetHeroProgressionForRando(h);

            InformationManager.DisplayMessage(new InformationMessage(
                $"AP: After reset - Vigor={h.GetAttributeValue(DefaultCharacterAttributes.Vigor)}, " +
                $"OneHanded lvl={h.GetSkillValue(DefaultSkills.OneHanded)}"));
        }

        private void OnGameLoaded(CampaignGameStarter starter)
        {
            _checkedFoodVarieties.Clear();
            ReceivedItemsTracker.Reset();

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
                if (_checkedFoodVarieties.Add(variety))
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
        private void OnHourlyTick()
        {
            Main.APClient.Flush();
            if (!ReceivedItemsTracker.IsReady)
            {
                ReceivedItemsTracker.IsReady = true;
                ReceivedItemsTracker.OnItemReceived();
            }
        }
        private void OnHeroLevelledUp(Hero hero, bool shouldNotify)
        {
            if (hero != Hero.MainHero) return;
            HeroLevelUpHook.ClearUnspentPoints(hero);
        }
    }
}