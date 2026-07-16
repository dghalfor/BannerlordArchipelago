using TaleWorlds.Core;
using TaleWorlds.Engine.GauntletUI;
using TaleWorlds.GauntletUI;
using TaleWorlds.InputSystem;
using TaleWorlds.ScreenSystem;

namespace BannerlordArchipelago.Archipelago.UI
{
    public class TierStatusGauntletLayer
    {
        private GauntletLayer _layer;
        private TierStatusScreenVM _dataSource;

        public bool IsOpen => _layer != null;

        public void Open()
        {
            if (IsOpen) return;

            _dataSource = new TierStatusScreenVM();
            _dataSource.OnClose += Close;

            _layer = new GauntletLayer("GauntletLayer", 1000, false);
            _layer.LoadMovie("TierStatusScreen", _dataSource);        // matches XML filename, no extension
            _layer.InputRestrictions.SetInputRestrictions();          // captures mouse for the button click

            ScreenManager.TopScreen.AddLayer(_layer);
            ScreenManager.TrySetFocus(_layer);
        }

        public void Close()
        {
            if (!IsOpen) return;

            ScreenManager.TopScreen.RemoveLayer(_layer);
            _layer = null;
            _dataSource.OnClose -= Close;
            _dataSource = null;
        }
    }
}