using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using DayZModdingToolbox.Common;
using DayZModdingToolbox.Data;
using Monitor.Core.Utilities;

namespace DayZModdingToolbox.ViewModels
{
    public class DebugViewModel : BindableBase
    {
        private bool _clientAndServerDebug;

        public DebugViewModel()
        {
            UpdateForeignBindings();
        }

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
                    if (!string.IsNullOrEmpty(mod.Modpack)) project = $" -project={mod.Modpack}";
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
                    string fixscriptsFile = Path.Combine(currentDir, "FixScripts.bat");

                    ReplaceLineInFile($"set ROOT_DIR={Path.Combine(Settings.Instance.PathWorkdrive, "scripts")}\r\n", fixscriptsFile, 9);

                    Process.Start(fixscriptsFile);
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

        public Command DismountPDrive { get; } = new(() =>
        {
            if (!Directory.Exists(Settings.Instance.PathWorkdrive))
            {
                MessageBox.Show("Workdrive not mounted.");
                return;
            }

            string workdriveTool = Path.Combine(Settings.Instance.PathDayzTools, "Bin", "WorkDrive", "WorkDrive.exe");
            string args = "/y /Silent /nowarnings /dismount P:";

            Process.Start(workdriveTool, args);
        });

        public Command ExtractGameData
        {
            get
            {
                return new(() =>
                {
                    string workdrive = Path.Combine(Settings.Instance.PathDayzTools, "Bin", "WorkDrive", "WorkDrive.exe");
                    string args = $"/ExtractGameData \"{Settings.Instance.PathDayz}\" \"{Settings.Instance.PathWorkDir}\"";
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

        public Command MountPDrive { get; } = new(() =>
        {
            if (Directory.Exists(Settings.Instance.PathWorkdrive))
            {
                MessageBox.Show("Workdrive already mounted.");
                return;
            }

            string workdriveTool = Path.Combine(Settings.Instance.PathDayzTools, "Bin", "WorkDrive", "WorkDrive.exe");
            string args = $"/y /Silent /nowarnings /mount P: \"{Settings.Instance.PathWorkDir}\"";

            Process.Start(workdriveTool, args);
        });

        public Command RefreshModLinks
        {
            get
            {
                return new(() =>
                {
                    if (!Path.Exists(Settings.Instance.PathWorkdrive))
                    {
                        MessageBox.Show("Workdrive not mounted.");
                        return;
                    }
                    UpdateForeignBindings();
                });
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
                     if (!Path.Exists(Settings.Instance.PathWorkdrive))
                     {
                         MessageBox.Show("Mount workdrive first.");
                         return;
                     }

                     // Check scripts link
                     var scriptsDirInfo = new DirectoryInfo(Path.Combine(Settings.Instance.PathDayz, "scripts"));
                     if (scriptsDirInfo.Exists)
                     {
                         if (!((scriptsDirInfo.Attributes & FileAttributes.ReparsePoint) == FileAttributes.ReparsePoint))
                         {
                             var result = MessageBox.Show("Warning: Your scripts folder is not a symbolic link. Do you want to remove the folder and recreate it as symbolic link?", "Warning", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                             if (result == MessageBoxResult.Yes)
                             {
                                 scriptsDirInfo.Delete(true);
                                 JunctionPoint.Create(Path.Combine(Settings.Instance.PathDayz, "scripts"), Path.Combine(Settings.Instance.PathWorkdrive, "scripts"), true);
                             }
                         }
                     }
                     else
                     {
                         JunctionPoint.Create(Path.Combine(Settings.Instance.PathDayz, "scripts"), Path.Combine(Settings.Instance.PathWorkdrive, "scripts"), true);
                     }

                     foreach (ModData mod in Settings.Instance.Mods)
                     {
                         mod.Update();

                         // Check and create links
                         if (mod.IsActive)
                         {
                             if (!mod.HasDayzDirLink)
                             {
                                 JunctionPoint.Create(mod.GetDayzDirLinkPath(), mod.ModPath, true);
                             }
                             if (!mod.HasWorkdriveLink)
                             {
                                 JunctionPoint.Create(mod.GetWorkdriveLinkPath(), mod.ModPath, true);
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
                    var modPath = mod.GetWorkdriveLinkPath();
                    if (modPath.EndsWith('\\')) { modPath = modPath[..^1]; };
                    mods.Add(modPath);
                }
            }
            string workbenchArgs = "-scriptDebug=true";
            if (mods.Count > 0) { workbenchArgs = $" \"-mod={string.Join(';', mods)}\""; }
            string workbench = Path.Combine(Settings.Instance.PathDayzTools, "Bin", "Workbench", "workbenchApp.exe");
            var info = new ProcessStartInfo(workbench, workbenchArgs);
            info.WorkingDirectory = Path.GetDirectoryName(workbench);
            Process.Start(info);
        });

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

        private static void ReplaceLineInFile(string newText, string fileName, int line_to_edit)
        {
            string[] arrLine = File.ReadAllLines(fileName);
            arrLine[line_to_edit - 1] = newText;
            File.WriteAllLines(fileName, arrLine);
        }
    }
}