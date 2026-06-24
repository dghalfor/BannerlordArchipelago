using StoryMode.StoryModePhases;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BannerlordArchipelago.Data
{
    public static class DataDicts
    {
        public static readonly Dictionary<string, string> GameNameToAPLocation = new Dictionary<string, string>
        {
            { TutorialQuestPhase.TravelToVillageStarted.ToString(),"Tutorial Reached Village" },
            { TutorialQuestPhase.TalkToTheHeadmanStarted.ToString(),"Tutorial Talk to Headman" },
            { TutorialQuestPhase.RecruitAndPurchaseStarted.ToString(),"Tutorial Recruited Troops" },
            { TutorialQuestPhase.LocateAndRescueTravellerStarted.ToString(),"Tutorial Rescued Traveller" },
            { TutorialQuestPhase.FindHideoutStarted.ToString(),"Tutorial Found Hideout" },
            { TutorialQuestPhase.Finalized.ToString(),"Tutorial Finished Quest" },
            { "VillagersInNeed_OnHeadmanRescued", "Stealth Quest Rescued Headman" },
            //Tournaments
            { "Tournament Win - Askar", "Tournament Win - Askar" },
            { "Tournament Win - Hubyar", "Tournament Win - Hubyar" },
            { "Tournament Win - Husn Fulq", "Tournament Win - Husn Fulq" },
            { "Tournament Win - Iyakis", "Tournament Win - Iyakis" },
            { "Tournament Win - Qasira", "Tournament Win - Qasira" },
            { "Tournament Win - Quyaz", "Tournament Win - Quyaz" },
            { "Tournament Win - Razih", "Tournament Win - Razih" },
            { "Tournament Win - Sanala", "Tournament Win - Sanala" },
            { "Tournament Win - Car Banseth", "Tournament Win - Car Banseth" },
            { "Tournament Win - Dunglanys", "Tournament Win - Dunglanys" },
            { "Tournament Win - Marunath", "Tournament Win - Marunath" },
            { "Tournament Win - Pen Cannoc", "Tournament Win - Pen Cannoc" },
            { "Tournament Win - Seonon", "Tournament Win - Seonon" },
            { "Tournament Win - Akkalat", "Tournament Win - Akkalat" },
            { "Tournament Win - Baltakhand", "Tournament Win - Baltakhand" },
            { "Tournament Win - Chaikand", "Tournament Win - Chaikand" },
            { "Tournament Win - Makeb", "Tournament Win - Makeb" },
            { "Tournament Win - Odokh", "Tournament Win - Odokh" },
            { "Tournament Win - Ortongard", "Tournament Win - Ortongard" },
            { "Tournament Win - Balgard", "Tournament Win - Balgard" },
            { "Tournament Win - Omor", "Tournament Win - Omor" },
            { "Tournament Win - Revyl", "Tournament Win - Revyl" },
            { "Tournament Win - Sibir", "Tournament Win - Sibir" },
            { "Tournament Win - Tyal", "Tournament Win - Tyal" },
            { "Tournament Win - Varcheg", "Tournament Win - Varcheg" },
            { "Tournament Win - Varnovapol", "Tournament Win - Varnovapol" },
            { "Tournament Win - Charas", "Tournament Win - Charas" },
            { "Tournament Win - Galend", "Tournament Win - Galend" },
            { "Tournament Win - Jaculan", "Tournament Win - Jaculan" },
            { "Tournament Win - Ocs Hall", "Tournament Win - Ocs Hall" },
            { "Tournament Win - Ostican", "Tournament Win - Ostican" },
            { "Tournament Win - Pravend", "Tournament Win - Pravend" },
            { "Tournament Win - Rovalt", "Tournament Win - Rovalt" },
            { "Tournament Win - Sargot", "Tournament Win - Sargot" },
            { "Tournament Win - Amprela", "Tournament Win - Amprela" },
            { "Tournament Win - Argoron", "Tournament Win - Argoron" },
            { "Tournament Win - Diathma", "Tournament Win - Diathma" },
            { "Tournament Win - Epicrotea", "Tournament Win - Epicrotea" },
            { "Tournament Win - Myzea", "Tournament Win - Myzea" },
            { "Tournament Win - Saneopa", "Tournament Win - Saneopa" },
            { "Tournament Win - Danustica", "Tournament Win - Danustica" },
            { "Tournament Win - Lycaron", "Tournament Win - Lycaron" },
            { "Tournament Win - Onira", "Tournament Win - Onira" },
            { "Tournament Win - Phycaon", "Tournament Win - Phycaon" },
            { "Tournament Win - Poros", "Tournament Win - Poros" },
            { "Tournament Win - Syronea", "Tournament Win - Syronea" },
            { "Tournament Win - Vostrum", "Tournament Win - Vostrum" },
            { "Tournament Win - Amitatys", "Tournament Win - Amitatys" },
            { "Tournament Win - Jalmarys", "Tournament Win - Jalmarys" },
            { "Tournament Win - Lageta", "Tournament Win - Lageta" },
            { "Tournament Win - Ortysia", "Tournament Win - Ortysia" },
            { "Tournament Win - Rhotae", "Tournament Win - Rhotae" },
            { "Tournament Win - Zeonica", "Tournament Win - Zeonica" },
            //Quests
            { "ArtisanCantSellProductsAtAFairPriceIssueQuest", "Artisan Can't Sell Products Quest" },
            { "ArmyNeedsSuppliesIssueQuest", "Army Needs Supplies Quest" },
            { "ArtisanOverpricedGoodsIssueQuest", "Artisan Overpriced Goods Quest" },
            { "BettingFraudQuest", "Betting Fraud Quest" },
            { "CapturedByBountyHuntersIssueQuest", "Captured by Bounty Hunters Quest" },
            { "CaravanAmbushIssueQuest","Caravan Ambush Quest" },
            { "EscortMerchantCaravanIssueQuest","Escort Merchant Caravan Quest" },
            { "ExtortionByDesertersIssueQuest","Extortion by Deserters Quest" },
            { "GangLeaderNeedsRecruitsIssueQuest","Gang Leader Needs Recruits Quest" },
            { "GangLeaderNeedsSpecialWeaponsIssueQuest","Gang Leader Needs Special Weapons Quest" },
            { "GangLeaderNeedsToOffloadStolenGoodsIssueQuest","Gang Leader Needs to Offload Stolen Goods Quest" },
            { "GangLeaderNeedsWeaponsIssueQuest","Gang Leader Needs Weapons Quest" },
            { "HeadmanNeedsGrainIssueQuest","Headman Needs Grain Quest" },
            { "HeadmanNeedsToDeliverAHerdIssueQuest","Headman Needs to Deliver a Herd Quest" },
            { "HeadmanVillageNeedsDraughtAnimalsIssueQuest","Headman Village Needs Draught Animals Quest" },
            { "LadysKnightOutIssueQuest","Lady's Knight Out Quest" },
            { "LandLordCompanyOfTroubleIssueQuest","Land Lord Company of Trouble Quest" },
            { "LandlordNeedsAccessToVillageCommonsIssueQuest","Landlord Needs Access to Village Commons Quest" },
            { "LandLordNeedsManualLaborersIssueQuest","Land Lord Needs Manual Laborers Quest" },
            { "LandLordTheArtOfTheTradeIssueQuest","Land Lord The Art of the Trade Quest" },
            { "LandlordTrainingForRetainersIssueQuest","Landlord Training for Retainers Quest" },
            { "LesserNobleRevoltIssueQuest","Lesser Noble Revolt Quest" },
            { "LordNeedsGarrisonTroopsIssueQuest","Lord Needs Garrison Troops Quest" },
            { "LordNeedsHorsesIssueQuest","Lord Needs Horses Quest" },
            { "LordsNeedsTutorIssueQuest","Lord Needs Tutor Quest" },
            { "LordWantsRivalCapturedIssueQuest","Lord Wants Rival Captured Quest" },
            { "MerchantArmyOfPoachersIssueQuest","Merchant Army of Poachers Quest" },
            { "MerchantNeedsHelpWithOutlawsIssueQuest","Merchant Needs Help with Outlaws Quest" },
            { "NearbyBanditBaseIssueQuest","Nearby Bandit Base Quest" },
            { "RaidAnEnemyTerritoryQuest","Raid an Enemy Territory Quest" },
            { "RevenueFarmingIssueQuest","Revenue Farming Quest" },
            { "RivalGangMovingInIssueQuest", "Rival Gang Moving In Quest" },
            { "ScoutEnemyGarrisonsQuest","Scout Enemy Garrisons Quest" },
            { "SmugglersIssueQuest","Smugglers Quest" },
            { "TheConquestOfSettlementIssueQuest","The Conquest of Settlement Quest" },
            { "VillageNeedsCraftingMaterialsIssueQuest","Village Needs Crafting Materials Quest" },
            { "VillageNeedsToolsIssueQuest","Village Needs Tools Quest" },
        };
        public static readonly Dictionary<string, string> QuestTypeToLocation = new Dictionary<string, string>()
        {
            
        };

        public static readonly Dictionary<string, string> ItemNameToGameObject = new Dictionary<string, string>
        {
        };
    }
}
