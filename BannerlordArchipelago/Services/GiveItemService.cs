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

        // ── Hero Level (via XP on a broad skill spread) ──────────────────────
        // Bannerlord levels are driven by skill XP, not a direct level field.
        // AddSkillXp on a broad set of skills is the safest way to push a level.
        // Alternatively, HeroDeveloper.UnlockGenericPerks() triggers perk points.
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