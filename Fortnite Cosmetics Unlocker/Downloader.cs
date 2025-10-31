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
            ("https://github.com/Landmark1218/Trash/raw/refs/heads/main/collection_book_people0.json", "collection_book_people0.json"),
            ("https://github.com/Landmark1218/Trash/raw/refs/heads/main/collection_book_schematics0.json", "collection_book_schematics0.json"),
            ("https://github.com/Landmark1218/Trash/raw/refs/heads/main/collections.json", "collections.json"),
            ("https://github.com/Landmark1218/Trash/raw/refs/heads/main/common_core.json", "common_core.json"),
            ("https://github.com/Landmark1218/Trash/raw/refs/heads/main/common_public.json", "common_public.json"),
            ("https://github.com/Landmark1218/Trash/raw/refs/heads/main/creative.json", "creative.json"),
            ("https://github.com/Landmark1218/Trash/raw/refs/heads/main/metadata.json", "metadata.json"),
            ("https://github.com/Landmark1218/Trash/raw/refs/heads/main/outpost0.json", "outpost0.json"),
            ("https://github.com/Landmark1218/Trash/raw/refs/heads/main/recycle_bin.json", "recycle_bin.json"),
            ("https://github.com/Landmark1218/Trash/raw/refs/heads/main/theater0.json", "theater0.json"),
            ("https://github.com/Landmark1218/Trash/raw/refs/heads/main/theater1.json", "theater1.json"),
            ("https://github.com/Landmark1218/Trash/raw/refs/heads/main/theater2.json", "theater2.json"),
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
