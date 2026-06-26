using BannerlordArchipelago.Archipelago;
using HarmonyLib;
using System;
using System.Reflection;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.CampaignBehaviors;
using TaleWorlds.CampaignSystem.CraftingSystem;
using TaleWorlds.CampaignSystem.ViewModelCollection.WeaponCrafting;
using TaleWorlds.CampaignSystem.ViewModelCollection.WeaponCrafting.WeaponDesign;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace BannerlordArchipelago.Hooks
{
    [HarmonyPatch(typeof(CraftingCampaignBehavior), "CreateCraftedWeaponInFreeBuildMode")]
    public static class CraftingFreeBuildPatch
    {
        public static void Postfix(ItemObject __result)
        {
            try
            {
                if (__result == null) return;
                CraftingShared.HandleCraftedWeapon(__result);
            }
            catch (Exception e)
            {
                InformationManager.DisplayMessage(new InformationMessage(
                    $"[AP] CraftingFreeBuildPatch postfix exception: {e.Message}", Colors.Red));
            }
        }
    }
    [HarmonyPatch(typeof(CraftingVM), "ExecuteMainAction")]
    public static class CraftingMainActionPatch
    {
        private static readonly FieldInfo CraftingField = typeof(WeaponDesignVM)
            .GetField("_crafting", BindingFlags.NonPublic | BindingFlags.Instance);

        public static bool Prefix(CraftingVM __instance)
        {
            try
            {
                var weaponDesignVM = __instance.WeaponDesign;
                if (weaponDesignVM == null) return true;

                var crafting = CraftingField.GetValue(weaponDesignVM) as Crafting;
                if (crafting == null) return true;

                return CraftingShared.CheckCraftingAllowed(crafting.CurrentWeaponDesign);
            }
            catch (Exception e)
            {
                InformationManager.DisplayMessage(new InformationMessage(
                    $"[AP] CraftingMainActionPatch exception: {e.Message}", Colors.Red));
                return true;
            }
        }
    }

    [HarmonyPatch(typeof(CraftingCampaignBehavior), "CreateCraftedWeaponInCraftingOrderMode")]
    public static class CraftingOrderModePatch
    {
        public static bool Prefix(Hero crafterHero, CraftingOrder craftingOrder, WeaponDesign weaponDesign)
        {
            try
            {
                if (CraftingShared.CheckCraftingAllowed(weaponDesign)) return true;

                // Return a null ItemObject — GetOrderResult will need to handle this gracefully
                return false;
            }
            catch (Exception e)
            {
                InformationManager.DisplayMessage(new InformationMessage(
                    $"[AP] CraftingOrderModePatch prefix exception: {e.Message}", Colors.Red));
                return true;
            }
        }

        public static void Postfix(ItemObject __result)
        {
            try
            {
                if (__result == null) return;
                CraftingShared.HandleCraftedWeapon(__result);
            }
            catch (Exception e)
            {
                InformationManager.DisplayMessage(new InformationMessage(
                    $"[AP] CraftingOrderModePatch postfix exception: {e.Message}", Colors.Red));
            }
        }
    }
    [HarmonyPatch(typeof(CraftingCampaignBehavior), "GetOrderResult")]
    public static class CraftingOrderPatch
    {
        public static bool Prefix(CraftingOrder craftingOrder, ItemObject craftedItem)
        {
            return craftedItem != null;
        }
    }
    internal static class CraftingShared
    {
        internal static void HandleCraftedWeapon(ItemObject item)
        {
            string weaponType = GetWeaponType(item);
            int tier = (int)item.Tier;

            InformationManager.DisplayMessage(new InformationMessage(
                $"[AP] Crafted: template='{weaponType}' tier={tier}",
                Colors.Yellow));

            for (int t = 1; t <= tier; t++)
            {
                Main.APClient.SendLocationCheck($"Crafted {weaponType} Tier {t}");
            }
        }

        internal static string GetWeaponType(ItemObject item)
        {
            var template = item.WeaponDesign?.Template;
            if (template != null)
                return template.StringId;

            var weapon = item.PrimaryWeapon;
            if (weapon == null) return "Unknown";

            if (weapon.IsMeleeWeapon)
            {
                if (weapon.WeaponClass == WeaponClass.TwoHandedSword) return "TwoHandedSword";
                if (weapon.WeaponClass == WeaponClass.OneHandedSword) return "OneHandedSword";
                if (weapon.WeaponClass == WeaponClass.TwoHandedAxe) return "TwoHandedAxe";
                if (weapon.WeaponClass == WeaponClass.OneHandedAxe) return "OneHandedAxe";
                if (weapon.WeaponClass == WeaponClass.TwoHandedMace) return "TwoHandedMace";
                if (weapon.WeaponClass == WeaponClass.Mace) return "Mace";
                if (weapon.WeaponClass == WeaponClass.Dagger) return "Dagger";
                if (weapon.WeaponClass == WeaponClass.TwoHandedPolearm) return "TwoHandedPolearm";
                if (weapon.WeaponClass == WeaponClass.OneHandedPolearm) return "OneHandedPolearm";
            }
            if (weapon.IsRangedWeapon || weapon.IsAmmo)
            {
                if (weapon.WeaponClass == WeaponClass.Javelin) return "Javelin";
                if (weapon.WeaponClass == WeaponClass.ThrowingAxe) return "ThrowingAxe";
                if (weapon.WeaponClass == WeaponClass.ThrowingKnife) return "ThrowingKnife";
            }

            return weapon.WeaponClass.ToString();
        }
    
    internal static bool CheckCraftingAllowed(WeaponDesign weaponDesign)
        {
            if (weaponDesign?.Template == null) return true;

            string weaponType = weaponDesign.Template.StringId;

            if (!Data.DataDicts.CraftingPlanItem.TryGetValue(weaponType, out string requiredPlan))
                return true;
            if (ReceivedItemsTracker.GetCount(requiredPlan) > 0) return true;

            InformationManager.DisplayMessage(new InformationMessage(
                $"[AP] You need '{requiredPlan}' to craft this weapon type.",
                new Color(1.0f, 0.3f, 0.3f)));
            return false;
        }
    }
}