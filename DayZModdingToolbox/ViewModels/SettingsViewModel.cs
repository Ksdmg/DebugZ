using System.Collections.ObjectModel;
using System.Linq;

using DayZModdingToolbox.Common;
using DayZModdingToolbox.Data;

namespace DayZModdingToolbox.ViewModels
{
    public class SettingsViewModel : BindableBase
    {
        private static SettingsViewModel instance;
        private bool _filepatching;
        private ObservableCollection<ModData> _mods;
        private string _pathDayz;
        private string _pathDayzTools;
        private string _pathProfiles;
        private string _pathWorkdrive;
        private string _serverConfigPath;
        private int _serverPort;

        public SettingsViewModel()
        {
            _mods = new(Settings.Instance.Mods);
            _filepatching = Settings.Instance.Filepatching;
            _pathDayz = Settings.Instance.PathDayz;
            _pathDayzTools = Settings.Instance.PathDayzTools;
            _pathProfiles = Settings.Instance.PathProfiles;
            _pathWorkdrive = Settings.Instance.PathWorkdrive;
            _serverConfigPath = Settings.Instance.ServerConfigPath;
            _serverPort = Settings.Instance.ServerPort;

            Save = new(() =>
            {
                UpdateList!.Execute(new());
                Settings.Instance.Mods = this.Mods.ToList();
                Settings.Instance.Save();
            });

            UpdateList = new(() =>
            {
                ModsChanged();
            });

            BuildAllPbos = new(() =>
            {
                foreach (ModData mod in Mods)
                {
                    mod.BuildPbo = true;
                }
            });

            BuildNoPbos = new(() =>
            {
                foreach (ModData mod in Mods)
                {
                    mod.BuildPbo = false;
                }
            });

            _mods.CollectionChanged += this.ModsCollectionChanged;

            instance = this;
        }

        public static SettingsViewModel Instance
        {
            get
            {
                return instance;
            }
        }

        public int ActiveModsCount
        {
            get
            {
                return Mods.Count(x => x.IsActive);
            }
            set
            {
                RaisePropertyChanged(nameof(ActiveModsCount));
            }
        }

        public Command BuildAllPbos { get; }
        public Command BuildNoPbos { get; }

        public bool Filepatching
        {
            get
            {
                return _filepatching;
            }
            set
            {
                if (SetProperty(ref _filepatching, value))
                {
                    Settings.Instance.Filepatching = value;
                }
            }
        }

        public int FullyLinkedMods
        {
            get
            {
                return Mods.Count(x => x.HasDayzDirLink && x.HasWorkdriveLink);
            }
            set
            {
                RaisePropertyChanged(nameof(FullyLinkedMods));
            }
        }

        public ObservableCollection<ModData> Mods
        {
            get
            {
                return _mods;
            }
            set
            {
                if (SetProperty(ref _mods, value))
                {
                    Settings.Instance.Mods = _mods.ToList();
                }
            }
        }

        public string PathDayz
        {
            get
            {
                return _pathDayz;
            }
            set
            {
                if (SetProperty(ref _pathDayz, value))
                {
                    Settings.Instance.PathDayz = value;
                };
            }
        }

        public string PathDayzTools
        {
            get
            {
                return _pathDayzTools;
            }
            set
            {
                if (SetProperty(ref _pathDayzTools, value))
                {
                    Settings.Instance.PathDayzTools = value;
                }
            }
        }

        public string PathProfiles
        {
            get
            {
                return _pathProfiles;
            }
            set
            {
                if (SetProperty(ref _pathProfiles, value))
                {
                    Settings.Instance.PathProfiles = value;
                }
            }
        }

        public string PathWorkdrive
        {
            get
            {
                return _pathWorkdrive;
            }
            set
            {
                if (SetProperty(ref _pathWorkdrive, value))
                {
                    Settings.Instance.PathWorkdrive = value;
                }
            }
        }

        public Command Save { get; }

        public string ServerConfigPath
        {
            get
            {
                return _serverConfigPath;
            }
            set
            {
                if (SetProperty(ref _serverConfigPath, value))
                {
                    Settings.Instance.ServerConfigPath = value;
                }
            }
        }

        public int ServerPort
        {
            get
            {
                return _serverPort;
            }
            set
            {
                if (SetProperty(ref _serverPort, value))
                {
                    Settings.Instance.ServerPort = value;
                }
            }
        }

        public int TotalModsCount
        {
            get
            {
                return Mods.Count;
            }
            set { }
        }

        public Command UpdateList { get; }

        public static void ModsChanged()
        {
            if (instance != null)
            {
                foreach (ModData mod in instance.Mods)
                {
                    mod.Update();
                }
                instance?.RaisePropertyChanged(nameof(TotalModsCount));
                instance?.RaisePropertyChanged(nameof(ActiveModsCount));
                DebugViewModel.UpdateForeignBindings();
            }
        }

        private void ModsCollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            ModsChanged();
        }
    }
}