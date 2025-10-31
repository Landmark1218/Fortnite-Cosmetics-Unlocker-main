using System;
using System.IO;
using System.Net;

namespace Fortnite_Cosmetics_Unlocker
{
    internal static class Downloader
    {
        private static readonly (string url, string fileName)[] JsonFiles = new[]
        {
            ("https://github.com/Landmark1218/Trash/raw/refs/heads/main/athena.json", "athena.json"),
            ("https://github.com/Landmark1218/Trash/raw/refs/heads/main/campaign.json", "campaign.json"),
            // ...残りのJSONファイルも同様
        };

        public static void EnsureProfilesExist()
        {
            string profilesPath = Path.Combine(Directory.GetCurrentDirectory(), "profiles");
            if (!Directory.Exists(profilesPath))
                Directory.CreateDirectory(profilesPath);

            using (WebClient webClient = new WebClient())
            {
                foreach (var (url, fileName) in JsonFiles)
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
        }
    }
}
