using Archipelago.MultiClient.Net.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.CampaignBehaviors;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace BannerlordArchipelago.Archipelago
{
    public static class ArchipelagoItems
    {
        public const string DragonBannerPiece = "Dragon Banner Piece";

        // Denars
        public const string SmallPouchOfDenars = "Small Pouch of Denars";
        public const string PouchOfDenars = "Pouch of Denars";
        public const string BagOfDenars = "Bag of Denars";

        // Food
        public const string Butter = "Butter";

        // Horses
        public const string Horse = "Horse";

        // Levels
        public const string HeroLevel = "Hero Level";

        // Relations
        public const string LordRelation = "Lord Relation";

        // Renown
        public const string SmallRenown = "Small Renown";
        public const string Renown = "Renown";
        public const string LargeRenown = "Large Renown";

        // Attribute Points (progressive; count 7 per attribute, since baseline is forced to 3 and cap is 10)
        public const string VigorAttribute = "Vigor Attribute";
        public const string ControlAttribute = "Control Attribute";
        public const string EnduranceAttribute = "Endurance Attribute";
        public const string CunningAttribute = "Cunning Attribute";
        public const string SocialAttribute = "Social Attribute";
        public const string IntelligenceAttribute = "Intelligence Attribute";

        // Focus Points (progressive per skill; count 5 per skill, the hard vanilla focus cap)
        public const string OneHandedFocus = "OneHanded Focus";
        public const string TwoHandedFocus = "TwoHanded Focus";
        public const string PolearmFocus = "Polearm Focus";
        public const string BowFocus = "Bow Focus";
        public const string CrossbowFocus = "Crossbow Focus";
        public const string ThrowingFocus = "Throwing Focus";
        public const string RidingFocus = "Riding Focus";
        public const string AthleticsFocus = "Athletics Focus";
        public const string CraftingFocus = "Crafting Focus"; // in-game UI label is "Smithing"; DefaultSkills.Crafting is the actual skill
        public const string TacticsFocus = "Tactics Focus";
        public const string ScoutingFocus = "Scouting Focus";
        public const string RogueryFocus = "Roguery Focus";
        public const string CharmFocus = "Charm Focus";
        public const string LeadershipFocus = "Leadership Focus";
        public const string TradeFocus = "Trade Focus";
        public const string StewardFocus = "Steward Focus";
        public const string MedicineFocus = "Medicine Focus";
        public const string EngineeringFocus = "Engineering Focus";

        private static Dictionary<string, SkillObject> _focusItemToSkill;
        public static Dictionary<string, SkillObject> FocusItemToSkill
        {
            get
            {
                if (_focusItemToSkill == null)
                {
                    _focusItemToSkill = new Dictionary<string, SkillObject>
                    {
                        { OneHandedFocus, DefaultSkills.OneHanded },
                        { TwoHandedFocus, DefaultSkills.TwoHanded },
                        { PolearmFocus, DefaultSkills.Polearm },
                        { BowFocus, DefaultSkills.Bow },
                        { CrossbowFocus, DefaultSkills.Crossbow },
                        { ThrowingFocus, DefaultSkills.Throwing },
                        { RidingFocus, DefaultSkills.Riding },
                        { AthleticsFocus, DefaultSkills.Athletics },
                        { CraftingFocus, DefaultSkills.Crafting },
                        { TacticsFocus, DefaultSkills.Tactics },
                        { ScoutingFocus, DefaultSkills.Scouting },
                        { RogueryFocus, DefaultSkills.Roguery },
                        { CharmFocus, DefaultSkills.Charm },
                        { LeadershipFocus, DefaultSkills.Leadership },
                        { TradeFocus, DefaultSkills.Trade },
                        { StewardFocus, DefaultSkills.Steward },
                        { MedicineFocus, DefaultSkills.Medicine },
                        { EngineeringFocus, DefaultSkills.Engineering },
                    };
                }
                return _focusItemToSkill;
            }
        }

        private static Dictionary<string, CharacterAttribute> _attributeItemToAttribute;
        public static Dictionary<string, CharacterAttribute> AttributeItemToAttribute
        {
            get
            {
                if (_attributeItemToAttribute == null)
                {
                    _attributeItemToAttribute = new Dictionary<string, CharacterAttribute>
                    {
                        { VigorAttribute, DefaultCharacterAttributes.Vigor },
                        { ControlAttribute, DefaultCharacterAttributes.Control },
                        { EnduranceAttribute, DefaultCharacterAttributes.Endurance },
                        { CunningAttribute, DefaultCharacterAttributes.Cunning },
                        { SocialAttribute, DefaultCharacterAttributes.Social },
                        { IntelligenceAttribute, DefaultCharacterAttributes.Intelligence },
                    };
                }
                return _attributeItemToAttribute;
            }
        }

        public const string ProgressiveSturgiaTroopTier = "Progressive sturgia Troop Tier";
        public const string ProgressiveEmpireTroopTier = "Progressive empire Troop Tier";
        public const string ProgressiveBattaniaTroopTier = "Progressive battania Troop Tier";
        public const string ProgressiveVlandiaTroopTier = "Progressive vlandia Troop Tier";
        public const string ProgressiveKhuzaitTroopTier = "Progressive khuzait Troop Tier";
        public const string ProgressiveAseraiTroopTier = "Progressive aserai Troop Tier";
        public const string ProgressiveNordTroopTier = "Progressive nord Troop Tier";

        public const string TournamentPassAserai = "Tournament Pass - Aserai";
        public const string TournamentPassBattania = "Tournament Pass - Battania";
        public const string TournamentPassKhuzait = "Tournament Pass - Khuzait";
        public const string TournamentPassSturgia = "Tournament Pass - Sturgia";
        public const string TournamentPassVlandia = "Tournament Pass - Vlandia";
        public const string TournamentPassNorthernEmpire = "Tournament Pass - Northern Empire";
        public const string TournamentPassSouthernEmpire = "Tournament Pass - Southern Empire";
        public const string TournamentPassWesternEmpire = "Tournament Pass - Western Empire";

        public const string CraftingRecipeOneHandedSword = "Crafting Recipe - One Handed Sword";
        public const string CraftingRecipeTwoHandedSword = "Crafting Recipe - Two Handed Sword";
        public const string CraftingRecipeOneHandedAxe = "Crafting Recipe - One Handed Axe";
        public const string CraftingRecipeTwoHandedAxe = "Crafting Recipe - Two Handed Axe";
        public const string CraftingRecipeMace = "Crafting Recipe - Mace";
        public const string CraftingRecipeTwoHandedMace = "Crafting Recipe - Two Handed Mace";
        public const string CraftingRecipeDagger = "Crafting Recipe - Dagger";
        public const string CraftingRecipePike = "Crafting Recipe - Pike";
        public const string CraftingRecipeTwoHandedPolearm = "Crafting Recipe - Two Handed Polearm";
        public const string CraftingRecipeJavelin = "Crafting Recipe - Javelin";
        public const string CraftingRecipeThrowingAxe = "Crafting Recipe - Throwing Axe";
        public const string CraftingRecipeThrowingKnife = "Crafting Recipe - Throwing Knife";

        public const string CraftingPlanBundle = "Crafting Plan Bundle";

        public static bool SuppressCharmGain = false;

        public static List<string> AlwaysProcessItems()
        {
            return new List<string>
            {
                DragonBannerPiece,
                ProgressiveSturgiaTroopTier,
                ProgressiveEmpireTroopTier,
                ProgressiveBattaniaTroopTier,
                ProgressiveVlandiaTroopTier,
                ProgressiveKhuzaitTroopTier,
                ProgressiveAseraiTroopTier,
                ProgressiveNordTroopTier,
                TournamentPassAserai,
                TournamentPassBattania,
                TournamentPassKhuzait,
                TournamentPassSturgia,
                TournamentPassVlandia,
                TournamentPassNorthernEmpire,
                TournamentPassSouthernEmpire,
                TournamentPassWesternEmpire,
                CraftingRecipeOneHandedSword,
                CraftingRecipeTwoHandedSword,
                CraftingRecipeOneHandedAxe,
                CraftingRecipeTwoHandedAxe,
                CraftingRecipeMace,
                CraftingRecipeTwoHandedMace,
                CraftingRecipeDagger,
                CraftingRecipePike,
                CraftingRecipeTwoHandedPolearm,
                CraftingRecipeJavelin,
                CraftingRecipeThrowingAxe,
                CraftingRecipeThrowingKnife,
            };
        }
    }

    public static class ReceivedItemsTracker
    {
        private static readonly Dictionary<string, int> _itemCounts = new Dictionary<string, int>();
        public static bool IsReady;
        private static readonly Queue<(string itemName, int index)> _pendingItems = new Queue<(string, int)>();
        public static void EnqueuePending(string itemName, int index)
        {
            _pendingItems.Enqueue((itemName, index));
        }

        public static void OnItemReceived()
        {
            // This section avoids processing an item while in character creation. Because multiple items can be queued up before done, we loop through the queue once we are ready.
            if (!IsReady) return;
            while (_pendingItems.Count > 0)
            {
                var (itemName, index) = _pendingItems.Dequeue();

                if (index > ArchipelagoCampaignBehavior._savedItemIndex || ArchipelagoItems.AlwaysProcessItems().Contains(itemName))
                {
                    if (_itemCounts.ContainsKey(itemName))
                        _itemCounts[itemName]++;
                    else
                        _itemCounts[itemName] = 1;

                    LogToFile($"OnItemReceived: dispatching '{itemName}' (index={index}, count now={_itemCounts[itemName]})");
                    HandleItem(itemName);
                    LogToFile($"OnItemReceived: '{itemName}' handled successfully");
                }
            }
        }

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

        public static int GetCount(string itemName)
        {
            return _itemCounts.TryGetValue(itemName, out int count) ? count : 0;
        }

        public static void Reset()
        {
            _itemCounts.Clear();
        }

        private static void HandleItem(string itemName)
        {
            switch (itemName)
            {
                case ArchipelagoItems.DragonBannerPiece:
                    int count = GetCount(ArchipelagoItems.DragonBannerPiece);
                    InformationManager.DisplayMessage(new InformationMessage(
                        $"Received a Dragon Banner Piece! ({count}/{ArchipelagoClient.ServerData.DragonBannerPiecesRequired})",
                        Colors.Cyan
                    ));

                    if (count >= ArchipelagoClient.ServerData.DragonBannerPiecesRequired)
                    {
                        Main.APClient.SendGoalAchieved();
                    }
                    break;

                case ArchipelagoItems.SmallPouchOfDenars:
                    ItemGranter.GiveDenars(1000);
                    Notify(itemName, "1000 denars added to your party.");
                    break;

                case ArchipelagoItems.PouchOfDenars:
                    ItemGranter.GiveDenars(5000);
                    Notify(itemName, "5000 denars added to your party.");
                    break;

                case ArchipelagoItems.BagOfDenars:
                    ItemGranter.GiveDenars(10000);
                    Notify(itemName, "10000 denars added to your party.");
                    break;

                case ArchipelagoItems.Butter:
                    ItemGranter.GiveItem("butter", 50);
                    Notify(itemName, "50 butter added to your inventory.");
                    break;

                case ArchipelagoItems.Horse:
                    ItemGranter.GiveHorse("sumpter_horse", 1);
                    Notify(itemName, "A horse has joined your party.");
                    break;

                case ArchipelagoItems.SmallRenown:
                    ItemGranter.GiveRenown(25f);
                    Notify(itemName, "+25 renown.");
                    break;

                case ArchipelagoItems.Renown:
                    ItemGranter.GiveRenown(100f);
                    Notify(itemName, "+100 renown.");
                    break;

                case ArchipelagoItems.LargeRenown:
                    ItemGranter.GiveRenown(300f);
                    Notify(itemName, "+300 renown.");
                    break;

                case ArchipelagoItems.LordRelation:
                    ItemGranter.GiveRelationWithAllLords(5);
                    Notify(itemName, "+5 relation with all lords.");
                    break;
                case ArchipelagoItems.CraftingPlanBundle:
                    ItemGranter.CraftingPlanBundleItem.Grant(Campaign.Current.GetCampaignBehavior<ICraftingCampaignBehavior>());
                    Notify(itemName, "Received a Crafting Plan Bundle.");
                    break;
                default:
                    if (ArchipelagoItems.FocusItemToSkill.TryGetValue(itemName, out var focusSkill))
                    {
                        ItemGranter.GrantFocusPoint(focusSkill);
                        Notify(itemName, $"+1 Focus in {focusSkill.Name}.");
                    }
                    else if (ArchipelagoItems.AttributeItemToAttribute.TryGetValue(itemName, out var attribute))
                    {
                        ItemGranter.GrantAttributePoint(attribute);
                        Notify(itemName, $"+1 {attribute.Name}.");
                    }
                    else if (!ArchipelagoItems.AlwaysProcessItems().Contains(itemName))
                    {
                        InformationManager.DisplayMessage(new InformationMessage(
                        $"[AP] Received unhandled item: {itemName}",
                        Colors.Yellow));
                    }
                    break;
            }
        }

        private static void Notify(string itemName, string detail)
        {
            InformationManager.DisplayMessage(new InformationMessage(
                $"[AP] Received {itemName}: {detail}",
                Colors.Cyan
            ));
        }
    }

}