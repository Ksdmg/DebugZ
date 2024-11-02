namespace DayZModdingToolbox.Common
{
    using System;
    using System.IO;

    using Newtonsoft.Json;

    /// <summary>
    /// Base class for implementing App Configurations and simple save method to the disk in JSON format.
    /// </summary>
    /// <typeparam name="T">Custom config class to save.</typeparam>
    public abstract class AppConfiguration<T>
        where T : AppConfiguration<T>, IConfiguration
    {
        internal static readonly Lazy<T> Lazy = new(
        () =>
        {
            // Create new instance from type T with private constructor (from inherited type)
            T retVal = (T)Activator.CreateInstance(typeof(T), true)!;
            Load(retVal);
            return retVal;
        });

        /// <summary>
        /// Initializes a new instance of the <see cref="AppConfiguration{T}"/> class.
        /// </summary>
        /// <param name="filepath">The path where the config file should be saved to.</param>
        /// <param name="encryptionKey">
        /// Key used for encrypting properties that should not be stored in clear-text.
        /// </param>
        protected AppConfiguration(string filepath, string encryptionKey = "GVhenBl2gS7GKhHQ6k2fIbj0yRuN4tQhClp2XHmFtiY=")
        {
            if (string.IsNullOrEmpty(filepath))
            {
                throw new ArgumentException($"'{nameof(filepath)}' cannot be null or empty.", nameof(filepath));
            }

            if (string.IsNullOrEmpty(encryptionKey))
            {
                throw new ArgumentException($"'{nameof(encryptionKey)}' cannot be null or empty.", nameof(encryptionKey));
            }

            // Check if a fully qualified path has been passed
            if (!Path.IsPathFullyQualified(filepath))
            {
                // If path is not fully qualified, make it fully qualified
                string fileExtension = Path.GetExtension(filepath).ToLowerInvariant();
                if (fileExtension != ".json")
                {
                    filepath += ".json";
                }

                filepath = Path.GetFileName(filepath);
                filepath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "DayzModdingToolbox", filepath);
            }

            this.EncryptionKey = encryptionKey;
            this.FileSavePath = filepath;
        }

        /// <summary>
        /// Gets the instance of the current config object.
        /// </summary>
        public static T Instance => Lazy.Value;

        /// <summary>
        /// Gets the key to use for encrypting properties.
        /// </summary>
        [JsonIgnore]
        public string EncryptionKey { get; }

        /// <summary>
        /// Gets the absolute path to save the config file to.
        /// </summary>
        [JsonIgnore]
        public string FileSavePath { get; }

        /// <summary>
        /// Saves the current state of the object to disk.
        /// </summary>
        public void Save()
        {
            if (!Directory.Exists(Path.GetDirectoryName(this.FileSavePath)))
            {
                _ = Directory.CreateDirectory(Path.GetDirectoryName(this.FileSavePath)!);
            }

            File.WriteAllText(this.FileSavePath, JsonConvert.SerializeObject(Lazy.Value, Formatting.Indented));
        }

        /// <summary>
        /// Loads a configuration file from the specified location and initializes a shared instance.
        /// </summary>
        /// <param name="config">Newly constructed instance of a config class.</param>
        private static void Load(T config)
        {
            // Check if a file already exists
            if (File.Exists(config.FileSavePath))
            {
                // Load Config from file
                JsonConvert.PopulateObject(File.ReadAllText(config.FileSavePath), config);
                return;
            }

            // Otherwise create a new instance and save the file
            else
            {
                File.WriteAllText(config.FileSavePath, JsonConvert.SerializeObject(config, Formatting.Indented));
            }
        }
    }
}