using Archipelago.MultiClient.Net.Helpers;
using BannerlordArchipelago.Archipelago;
using BannerlordArchipelago.CampaignBehaviors;
using HarmonyLib;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;

namespace BannerlordArchipelago
{
    public class Main : MBSubModuleBase
    {
        public static ArchipelagoClient APClient = new ArchipelagoClient();
        private Harmony _harmony;

        protected override void OnSubModuleLoad()
        {
            base.OnSubModuleLoad();
            _harmony = new Harmony("mod.bannerlord.bannerlordarchipelago");
            _harmony.PatchAll(System.Reflection.Assembly.GetExecutingAssembly());
            // No connect here — settings aren't loaded yet and there's no campaign
        }

        protected override void OnBeforeInitialModuleScreenSetAsRoot()
        {
            base.OnBeforeInitialModuleScreenSetAsRoot();
            // MCM settings are available by this point
        }

        protected override void OnGameStart(Game game, IGameStarter gameStarter)
        {
            base.OnGameStart(game, gameStarter);
            if (gameStarter is CampaignGameStarter campaignStarter)
            {
                campaignStarter.AddBehavior(new ArchipelagoCampaignBehavior());
                campaignStarter.AddBehavior(new APTournamentDialogBehavior());
            }
        }
        public override void OnGameEnd(Game game)
        {
            base.OnGameEnd(game);
            APClient.Disconnect();
            ReceivedItemsTracker.IsReady = false;
        }

        protected override void OnSubModuleUnloaded()
        {
            base.OnSubModuleUnloaded();
            APClient.Disconnect();
            ReceivedItemsTracker.IsReady = false;
        }
    }
}