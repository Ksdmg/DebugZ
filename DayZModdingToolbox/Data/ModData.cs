using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;

using DayZModdingToolbox.Common;

using Newtonsoft.Json.Linq;

namespace DayZModdingToolbox.Data
{
    public class ModData : BindableBase
    {
        private bool _clientmod;
        private bool _hasDayzDirLink;
        private bool _hasWorkdriveLink;
        private bool _isActive = true;
        private string _modName;
        private string _modPath;
        private bool _pathValid;

        public ModData()
        {
            _modPath = string.Empty;
            _modName = string.Empty;
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
            return Path.Combine(Settings.Instance.PathDayz, ModName);
        }

        public string GetPboDir()
        {
            return Path.Combine(Settings.Instance.PathWorkdrive, "Mods", "@" + ModName, "addons");
        }

        public string GetWorkdriveLinkPath()
        {
            return Path.Combine(Settings.Instance.PathWorkdrive, ModName);
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

        protected override bool SetProperty<T>(ref T storage, T value, [CallerMemberName] string propertyName = null)
        {
            var retVal = base.SetProperty(ref storage, value, propertyName);

            if (propertyName == nameof(ModPath) || propertyName == nameof(ModName))
            {
                if (Settings.Lazy.IsValueCreated)
                {
                    Update();
                }
            }

            return retVal;
        }
    }
}