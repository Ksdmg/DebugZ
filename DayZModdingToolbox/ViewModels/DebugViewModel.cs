using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;

using DayZModdingToolbox.Common;
using DayZModdingToolbox.Data;

namespace DayZModdingToolbox.ViewModels
{
    public class DebugViewModel : BindableBase
    {
        private static DebugViewModel instance;

        public DebugViewModel()
        {
            SetupModLinks = new(() =>
            {
                foreach (ModData mod in Settings.Instance.Mods)
                {
                    mod.Update();

                    // Check and create links
                    if (mod.IsActive)
                    {
                        if (!mod.HasDayzDirLink)
                        {
                            _ = Directory.CreateSymbolicLink(mod.GetDayzDirLinkPath(), mod.ModPath);
                        }
                        if (!mod.HasWorkdriveLink)
                        {
                            _ = Directory.CreateSymbolicLink(mod.GetWorkdriveLinkPath(), mod.ModPath);
                        }
                    }

                    // Remove Links
                    else
                    {
                        if (mod.HasDayzDirLink)
                        {
                            try
                            {
                                Directory.Delete(mod.GetDayzDirLinkPath(), false);
                            }
                            catch (Exception e)
                            {
                                Debug.Print(e.ToString());
                            }
                        }
                        if (mod.HasWorkdriveLink)
                        {
                            try
                            {
                                Directory.Delete(mod.GetWorkdriveLinkPath(), false);
                            }
                            catch (Exception e)
                            {
                                Debug.Print(e.ToString());
                            }
                        }
                    }
                }
                UpdateForeignBindings();
            }
            );

            RemoveAllModLinks = new(() =>
            {
                foreach (ModData mod in Settings.Instance.Mods)
                {
                    try
                    {
                        Directory.Delete(mod.GetDayzDirLinkPath(), false);
                    }
                    catch (Exception e)
                    {
                        Debug.Print(e.ToString());
                    }
                    try
                    {
                        Directory.Delete(mod.GetWorkdriveLinkPath(), false);
                    }
                    catch (Exception e)
                    {
                        Debug.Print(e.ToString());
                    }
                }
                UpdateForeignBindings();
            });

            BuildPbos = new(() =>
                {
                    List<Process> packing = new();
                    foreach (ModData mod in Settings.Instance.Mods)
                    {
                        if (mod.IsActive && mod.BuildPbo)
                        {
                            if (!Directory.Exists(mod.GetPboDir())) Directory.CreateDirectory(mod.GetPboDir());
                            packing.Add(Process.Start(Path.Combine(Settings.Instance.PathDayzTools, "Bin", "AddonBuilder", "AddonBuilder.exe"),
                                $"{mod.GetWorkdriveLinkPath()} {mod.GetPboDir()} -clear"));
                        }
                    }
                });

            StartMpDebugging = new(() =>
            {
                List<string> serverMods = new();
                List<string> clientMods = new();

                foreach (ModData mod in Settings.Instance.Mods)
                {
                    if (mod.IsActive)
                    {
                        if (mod.Clientmod)
                        {
                            clientMods.Add(mod.GetPboDir());
                        }
                        serverMods.Add(mod.GetPboDir());
                    }
                }
                string filePatching = Settings.Instance.Filepatching ? "-filePatching" : "";
                string diagServerArgs = $"-mod={string.Join(';', serverMods)} {filePatching} -server -config=serverDZ.cfg";
                string diagClientArgs = $"-mod={string.Join(';', clientMods)} {filePatching} -connect=127.0.0.1 -port={Settings.Instance.ServerPort}";

                Process.Start(Path.Combine(Settings.Instance.PathDayz, "DayZDiag_x64.exe"), diagServerArgs);
                Process.Start(Path.Combine(Settings.Instance.PathDayz, "DayZDiag_x64.exe"), diagClientArgs);
            });

            instance = this;
        }

        public static DebugViewModel Instance
        { get { return instance; } }

        public int ActiveModsCount
        {
            get
            {
                return SettingsViewModel is null ? 0 : SettingsViewModel.ActiveModsCount;
            }
        }

        public Command BuildPbos { get; }

        public int FullyLinkedMods
        {
            get
            {
                return SettingsViewModel is null ? 0 : SettingsViewModel.FullyLinkedMods;
            }
        }

        public Command RemoveAllModLinks { get; }

        public Command SetupModLinks { get; }

        public Command StartMpDebugging { get; }

        private static SettingsViewModel? SettingsViewModel
        {
            get
            {
                return SettingsViewModel.Instance;
            }
        }

        public static void UpdateForeignBindings()
        {
            instance?.RaisePropertyChanged(nameof(FullyLinkedMods));
            instance?.RaisePropertyChanged(nameof(ActiveModsCount));
        }
    }
}