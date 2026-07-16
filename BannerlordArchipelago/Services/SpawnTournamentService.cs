using System;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.Conversation;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.CampaignSystem.TournamentGames;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace BannerlordArchipelago.CampaignBehaviors
{
    public class APTournamentDialogBehavior : CampaignBehaviorBase
    {
        private const int TournamentCost = 10000;

        public override void RegisterEvents()
        {
            CampaignEvents.OnSessionLaunchedEvent.AddNonSerializedListener(this, OnSessionLaunched);
        }

        public override void SyncData(IDataStore dataStore) { }

        private void OnSessionLaunched(CampaignGameStarter starter)
        {
            starter.AddPlayerLine(
                "ap_request_tournament",
                "arena_master_talk",
                "ap_request_tournament_response",
                $"I'd like to pay {TournamentCost} denars to have a tournament arranged here as soon as possible.",
                ConversationArenaMasterNoTournamentOnCondition,
                null,
                100,
                ConversationArenaMasterAffordCheckOnCondition,
                null);

            starter.AddDialogLine(
                "ap_request_tournament_master_response",
                "ap_request_tournament_response",
                "ap_request_tournament_confirm",
                "{=!}Consider it done. Come back shortly and the lots will be drawn.",
                null,
                null,
                100,
                null);

            starter.AddPlayerLine(
                "ap_request_tournament_confirm_yes",
                "ap_request_tournament_confirm",
                "close_window",
                "Here you go.",
                null,
                ConsequenceStartTournament,
                100,
                null,
                null);

            starter.AddPlayerLine(
                "ap_request_tournament_confirm_no",
                "ap_request_tournament_confirm",
                "close_window",
                "Actually, never mind.",
                null,
                null,
                100,
                null,
                null);
        }

        private bool ConversationArenaMasterNoTournamentOnCondition()
        {
            var conversationChar = CharacterObject.OneToOneConversationCharacter;
            var settlement = Settlement.CurrentSettlement;

            if (conversationChar?.Occupation != Occupation.ArenaMaster)
                return false;

            if (settlement == null || !settlement.IsTown)
                return false;

            return Campaign.Current.TournamentManager.GetTournamentGame(settlement.Town) == null;
        }

        private bool ConversationArenaMasterAffordCheckOnCondition(out TextObject explanation)
        {
            if (Hero.MainHero.Gold < TournamentCost)
            {
                explanation = new TextObject($"You need {TournamentCost} denars.");
                return false;
            }

            explanation = null;
            return true;
        }

        private void ConsequenceStartTournament()
        {
            var town = Settlement.CurrentSettlement.Town;
            var tournamentManager = Campaign.Current.TournamentManager;

            // Defensive re-check in case state changed mid-conversation.
            if (tournamentManager.GetTournamentGame(town) != null)
            {
                InformationManager.DisplayMessage(new InformationMessage("A tournament is already planned here."));
                return;
            }

            var tournament = Campaign.Current.Models.TournamentModel.CreateTournament(town);
            tournamentManager.AddTournament(tournament);

            GiveGoldAction.ApplyBetweenCharacters(Hero.MainHero, null, TournamentCost, false);

            InformationManager.DisplayMessage(new InformationMessage($"A tournament will begin shortly in {Settlement.CurrentSettlement.Name}."));
        }
    }
}