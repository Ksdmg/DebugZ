using DayZModdingToolbox.Common;

namespace DayZModdingToolbox
{
    public class MainWindowViewModel : BindableBase
    {
        private bool _settingsVisible;

        public bool SettingsVisible
        {
            get
            {
                return _settingsVisible;
            }
            set
            {
                _ = SetProperty(ref _settingsVisible, value);
            }
        }
    }
}