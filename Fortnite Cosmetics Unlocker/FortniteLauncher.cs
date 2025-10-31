using Newtonsoft.Json.Linq;
using System;
using System.Diagnostics;
using System.IO;
using System.Net;

namespace Fortnite_Cosmetics_Unlocker
{
    internal static class FortniteLauncher
    {
        public static void KillFortniteProcess()
        {
            try
            {
                foreach (var proc in Process.GetProcessesByName("UnrealEditorFortnite-Win64-Shipping-PlayInFrontEnd"))
                {
                    proc.Kill();
                    proc.WaitForExit();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Process termination error: {ex.Message}");
            }
        }

        public static void TryLaunchPlayInFrontEnd()
        {
            string manifestsDir = null;

            foreach (char drive in "CDEFGHIJKLMNOPQRSTUVWXYZ")
            {
                string EpicManifests = $@"{drive}:\\ProgramData\\Epic\\EpicGamesLauncher\\Data\\Manifests";
                if (Directory.Exists(EpicManifests))
                {
                    manifestsDir = EpicManifests;
                    break;
                }
            }

            if (manifestsDir == null)
            {
                Console.WriteLine("There is no Manifests folder");
                return;
            }

            foreach (var itemFile in Directory.GetFiles(manifestsDir, "*.item"))
            {
                try
                {
                    string json = File.ReadAllText(itemFile);
                    JObject root = JObject.Parse(json);

                    var launchExeToken = root["LaunchExecutable"];
                    if (launchExeToken == null) continue;

                    string launchExe = launchExeToken.ToString();
                    if (string.IsNullOrEmpty(launchExe) || !launchExe.Contains("FortniteGame/Binaries/Win64/UnrealEditorFortnite-Win64-Shipping.exe"))
                        continue;

                    var installLocToken = root["InstallLocation"];
                    if (installLocToken == null) continue;

                    string installLocation = installLocToken.ToString();
                    string exeDir = Path.Combine(installLocation, "FortniteGame", "Binaries", "Win64");
                    string playInFrontEndExe = Path.Combine(exeDir, "UnrealEditorFortnite-Win64-Shipping-PlayInFrontEnd.exe");

                    DownloadPaks(installLocation);

                    if (File.Exists(playInFrontEndExe))
                    {
                        Process.Start(new ProcessStartInfo
                        {
                            FileName = playInFrontEndExe,
                            Arguments = "-disableplugins=\"AtomVK,ValkyrieFortnite\"",
                            UseShellExecute = false,
                        });
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Parsing error: {ex.Message}");
                }
            }
        }

        private static void DownloadPaks(string installLocation)
        {
            string paksPath = Path.Combine(installLocation, "FortniteGame", "Content", "Paks");
            if (!Directory.Exists(paksPath))
            {
                Console.WriteLine("The Paks folder was not found.");
                return;
            }

            var pakFiles = new[]
            {
                ("https://github.com/Landmark1218/Trash/raw/refs/heads/main/UEFNFortniteGame-WindowsUEFN_4.pak", "UEFNFortniteGame-WindowsUEFN_4.pak"),
                ("https://github.com/Landmark1218/Trash/raw/refs/heads/main/UEFNFortniteGame-WindowsUEFN_AthenaHUD.pak", "UEFNFortniteGame-WindowsUEFN_AthenaHUD.pak"),
                ("https://github.com/Landmark1218/Trash/raw/refs/heads/main/UEFNFortniteGame-WindowsUEFN_HUDs.pak", "UEFNFortniteGame-WindowsUEFN_HUDs.pak"),
                ("https://github.com/Landmark1218/Trash/raw/refs/heads/main/UEFNFortniteGame-WindowsUEFN_Unlock_P.pak", "UEFNFortniteGame-WindowsUEFN_Unlock_P.pak"),
            };

            using (WebClient wc = new WebClient())
            {
                foreach (var (url, fileName) in pakFiles)
                {
                    string destination = Path.Combine(paksPath, fileName);
                    if (File.Exists(destination)) continue;

                    try
                    {
                        Console.WriteLine($"Downloading...: {url}");
                        wc.DownloadFile(url, destination);
                        Console.WriteLine($"Download complete");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Download failed: {fileName} - {ex.Message}");
                    }
                }
            }
        }
    }
}
