using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.CampaignBehaviors;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.ObjectSystem;

namespace BannerlordArchipelago.Archipelago
{
    public static class ItemGranter
    {
        // ── Denars ──────────────────────────────────────────────────────────
        public static void GiveDenars(int amount)
        {
            GiveGoldAction.ApplyBetweenCharacters(null, Hero.MainHero, amount, false);
        }

        // ── Butter (or any item by string ID) ───────────────────────────────
        public static void GiveItem(string itemId, int count = 1)
        {
            var itemObject = MBObjectManager.Instance.GetObject<ItemObject>(itemId);
            if (itemObject == null)
            {
                Log($"GiveItem: could not find item '{itemId}'");
                return;
            }
            var element = new ItemRosterElement(itemObject, count);
            Hero.MainHero.PartyBelongedTo?.ItemRoster.AddToCounts(itemObject, count);
        }

        // ── Horses (by culture or generic) ──────────────────────────────────
        // Pass a specific item ID like "aserai_horse" or "t3_horse" etc.
        // If you want a generic mount, "sumpter_horse" is always available.
        public static void GiveHorse(string horseItemId = "sumpter_horse", int count = 1)
        {
            GiveItem(horseItemId, count);
        }

        // ── Renown ────────────────────────────────────────────────────────
        public static void GiveRenown(float amount)
        {
            if (Hero.MainHero.Clan == null)
            {
                Log("GiveRenown: player has no clan");
                return;
            }
            GainRenownAction.Apply(Hero.MainHero, amount, false);
        }

        // ── Focus / Attribute Points ─────────────────────────────────────────
        // Replaces the old direct-XP scrolls. Player still invests these points
        // through the normal game UI is bypassed entirely here - AddFocus/AddAttribute
        // invest the point directly, matching the "no manual assignment" design
        // decision (see ClearUnspentPoints in the campaign behavior).
        private static void LogToFile(string msg)
        {
            try
            {
                System.IO.File.AppendAllText(
                    System.IO.Path.Combine(BasePath.Name, "Modules", "BannerlordArchipelago", "ap_debug.log"),
                    $"{DateTime.Now:HH:mm:ss.fff} {msg}\n");
            }
            catch { /* never let logging itself crash the game */ }
        }

        public static void GrantFocusPoint(SkillObject skill)
        {
            LogToFile($"GrantFocusPoint: about to call AddFocus for {skill?.StringId}");
            // checkFocusLimit: true - respects the hard vanilla cap of 5 per skill.
            // If already at cap, this call should no-op safely; confirm against
            // your decompile that AddFocus doesn't throw when already at max.
            Hero.MainHero.HeroDeveloper.AddFocus(skill, 1, true);
            LogToFile($"GrantFocusPoint: AddFocus succeeded for {skill?.StringId}");
        }

        public static void GrantAttributePoint(CharacterAttribute attribute)
        {
            int before = Hero.MainHero.GetAttributeValue(attribute);
            LogToFile($"GrantAttributePoint: about to call AddAttribute for {attribute?.StringId}, current value={before}");
            // checkAttributeLimit: true - respects the cap of 10.
            Hero.MainHero.HeroDeveloper.AddAttribute(attribute, 1, false);
            int after = Hero.MainHero.GetAttributeValue(attribute);
            LogToFile($"GrantAttributePoint: AddAttribute called for {attribute?.StringId}, before={before}, after={after}");
        }

        // ── Hero Level (via XP on a broad skill spread) ──────────────────────
        // Bannerlord levels are driven by skill XP, not a direct level field.
        // AddSkillXp on a broad set of skills is the safest way to push a level.
        // Alternatively, HeroDeveloper.UnlockGenericPerks() triggers perk points.
        // NOTE: kept only for the still-unhandled "Hero Level" item - no longer
        // used for skillsanity scrolls, which have been replaced by focus/attribute grants above.
        public static void GiveSkillXp(SkillObject skill, float amount)
        {
            Hero.MainHero.AddSkillXp(skill, amount);
        }

        // Give +delta relation with every living lord in a given kingdom by StringId,
        // or all kingdoms if kingdomStringId is null.
        public static void GiveRelationWithAllLords(int delta, string kingdomStringId = null)
        {
            ArchipelagoItems.SuppressCharmGain = true;
            try
            {
                foreach (var hero in Hero.AllAliveHeroes)
                {
                    if (!hero.IsLord || hero == Hero.MainHero) continue;
                    if (kingdomStringId != null && hero.MapFaction?.StringId != kingdomStringId) continue;
                    ChangeRelationAction.ApplyRelationChangeBetweenHeroes(Hero.MainHero, hero, delta, false);
                }
            }
            finally
            {
                ArchipelagoItems.SuppressCharmGain = false;
            }
        }
        public static class CraftingPlanBundleItem
        {
            private const int PlansPerBundle = 10;

            private static readonly FieldInfo OpenedPartsDictionaryField = typeof(CraftingCampaignBehavior)
                .GetField("_openedPartsDictionary", BindingFlags.NonPublic | BindingFlags.Instance);

            public static void Grant(ICraftingCampaignBehavior craftingBehavior)
            {
                var behavior = craftingBehavior as CraftingCampaignBehavior;
                if (behavior == null) return;

                var openedParts = (Dictionary<CraftingTemplate, MBList<CraftingPiece>>)
                    OpenedPartsDictionaryField.GetValue(behavior);

                var locked = new List<(CraftingPiece piece, CraftingTemplate template)>();

                foreach (var template in CraftingTemplate.All)
                    foreach (var piece in template.Pieces)
                        if (!piece.IsHiddenOnDesigner && !craftingBehavior.IsOpened(piece, template))
                            locked.Add((piece, template));

                if (locked.Count == 0)
                {
                    Log("Crafting Plan Bundle — all plans already known!");
                    return;
                }

                var toUnlock = locked
                    .OrderBy(_ => MBRandom.RandomFloat)
                    .Take(PlansPerBundle)
                    .ToList();

                foreach (var (piece, template) in toUnlock)
                {
                    openedParts[template].Add(piece);
                    CampaignEventDispatcher.Instance.CraftingPartUnlocked(piece);
                }
            }
        }

        private static void Log(string msg)
        {
            InformationManager.DisplayMessage(new InformationMessage($"[AP] {msg}", Colors.Red));
        }
    }
}