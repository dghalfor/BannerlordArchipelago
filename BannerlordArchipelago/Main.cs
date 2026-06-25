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
            }
        }

        protected override void OnSubModuleUnloaded()
        {
            base.OnSubModuleUnloaded();
            APClient.Disconnect();
        }
    }
}