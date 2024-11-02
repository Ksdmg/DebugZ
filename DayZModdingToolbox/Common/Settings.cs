using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

using DayZModdingToolbox.Data;

namespace DayZModdingToolbox.Common
{
    [JsonSerializable(typeof(Settings))]
    public class Settings : AppConfiguration<Settings>, IConfiguration
    {
        private Settings()
            : base(Path.Combine(Directory.GetCurrentDirectory(), "Settings.json"))
        {
        }

        public bool Filepatching { get; set; } = true;
        public List<ModData> Mods { get; set; } = new();
        public string PathDayz { get; set; } = @"E:\SteamLibrary\steamapps\common\DayZ";
        public string PathDayzTools { get; set; } = @"E:\SteamLibrary\steamapps\common\DayZ Tools";

        /// <summary>
        /// Gets or sets the profiles path. Startparameter -profiles If not set, defaults to %localappdata%\DayZ
        /// </summary>
        public string PathProfiles { get; set; } = @"%localappdata%\DayZ";

        public string PathWorkdrive { get; set; } = @"P:\";
        public string ServerConfigPath { get; set; } = @"serverDZ.cfg";
        public int ServerPort { get; set; } = 2302;
    }
}