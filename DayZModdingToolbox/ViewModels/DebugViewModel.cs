using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

using DayZModdingToolbox.Common;
using DayZModdingToolbox.Data;

namespace DayZModdingToolbox.ViewModels
{
    public class DebugViewModel : BindableBase
    {
        private bool _clientAndServerDebug;

        public int ActiveModsCount
        {
            get
            {
                return Settings.Instance.Mods is null ? 0 : Settings.Instance.Mods.Count((x) => x.IsActive);
            }
        }

        public Command BuildPbos { get; } = new(() =>
        {
            List<Process> packing = new();
            foreach (ModData mod in Settings.Instance.Mods)
            {
                if (mod.IsActive && mod.BuildPbo)
                {
                    if (!Directory.Exists(mod.GetPboDir())) Directory.CreateDirectory(mod.GetPboDir());
                    string project = string.Empty;
                    if (string.IsNullOrEmpty(mod.Modpack)) project = $" -project={mod.Modpack}";
                    string args = $"{mod.GetWorkdriveLinkPath()} {mod.GetPboDir()} -clear{project}";
                    packing.Add(Process.Start(Path.Combine(Settings.Instance.PathDayzTools, "Bin", "AddonBuilder", "AddonBuilder.exe"), args));
                }
            }
        });

        public Command CleanupWorkdrive
        {
            get
            {
                return new(() =>
                {
                    string currentDir = Directory.GetCurrentDirectory();
                    var FixScripts = Path.Combine(currentDir, "FixScripts.bat");
                    Process.Start(FixScripts);
                });
            }
        }

        public bool ClientAndServerDebug
        {
            get
            {
                return _clientAndServerDebug;
            }
            set
            {
                SetProperty(ref _clientAndServerDebug, value);
            }
        }

        public Command ExtractGameData
        {
            get
            {
                return new(() =>
                {
                    string workdrive = Path.Combine(Settings.Instance.PathDayzTools, "Bin", "WorkDrive", "WorkDrive.exe");
                    string args = "/ExtractGameData";
                    Process.Start(workdrive, args);
                });
            }
        }

        public int FullyLinkedMods
        {
            get
            {
                return Settings.Instance.Mods is null ? 0 : Settings.Instance.Mods.Count(x => x.HasDayzDirLink && x.HasWorkdriveLink);
            }
        }

        public Command RemoveAllModLinks
        {
            get
            {
                return new(() =>
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
            }
        }

        public Command SetupModLinks
        {
            get
            {
                return new(() =>
                 {
                     foreach (ModData mod in Settings.Instance.Mods)
                     {
                         mod.Update();

                         // Check and create links
                         if (mod.IsActive)
                         {
                             if (!mod.HasDayzDirLink)
                             {
                                 Directory.CreateDirectory(string.Join(@"\", mod.GetDayzDirLinkPath().Split(@"\")[..^1]));
                                 _ = Directory.CreateSymbolicLink(mod.GetDayzDirLinkPath(), mod.ModPath);
                             }
                             if (!mod.HasWorkdriveLink)
                             {
                                 Directory.CreateDirectory(string.Join(@"\", mod.GetWorkdriveLinkPath().Split(@"\")[..^1]));
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
            }
        }

        public Command StartMpDebugging
        {
            get
            {
                return new(async () =>
                {
                    List<string> serverMods = new();
                    List<string> clientMods = new();

                    foreach (ModData mod in Settings.Instance.Mods)
                    {
                        if (mod.IsActive)
                        {
                            if (mod.Clientmod)
                            {
                                if (!clientMods.Contains(GetModDebugPath(mod))) clientMods.Add(GetModDebugPath(mod));
                            }

                            if (!serverMods.Contains(GetModDebugPath(mod))) serverMods.Add(GetModDebugPath(mod));
                        }
                    }
                    string filePatching = Settings.Instance.Filepatching ? " -filePatching" : "";
                    string modLoadParamServer = string.Empty;
                    if (serverMods.Count > 0)
                    {
                        modLoadParamServer = $" \"-mod={string.Join(';', serverMods)}\"";
                    }
                    string diagServerArgs = $"-server -config=serverDZ.cfg{filePatching}{modLoadParamServer}";

                    Process.Start(Path.Combine(Settings.Instance.PathDayz, "DayZDiag_x64.exe"), diagServerArgs);

                    if (ClientAndServerDebug)
                    {
                        await Task.Delay(TimeSpan.FromSeconds(10));

                        string modLoadParamClient = string.Empty;
                        if (clientMods.Count > 0)
                        {
                            modLoadParamClient = $" \"-mod={string.Join(';', clientMods)}\"";
                        }

                        string diagClientArgs = $"-connect=127.0.0.1 -port={Settings.Instance.ServerPort}{filePatching}{modLoadParamClient}";
                        Process.Start(Path.Combine(Settings.Instance.PathDayz, "DayZDiag_x64.exe"), diagClientArgs);
                    }
                });
            }
        }

        public Command StartWorkbench { get; } = new(() =>
        {
            List<string> mods = new();

            foreach (ModData mod in Settings.Instance.Mods)
            {
                if (mod.IsActive)
                {
                    mods.Add(mod.GetWorkdriveLinkPath());
                }
            }

            string workbenchArgs = $"\"-mod={string.Join(';', mods)}\"";
            string workbench = Path.Combine(Settings.Instance.PathDayzTools, "Bin", "Workbench", "workbenchApp.exe");
            var info = new ProcessStartInfo(workbench, workbench);
            info.WorkingDirectory = Settings.Instance.PathDayz;
            Process.Start(info);
        }
        );

        public void UpdateForeignBindings()
        {
            RaisePropertyChanged(nameof(FullyLinkedMods));
            RaisePropertyChanged(nameof(ActiveModsCount));
        }

        private static string GetModDebugPath(ModData mod)
        {
            if (!string.IsNullOrWhiteSpace(mod.Modpack))
            {
                return mod.GetModpackDir();
            }

            return mod.GetModDir();
        }
    }
}