using Newtonsoft.Json.Linq;
using System;
using System.Diagnostics;
using System.IO;

namespace Fortnite_Cosmetics_Unlocker
{
    internal static class FortniteLauncher
    {
        public static void KillFortniteProcess()
        {
            try
            {
                foreach (var proc in Process.GetProcessesByName("UnrealEditorFortnite-Win64-Shipping"))
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
                string EpicManifests = $@"{drive}:\ProgramData\Epic\EpicGamesLauncher\Data\Manifests";
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
                    if (string.IsNullOrEmpty(launchExe) ||
                        !launchExe.Contains("FortniteGame/Binaries/Win64/UnrealEditorFortnite-Win64-Shipping.exe"))
                        continue;

                    var installLocToken = root["InstallLocation"];
                    if (installLocToken == null) continue;

                    string installLocation = installLocToken.ToString();
                    string exeDir = Path.Combine(installLocation, "FortniteGame", "Binaries", "Win64");
                    string playInFrontEndExe = Path.Combine(
                        exeDir,
                        "UnrealEditorFortnite-Win64-Shipping.exe"
                    );

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
    }
}
