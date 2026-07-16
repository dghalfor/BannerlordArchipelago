using System;
using System.Reflection;
using HarmonyLib;
using SandBox.GauntletUI;
using TaleWorlds.Core; // TODO: verify this is the correct namespace for IGameStateListener — your IDE will flag it if wrong

namespace BannerlordArchipelago.Archipelago.UI
{
    [HarmonyPatch]
    public static class PartyScreenOverlayOpenPatch
    {
        private static readonly TierStatusGauntletLayer _overlay = new TierStatusGauntletLayer();
        public static TierStatusGauntletLayer Overlay => _overlay;

        static MethodBase TargetMethod()
        {
            var interfaceMethod = typeof(IGameStateListener).GetMethod("OnActivate");
            var map = typeof(GauntletPartyScreen).GetInterfaceMap(typeof(IGameStateListener));
            int idx = Array.IndexOf(map.InterfaceMethods, interfaceMethod);
            return map.TargetMethods[idx];
        }

        static void Postfix()
        {
            _overlay.Open();
        }
    }

    [HarmonyPatch]
    public static class PartyScreenOverlayClosePatch
    {
        static MethodBase TargetMethod()
        {
            var interfaceMethod = typeof(IGameStateListener).GetMethod("OnFinalize");
            var map = typeof(GauntletPartyScreen).GetInterfaceMap(typeof(IGameStateListener));
            int idx = Array.IndexOf(map.InterfaceMethods, interfaceMethod);
            return map.TargetMethods[idx];
        }

        static void Prefix()
        {
            PartyScreenOverlayOpenPatch.Overlay.Close();
        }
    }
}