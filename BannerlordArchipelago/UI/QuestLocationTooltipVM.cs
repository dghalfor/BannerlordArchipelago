using Bannerlord.UIExtenderEx.Attributes;
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
        private string _rawQuestTitle = "";
        private string _displayedTitle = "";
        private bool _isVisible;
        private bool _isExpanded = true;

        [DataSourceProperty]
        public MBBindingList<QuestLocationEntryVM> Locations
        {
            get => _locations;
            set { if (value != _locations) { _locations = value; OnPropertyChangedWithValue(value, nameof(Locations)); } }
        }

        [DataSourceProperty]
        public string QuestTitle
        {
            get => _displayedTitle;
            set { if (value != _displayedTitle) { _displayedTitle = value; OnPropertyChangedWithValue(value, nameof(QuestTitle)); } }
        }

        [DataSourceProperty]
        public bool IsVisible
        {
            get => _isVisible;
            set { if (value != _isVisible) { _isVisible = value; OnPropertyChangedWithValue(value, nameof(IsVisible)); } }
        }

        public void Populate(string questTitle, IEnumerable<string> locationDisplayTexts)
        {
            _rawQuestTitle = questTitle;
            Locations.Clear();
            foreach (var text in locationDisplayTexts) Locations.Add(new QuestLocationEntryVM(text));

            IsVisible = true;
            IsExpanded = true;
            QuestTitle = _rawQuestTitle;
        }

        [DataSourceProperty]
        public bool IsExpanded
        {
            get => _isExpanded;
            set { if (value != _isExpanded) { _isExpanded = value; OnPropertyChangedWithValue(value, nameof(IsExpanded)); } }
        }

        public void OnHoverBegin()
        {
            QuestLocationTooltipManager.SetButtonInputRestricted(true);
        }

        public void OnHoverEnd()
        {
            QuestLocationTooltipManager.SetButtonInputRestricted(false);
        }
        public void ExecuteClose()
        {
            IsExpanded = !IsExpanded;

            OnPropertyChanged(nameof(IsExpanded));
        }
    }
}