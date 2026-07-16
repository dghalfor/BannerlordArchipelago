using System.Collections.Generic;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Engine.GauntletUI;
using TaleWorlds.GauntletUI.Data;
using TaleWorlds.Library;
using TaleWorlds.ScreenSystem;

namespace BannerlordArchipelago.UI
{
    public class QuestLocationTooltipLayer : GauntletLayer
    {
        public QuestLocationTooltipVM DataSource { get; }

        private readonly GauntletMovieIdentifier _movieIdentifier;

        // Base signature is GauntletLayer(string name, int localOrder, bool shouldClear = false) —
        // name comes first, not localOrder.
        public QuestLocationTooltipLayer(int localOrder) : base("QuestLocationTooltipLayer", localOrder)
        {
            DataSource = new QuestLocationTooltipVM();
            _movieIdentifier = LoadMovie("QuestLocationTooltip", DataSource);
        }

        protected override void OnFinalize()
        {
            // GauntletLayer.OnFinalize asserts if a movie is still loaded at that point,
            // so release it explicitly first rather than relying on the base class to
            // clean it up implicitly.
            if (_movieIdentifier != null)
            {
                ReleaseMovie(_movieIdentifier);
            }
            base.OnFinalize();
        }
    }

    /// <summary>
    /// Owns the single tooltip layer instance. Show/Hide are cheap to call repeatedly —
    /// the layer is created once and reused, matching the pattern used by
    /// TierStatusGauntletLayer elsewhere in this project.
    /// </summary>
    public static class QuestLocationTooltipManager
    {
        private static QuestLocationTooltipLayer _layer;
        private static bool _listenerRegistered;

        public static void Show(string questTitle, IEnumerable<string> locationDisplayTexts)
        {
            EnsureListenerRegistered();
            var screen = ScreenManager.TopScreen;
            if (screen == null) return;

            if (_layer == null)
            {
                _layer = new QuestLocationTooltipLayer(9000);
                screen.AddLayer(_layer);
            }

            // FORCE ENGINE REGISTRATION: Give click priority right over the overlay grid
           // _layer.SetInputRestrictions(true, InputUsageMask.MouseButtons | InputUsageMask.MouseWheels);

            // Explicitly lock focus to this layer so clicks hit the button
            ScreenManager.TrySetFocus(_layer);

            _layer.DataSource.Populate(questTitle, locationDisplayTexts);
        }

        public static void Hide()
        {
            _layer?.DataSource.ExecuteClose();
        }

        // The layer is bound to a specific screen instance (the campaign map screen).
        // If it isn't torn down before that screen goes away — new game, load game,
        // or return to main menu — the reference goes stale. Reset() removes it from
        // the screen (which triggers OnFinalize -> ReleaseMovie) so the next Show()
        // call creates a fresh layer on whatever screen is current.
        public static void Reset()
        {
            if (_layer != null)
            {
                var screen = ScreenManager.TopScreen;
                // RemoveLayer doesn't check whether the layer was ever added before
                // running deactivate/finalize on it, so guard with HasLayer to avoid
                // finalizing a layer that never went through AddLayer (e.g. if Show()
                // bailed out early because TopScreen was null at the time).
                if (screen != null && screen.HasLayer(_layer))
                {
                    screen.RemoveLayer(_layer);
                }
                _layer = null;
            }
        }

        private static void EnsureListenerRegistered()
        {
            if (_listenerRegistered) return;
            _listenerRegistered = true;

            // Covers both "load a different save" and "return to main menu / new game"
            // transitions so the layer never survives across a campaign boundary.
            // NOTE: confirm these exact event names/signatures against your decompiled
            // CampaignEvents for your game version — they do shift between patches.
            CampaignEvents.OnGameLoadFinishedEvent.AddNonSerializedListener(
                typeof(QuestLocationTooltipManager), Reset);
            CampaignEvents.OnNewGameCreatedEvent.AddNonSerializedListener(
                typeof(QuestLocationTooltipManager), (CampaignGameStarter _) => Reset());
        }
    }
}