using BannerlordArchipelago.Archipelago;
using HarmonyLib;
using System;
using TaleWorlds.CampaignSystem.CampaignBehaviors;
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
                    $"[AP] CraftingFreeBuildPatch exception: {e.Message}", Colors.Red));
            }
        }
    }

    [HarmonyPatch(typeof(CraftingCampaignBehavior), "GetOrderResult")]
    public static class CraftingOrderPatch
    {
        public static void Postfix(ItemObject craftedItem, bool isSucceed)
        {
            try
            {
                if (craftedItem == null || !isSucceed) return;
                CraftingShared.HandleCraftedWeapon(craftedItem);
            }
            catch (Exception e)
            {
                InformationManager.DisplayMessage(new InformationMessage(
                    $"[AP] CraftingOrderPatch exception: {e.Message}", Colors.Red));
            }
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
    }
}