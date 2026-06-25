using MCM.Abstractions.Attributes;
using MCM.Abstractions.Attributes.v2;
using MCM.Abstractions.Base.Global;

namespace BannerlordArchipelago.Settings
{
    public class ArchipelagoSettings : AttributeGlobalSettings<ArchipelagoSettings>
    {
        public override string Id => "BannerlordArchipelago_v1";
        public override string DisplayName => "Bannerlord Archipelago";
        public override string FolderName => "BannerlordArchipelago";
        public override string FormatType => "json";

        private string _host = "archipelago.gg";
        private string _port = "38281";
        private string _slotName = "";
        private string _password = "";

        [SettingPropertyText("Host", RequireRestart = false, HintText = "Archipelago server hostname or IP address.")]
        [SettingPropertyGroup("Connection")]
        public string Host
        {
            get => _host;
            set { _host = value; OnPropertyChanged(); }
        }

        [SettingPropertyText("Port", RequireRestart = false, HintText = "Archipelago server port.")]
        [SettingPropertyGroup("Connection")]
        public string Port
        {
            get => _port;
            set { _port = value; OnPropertyChanged(); }
        }

        [SettingPropertyText("Slot Name", RequireRestart = false, HintText = "Your player slot name in the multiworld.")]
        [SettingPropertyGroup("Connection")]
        public string SlotName
        {
            get => _slotName;
            set { _slotName = value; OnPropertyChanged(); }
        }

        [SettingPropertyText("Password", RequireRestart = false, HintText = "Server password, if any. Leave blank if none.")]
        [SettingPropertyGroup("Connection")]
        public string Password
        {
            get => _password;
            set { _password = value; OnPropertyChanged(); }
        }

        public int PortAsInt => int.TryParse(Port, out var p) ? p : 38281;
    }
}