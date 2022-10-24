using DayZModdingToolbox.Common;

namespace DayZModdingToolbox
{
    public class MainWindowViewModel : BindableBase
    {
        public MainWindowViewModel()
        {
        }


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
        private bool _settingsVisible;
    }
}
