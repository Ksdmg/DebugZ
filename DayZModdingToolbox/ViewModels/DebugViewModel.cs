using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

using DayZModdingToolbox.Common;
using DayZModdingToolbox.Data;

namespace DayZModdingToolbox.ViewModels
{
    public class DebugViewModel : BindableBase
    {
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
            });

            BuildPbos = new(async () =>
                {
                    List<Process> packing = new();
                    foreach (ModData mod in Settings.Instance.Mods)
                    {
                        if (mod.IsActive)
                        {
                            if (!Directory.Exists(mod.GetPboDir())) Directory.CreateDirectory(mod.GetPboDir());
                            packing.Add(Process.Start(Path.Combine(Settings.Instance.PathDayzTools, "Bin", "AddonBuilder", "AddonBuilder.exe"),
                                $"{mod.GetWorkdriveLinkPath()} {mod.GetPboDir()} -clear"));
                        }

                    }

                });
        }

        public Command BuildPbos { get; }
        public Command RemoveAllModLinks { get; }
        public Command SetupModLinks { get; }
    }
}