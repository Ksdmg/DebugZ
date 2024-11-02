using System.IO;
using System.Runtime.CompilerServices;

using DayZModdingToolbox.Common;

namespace DayZModdingToolbox.Data
{
    public class ModData : BindableBase
    {
        private bool _clientmod;
        private bool _hasDayzDirLink;
        private bool _hasWorkdriveLink;
        private bool _isActive = true;
        private string _modName = string.Empty;
        private string _modPath = string.Empty;
        private bool _pathValid;

        private bool buildPbo;

        private string modpack = string.Empty;

        public ModData()
        {
        }

        public bool BuildPbo
        {
            get
            {
                return buildPbo;
            }
            set
            {
                SetProperty(ref buildPbo, value);
            }
        }

        public bool Clientmod
        {
            get
            {
                return _clientmod;
            }
            set
            {
                SetProperty(ref _clientmod, value);
            }
        }

        public bool HasDayzDirLink
        {
            get
            {
                return _hasDayzDirLink;
            }
            set
            {
                SetProperty(ref _hasDayzDirLink, value);
            }
        }

        public bool HasWorkdriveLink
        {
            get
            {
                return _hasWorkdriveLink;
            }
            set
            {
                SetProperty(ref _hasWorkdriveLink, value);
            }
        }

        public bool IsActive
        {
            get
            {
                return _isActive;
            }
            set
            {
                SetProperty(ref _isActive, value);
            }
        }

        public string ModName
        {
            get
            {
                return _modName;
            }
            set
            {
                SetProperty(ref _modName, value);
            }
        }

        public string Modpack
        {
            get
            {
                return modpack;
            }
            set
            {
                SetProperty(ref modpack, value);
            }
        }

        public string ModPath
        {
            get
            {
                return _modPath;
            }
            set
            {
                SetProperty(ref _modPath, value);
            }
        }

        public bool PathValid
        {
            get
            {
                return _pathValid;
            }
            set
            {
                SetProperty(ref _pathValid, value);
            }
        }

        public string GetDayzDirLinkPath()
        {
            return Path.Combine(Settings.Instance.PathDayz, GetModpackRelativePath(), "addons", ModName);
        }

        public string GetModDir()
        {
            return Path.Combine(Settings.Instance.PathWorkdrive, "Mods", GetModpackRelativePath(), ModName);
        }

        public string GetModpackDir()
        {
            return Path.Combine(Settings.Instance.PathWorkdrive, "Mods", Modpack);
        }

        public string GetModpackFullPath()
        {
            return Path.Combine(Settings.Instance.PathWorkdrive, "Mods", GetModpackRelativePath());
        }

        public string GetModpackRelativePath()
        {
            return string.IsNullOrWhiteSpace(modpack) ? string.Empty : Modpack;
        }

        public string GetPboDir()
        {
            if (string.IsNullOrWhiteSpace(modpack))
            {
                return Path.Combine(GetModDir(), "addons");
            }
            return Path.Combine(GetModDir(), "addons");
        }

        public string GetWorkdriveLinkPath()
        {
            string modpack = string.Empty;
            if (!string.IsNullOrWhiteSpace(GetModpackRelativePath()))
            {
                modpack = Path.Combine(GetModpackRelativePath(), "addons");
            }
            return Path.Combine(Settings.Instance.PathWorkdrive, modpack, ModName);
        }

        public void Update()
        {
            PathValid = Directory.Exists(ModPath);

            if (!PathValid) return;

            if (ModName == null || ModName == string.Empty)
            {
                ModName = new DirectoryInfo(ModPath).Name;
            }
            HasWorkdriveLink = Directory.Exists(GetWorkdriveLinkPath());
            HasDayzDirLink = Directory.Exists(GetDayzDirLinkPath());
        }

        protected override bool SetProperty<T>(ref T storage, T value, [CallerMemberName] string? propertyName = null)
        {
            var retVal = base.SetProperty(ref storage, value, propertyName);

            if (propertyName == nameof(ModPath) || propertyName == nameof(ModName))
            {
                if (Settings.Lazy.IsValueCreated)
                {
                    Update();
                }
            }
            //SettingsViewModel.ModsChanged();
            return retVal;
        }
    }
}