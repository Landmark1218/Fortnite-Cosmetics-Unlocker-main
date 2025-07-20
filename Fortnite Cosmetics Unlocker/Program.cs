using Fiddler;
using Newtonsoft.Json.Linq;
using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Sockets;

namespace Fortnite_Cosmetics_Unlocker
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string profilesPath = Path.Combine(Directory.GetCurrentDirectory(), "profiles");
            if (!Directory.Exists(profilesPath))
            {
                Directory.CreateDirectory(profilesPath);
            }

            using (WebClient webClient = new WebClient())
            {
                (string url, string fileName)[] files = new[]
                {
                    ("https://github.com/Landmark1218/SakuraSwapper/raw/refs/heads/main/athena.json", "athena.json"),
                    ("https://github.com/Landmark1218/SakuraSwapper/raw/refs/heads/main/campaign.json", "campaign.json"),
                    ("https://github.com/Landmark1218/SakuraSwapper/raw/refs/heads/main/collection_book_people0.json", "collection_book_people0.json"),
                    ("https://sakurafn.pages.dev/hybrid/profile_template/collection_book_schematics0.json", "collection_book_schematics0.json"),
                    ("https://github.com/Landmark1218/SakuraSwapper/raw/refs/heads/main/collections.json", "collections.json"),
                    ("https://github.com/Landmark1218/SakuraSwapper/raw/refs/heads/main/common_core.json", "common_core.json"),
                    ("https://github.com/Landmark1218/SakuraSwapper/raw/refs/heads/main/common_public.json", "common_public.json"),
                    ("https://github.com/Landmark1218/SakuraSwapper/raw/refs/heads/main/creative.json", "creative.json"),
                    ("https://github.com/Landmark1218/SakuraSwapper/raw/refs/heads/main/metadata.json", "metadata.json"),
                    ("https://github.com/Landmark1218/SakuraSwapper/raw/refs/heads/main/outpost0.json", "outpost0.json"),
                    ("https://sakurafn.pages.dev/hybrid/profile_template/recycle_bin.json", "recycle_bin.json"),
                    ("https://github.com/Landmark1218/SakuraSwapper/raw/refs/heads/main/theater0.json", "theater0.json"),
                    ("https://sakurafn.pages.dev/hybrid/profile_template/theater1.json", "theater1.json"),
                    ("https://sakurafn.pages.dev/hybrid/profile_template/theater2.json", "theater2.json"),
                };

                foreach (var (url, fileName) in files)
                {
                    string savePath = Path.Combine(profilesPath, fileName);

                    if (File.Exists(savePath))
                    {
                        Console.WriteLine($"Skipped (already exists): {fileName}");
                        continue;
                    }

                    try
                    {
                        Console.WriteLine($"Downloading...{url}");
                        webClient.DownloadFile(url, savePath);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Failed to download {url}: {ex.Message}");
                    }
                }
            }

            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Welcome to Cosmetics Unlocker For PIE!");
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("Credit BiruFN,Landmark0920");
            Console.ResetColor();

            if (!Fiddler.Setup())
            {
                Console.WriteLine("Fiddlerのセットアップに失敗した");
                return;
            }

            var startupSettings = new FiddlerCoreStartupSettingsBuilder()
                .ListenOnPort(9999)
                .DecryptSSL()
                .RegisterAsSystemProxy()
                .Build();

            FiddlerApplication.BeforeRequest += OnBeforeRequest;
            FiddlerApplication.BeforeResponse += OnBeforeResponse;

            AppDomain.CurrentDomain.ProcessExit += (sender, e) =>
            {
                Console.WriteLine("Process exiting. Shutting down fiddler application...");
                FiddlerApplication.Shutdown();
                KillFortniteProcess();
            };

            Console.CancelKeyPress += (sender, e) =>
            {
                Console.WriteLine("Cancel key pressed. Shutting down fiddler application...");
                FiddlerApplication.Shutdown();
                KillFortniteProcess();
                Environment.Exit(0);
            };

            Console.WriteLine("Starting fiddler application");
            FiddlerApplication.Startup(startupSettings);

            Backend.Listen();
            Console.WriteLine("Listening to backend");

            TryLaunchPlayInFrontEnd();

            Console.WriteLine("Starting PIE");
            Console.WriteLine("To exit, press any key in this window to exit");
            Console.ReadKey(true);

            Console.WriteLine("Shutting down fiddler application");
            FiddlerApplication.Shutdown();
            KillFortniteProcess();

            Environment.Exit(0);
        }

        private static void KillFortniteProcess()
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
                Console.WriteLine($"プロセス終了時エラー: {ex.Message}");
            }
        }

        private static void TryLaunchPlayInFrontEnd()
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
                Console.WriteLine("Manifestsフォルダがないです");
                return;
            }

            string[] itemFiles = Directory.GetFiles(manifestsDir, "*.item");
            string targetLaunchExecutable = "FortniteGame/Binaries/Win64/UnrealEditorFortnite-Win64-Shipping.exe";

            foreach (var itemFile in itemFiles)
            {
                try
                {
                    string json = File.ReadAllText(itemFile);
                    JObject root = JObject.Parse(json);

                    var launchExeToken = root["LaunchExecutable"];
                    if (launchExeToken != null)
                    {
                        string launchExe = launchExeToken.ToString();

                        if (!string.IsNullOrEmpty(launchExe) && launchExe.Contains(targetLaunchExecutable))
                        {
                            var installLocToken = root["InstallLocation"];
                            if (installLocToken != null)
                            {
                                string installLocation = installLocToken.ToString();
                                string exeFullPath = Path.Combine(installLocation, launchExe.Replace('/', Path.DirectorySeparatorChar));
                                string exeDir = Path.GetDirectoryName(exeFullPath);

                                if (Directory.Exists(exeDir))
                                {
                                    string playInFrontEndExe = Path.Combine(exeDir, "UnrealEditorFortnite-Win64-Shipping-PlayInFrontEnd.exe");

                                    if (File.Exists(playInFrontEndExe))
                                    {
                                        var startInfo = new ProcessStartInfo
                                        {
                                            FileName = playInFrontEndExe,
                                            Arguments = "-disableplugins=\"AtomVK,ValkyrieFortnite\"",
                                            UseShellExecute = false,
                                        };

                                        Process.Start(startInfo);
                                    }
                                }
                            }
                            break;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"解析中エラー: {ex.Message}");
                }
            }
        }

        private static void OnBeforeRequest(Session session)
        {
            try
            {
                if (session.RequestHeaders["User-Agent"].Split('/')[0] == "Fortnite")
                {
                    if (session.PathAndQuery.StartsWith("/lightswitch/api/service/") ||
                        session.PathAndQuery.StartsWith("/fortnite/api/game/v2/profile/") ||
                        session.PathAndQuery.StartsWith("/api/locker/v4/"))
                    {
                        session.fullUrl = "http://localhost:1911" + session.PathAndQuery;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"OnBeforeRequest エラー: {ex.Message}");
            }
        }

        private static void OnBeforeResponse(Session session)
        {
            // :)
        }
    }
}