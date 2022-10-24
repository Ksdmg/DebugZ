using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DayZModdingToolbox.Common
{
    /// <summary>
    /// Interface that all AppConfigurations should implement.
    /// </summary>
    public interface IConfiguration
    {
        /// <summary>
        /// Gets singleton instance.
        /// </summary>
        public static object Instance { get; }

        /// <summary>
        /// Gets a string to use during encryption of settings.
        /// </summary>
        public string EncryptionKey { get; }

        /// <summary>
        /// Gets the filepath for storing the config file.
        /// </summary>
        public string FileSavePath { get; }
    }
}
