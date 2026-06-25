using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

        // Skill XP "scrolls"
        public const string OneHandedXp = "One Handed Scroll";
        public const string TwoHandedXp = "Two Handed Scroll";
        public const string PolearmXp = "Polearm Scroll";
        public const string BowXp = "Bow Scroll";
        public const string CrossbowXp = "Crossbow Scroll";
        public const string ThrowingXp = "Throwing Scroll";
        public const string RidingXp = "Riding Scroll";
        public const string AthleticsXp = "Athletics Scroll";
        public const string CraftingXp = "Crafting Scroll";
        public const string TacticsXp = "Tactics Scroll";
        public const string ScoutingXp = "Scouting Scroll";
        public const string RogueryXp = "Roguery Scroll";
        public const string CharmXp = "Charm Scroll";
        public const string LeadershipXp = "Leadership Scroll";
        public const string TradeXp = "Trade Scroll";
        public const string StewardXp = "Steward Scroll";
        public const string MedicineXp = "Medicine Scroll";
        public const string EngineeringXp = "Engineering Scroll";

        public static bool SuppressCharmGain = false;
    }

    public static class ReceivedItemsTracker
    {
        private static readonly Dictionary<string, int> _itemCounts = new Dictionary<string, int>();

        public static void OnItemReceived(string itemName, int index)
        {
            if (index > ArchipelagoCampaignBehavior._savedItemIndex || itemName == ArchipelagoItems.DragonBannerPiece)
            {
                if (_itemCounts.ContainsKey(itemName))
                    _itemCounts[itemName]++;
                else
                    _itemCounts[itemName] = 1;
            

                HandleItem(itemName);
            }
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

                case ArchipelagoItems.OneHandedXp:
                    ItemGranter.GiveSkillXp(DefaultSkills.OneHanded, 5000f);
                    Notify(itemName, "+5000 One Handed XP.");
                    break;
                case ArchipelagoItems.TwoHandedXp:
                    ItemGranter.GiveSkillXp(DefaultSkills.TwoHanded, 5000f);
                    Notify(itemName, "+5000 Two Handed XP.");
                    break;
                case ArchipelagoItems.PolearmXp:
                    ItemGranter.GiveSkillXp(DefaultSkills.Polearm, 5000f);
                    Notify(itemName, "+5000 Polearm XP.");
                    break;
                case ArchipelagoItems.BowXp:
                    ItemGranter.GiveSkillXp(DefaultSkills.Bow, 5000f);
                    Notify(itemName, "+5000 Bow XP.");
                    break;
                case ArchipelagoItems.CrossbowXp:
                    ItemGranter.GiveSkillXp(DefaultSkills.Crossbow, 5000f);
                    Notify(itemName, "+5000 Crossbow XP.");
                    break;
                case ArchipelagoItems.ThrowingXp:
                    ItemGranter.GiveSkillXp(DefaultSkills.Throwing, 5000f);
                    Notify(itemName, "+5000 Throwing XP.");
                    break;
                case ArchipelagoItems.RidingXp:
                    ItemGranter.GiveSkillXp(DefaultSkills.Riding, 2500f);
                    Notify(itemName, "+2500 Riding XP.");
                    break;
                case ArchipelagoItems.AthleticsXp:
                    ItemGranter.GiveSkillXp(DefaultSkills.Athletics, 2500f);
                    Notify(itemName, "+2500 Athletics XP.");
                    break;
                case ArchipelagoItems.CraftingXp:
                    ItemGranter.GiveSkillXp(DefaultSkills.Crafting, 2500f);
                    Notify(itemName, "+2500 Crafting XP.");
                    break;
                case ArchipelagoItems.TacticsXp:
                    ItemGranter.GiveSkillXp(DefaultSkills.Tactics, 2500f);
                    Notify(itemName, "+2500 Tactics XP.");
                    break;
                case ArchipelagoItems.ScoutingXp:
                    ItemGranter.GiveSkillXp(DefaultSkills.Scouting, 2500f);
                    Notify(itemName, "+2500 Scouting XP.");
                    break;
                case ArchipelagoItems.RogueryXp:
                    ItemGranter.GiveSkillXp(DefaultSkills.Roguery, 2500f);
                    Notify(itemName, "+2500 Roguery XP.");
                    break;
                case ArchipelagoItems.CharmXp:
                    ItemGranter.GiveSkillXp(DefaultSkills.Charm, 2500f);
                    Notify(itemName, "+2500 Charm XP.");
                    break;
                case ArchipelagoItems.LeadershipXp:
                    ItemGranter.GiveSkillXp(DefaultSkills.Leadership, 2500f);
                    Notify(itemName, "+2500 Leadership XP.");
                    break;
                case ArchipelagoItems.TradeXp:
                    ItemGranter.GiveSkillXp(DefaultSkills.Trade, 2500f);
                    Notify(itemName, "+2500 Trade XP.");
                    break;
                case ArchipelagoItems.StewardXp:
                    ItemGranter.GiveSkillXp(DefaultSkills.Steward, 2500f);
                    Notify(itemName, "+2500 Steward XP.");
                    break;
                case ArchipelagoItems.MedicineXp:
                    ItemGranter.GiveSkillXp(DefaultSkills.Medicine, 2500f);
                    Notify(itemName, "+2500 Medicine XP.");
                    break;
                case ArchipelagoItems.EngineeringXp:
                    ItemGranter.GiveSkillXp(DefaultSkills.Engineering, 2500f);
                    Notify(itemName, "+2500 Engineering XP.");
                    break;
                case ArchipelagoItems.LordRelation:
                    ItemGranter.GiveRelationWithAllLords(5);
                    Notify(itemName, "+5 relation with all lords.");
                    break;

                default:
                    InformationManager.DisplayMessage(new InformationMessage(
                        $"[AP] Received unhandled item: {itemName}",
                        Colors.Yellow
                    ));
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
