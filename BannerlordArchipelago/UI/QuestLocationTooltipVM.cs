using System.Collections.Generic;
using TaleWorlds.Library;

namespace BannerlordArchipelago.UI
{
    public class QuestLocationEntryVM : ViewModel
    {
        private string _displayText;

        public QuestLocationEntryVM(string displayText)
        {
            _displayText = displayText;
        }

        [DataSourceProperty]
        public string DisplayText
        {
            get => _displayText;
            set
            {
                if (value != _displayText)
                {
                    _displayText = value;
                    OnPropertyChangedWithValue(value, nameof(DisplayText));
                }
            }
        }
    }

    public class QuestLocationTooltipVM : ViewModel
    {
        private MBBindingList<QuestLocationEntryVM> _locations = new MBBindingList<QuestLocationEntryVM>();
        private string _questTitle;
        private bool _isVisible;
        private bool _isExpanded = true;
        private string _toggleButtonBrush = "Popup.CloseButton"; // Default fallback

        [DataSourceProperty]
        public MBBindingList<QuestLocationEntryVM> Locations
        {
            get => _locations;
            set { if (value != _locations) { _locations = value; OnPropertyChangedWithValue(value, nameof(Locations)); } }
        }

        [DataSourceProperty]
        public string QuestTitle
        {
            get => _questTitle;
            set { if (value != _questTitle) { _questTitle = value; OnPropertyChangedWithValue(value, nameof(QuestTitle)); } }
        }

        [DataSourceProperty]
        public bool IsVisible
        {
            get => _isVisible;
            set { if (value != _isVisible) { _isVisible = value; OnPropertyChangedWithValue(value, nameof(IsVisible)); } }
        }

        [DataSourceProperty]
        public bool IsExpanded
        {
            get => _isExpanded;
            set
            {
                if (value != _isExpanded)
                {
                    _isExpanded = value;
                    OnPropertyChangedWithValue(value, nameof(IsExpanded));
                    UpdateBrush(); // Refresh the icon whenever state changes
                }
            }
        }

        // NEW PROPERTY: Safely feeds the plain text brush name to the XML
        [DataSourceProperty]
        public string ToggleButtonBrush
        {
            get => _toggleButtonBrush;
            set { if (value != _toggleButtonBrush) { _toggleButtonBrush = value; OnPropertyChangedWithValue(value, nameof(ToggleButtonBrush)); } }
        }

        public void Populate(string questTitle, IEnumerable<string> locationDisplayTexts)
        {
            QuestTitle = questTitle;
            Locations.Clear();
            foreach (var text in locationDisplayTexts) Locations.Add(new QuestLocationEntryVM(text));
            IsVisible = true;
            IsExpanded = true;
            UpdateBrush();
        }

        public void ExecuteClose()
        {
            // Toggle the value
            IsExpanded = !IsExpanded;

            // Optional: Add a debug line to check if the game registers the click in your console
            InformationManager.DisplayMessage(new InformationMessage($"Quest list toggle clicked! Current state: {IsExpanded}"));
        }

        // Changes the brush. Swap these string names with custom mod textures if needed!
        private void UpdateBrush()
        {
            ToggleButtonBrush = IsExpanded ? "Popup.CloseButton" : "ResetButton";
        }
    }
}