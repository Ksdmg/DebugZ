using DayZModdingToolbox.Common;

namespace DayZModdingToolbox
{
    public class MainWindowViewModel : BindableBase
    {
        private static MainWindowViewModel instance;
        private bool _settingsVisible;

        public MainWindowViewModel()
        {
            instance = this;
        }

        public static MainWindowViewModel Instance { get; }

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