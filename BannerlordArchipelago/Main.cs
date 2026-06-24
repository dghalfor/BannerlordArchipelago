using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
            APClient.Connect();
        }

        public override void OnGameLoaded(Game game, object initializeData)
        {
            base.OnGameLoaded(game, initializeData);

        }
        protected override void OnSubModuleUnloaded()
        {
            base.OnSubModuleUnloaded();
            APClient.Disconnect();
        }

    }
}
