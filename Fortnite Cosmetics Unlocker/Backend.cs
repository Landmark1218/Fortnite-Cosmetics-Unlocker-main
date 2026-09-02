using Fiddler;
using Newtonsoft.Json.Linq;
using Org.BouncyCastle.Asn1.Ocsp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading;
using System.Threading.Tasks;

namespace Fortnite_Cosmetics_Unlocker
{
    internal class Backend
    {
        private static readonly string ProfileTemplateDir = Path.Combine(Directory.GetCurrentDirectory(), "profile_template");
        private static readonly string LoadoutDir = Path.Combine(Directory.GetCurrentDirectory(), "loadouts");
        private static readonly string EvoLockerPath = Path.Combine(Directory.GetCurrentDirectory(), "EvoLocker.json");

        public static void Listen()
        {
            new Thread(Start) { IsBackground = true }.Start();
        }

        public static void Start()
        {
            EnsureDirectoriesExist();
            EnsureEvoLockerExists();

            HttpListener httpListener = new HttpListener();
            httpListener.Prefixes.Add("http://127.0.0.1:1911/");
            httpListener.IgnoreWriteExceptions = true;
            httpListener.Start();

            while (httpListener.IsListening)
            {
                HttpListenerContext context = null;
                try
                {
                    context = httpListener.GetContext();

                    // /content/api/pages/fortnite-game/ - Content API
                    if (context.Request.HttpMethod == "GET" &&
                        context.Request.RawUrl.StartsWith("/content/api/pages/fortnite-game/"))
                    {
                        HandleContentApi(context);
                    }

                    // /fortnite/api/game/v2/profile/ - Profile API
                    if (context.Request.HttpMethod == "POST" &&
                        context.Request.RawUrl.StartsWith("/fortnite/api/game/v2/profile/"))
                    {
                        HandleProfileApi(context);
                    }

                    // /api/locker/v4/ - Locker API
                    if (context.Request.RawUrl.StartsWith("/api/locker/v4/"))
                    {
                        if (context.Request.HttpMethod == "GET")
                            HandleLockerGet(context);
                        else if (context.Request.HttpMethod == "PUT")
                            HandleLockerPut(context);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                }
                finally
                {
                    context?.Response.Close();
                }
            }
        }

        private static void HandleContentApi(HttpListenerContext context)
        {
            try
            {
                var response = new JsonObject
                {
                    ["_title"] = "Fortnite Game",
                    ["emergencynoticev2"] = new JsonObject
                    {
                        ["_type"] = "Emergency Notices",
                        ["emergencynotices"] = new JsonObject
                        {
                            ["emergencynotices"] = new JsonArray()
                        }
                    }
                };

                SendJsonResponse(context, response, 200);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Content API error: {ex.Message}");
                context.Response.StatusCode = 500;
            }
        }

        private static void HandleProfileApi(HttpListenerContext context)
        {
            try
            {
                var parts = context.Request.RawUrl.Split('/');
                string accountId = parts[6];
                string profileId = context.Request.QueryString["profileId"];
                string rvn = context.Request.QueryString["rvn"];

                string templateFile = Path.Combine(ProfileTemplateDir, $"{profileId}.json");
                if (!File.Exists(templateFile))
                {
                    context.Response.StatusCode = 404;
                    return;
                }

                var profile = JsonNode.Parse(File.ReadAllText(templateFile));
                if (profile == null)
                {
                    context.Response.StatusCode = 500;
                    return;
                }

                // RVN更新
                if (rvn != "-1")
                {
                    profile["rvn"] = Convert.ToInt32(rvn) + 1;
                }
                else
                {
                    profile["rvn"] = 1;
                }

                profile["accountId"] = accountId;

                if (profileId == "athena")
                {
                    profile["stats"]["attributes"]["level"] = 810;
                    profile["stats"]["attributes"]["book_level"] = 810;
                    profile["stats"]["attributes"]["accountLevel"] = 810;
                }

                var profileChanges = new JsonArray();
                var changeObj = new JsonObject
                {
                    ["changeType"] = "fullProfileUpdate",
                    ["profile"] = profile
                };
                profileChanges.Add(changeObj);

                var response = new JsonObject
                {
                    ["profileRevision"] = Convert.ToInt32(profile["rvn"].ToString()),
                    ["profileId"] = profile["profileId"].ToString(),
                    ["profileChangesBaseRevision"] = Convert.ToInt32(profile["rvn"].ToString()),
                    ["profileChanges"] = profileChanges,
                    ["profileCommandRevision"] = Convert.ToInt32(profile["commandRevision"].ToString()),
                    ["serverTime"] = DateTime.UtcNow.ToString("yyyy-MM-dd'T'HH:mm:ss.fff'Z'")
                };

                SendJsonResponse(context, response, 200);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Profile API error: {ex.Message}");
                context.Response.StatusCode = 500;
            }
        }

        private static void HandleLockerGet(HttpListenerContext context)
        {
            try
            {
                var parts = context.Request.RawUrl.Split('/');
                string deploymentId = parts[4];
                string accountId = parts[6];

                if (parts[5] != "account" || parts.Length < 8 || parts[7] != "items")
                    return;

                var evoLocker = JsonNode.Parse(File.ReadAllText(EvoLockerPath));
                if (evoLocker == null)
                {
                    context.Response.StatusCode = 500;
                    return;
                }

                var activeLoadout = new JsonObject
                {
                    ["accountId"] = accountId,
                    ["athenaItemId"] = "9092ba65-01e8-4598-9c09-6de6be10ea49",
                    ["creationTime"] = "0001-01-01T00:00:00Z",
                    ["deploymentId"] = deploymentId,
                    ["loadouts"] = evoLocker,
                    ["namespace"] = "EvoLocker:athena",
                    ["shuffleType"] = "DISABLED",
                    ["updatedTime"] = DateTime.UtcNow.ToString("yyyy-MM-dd'T'HH:mm:ss'Z'")
                };

                var response = new JsonObject
                {
                    ["activeLoadoutGroup"] = activeLoadout,
                    ["loadoutGroupPresets"] = new JsonArray(),
                    ["loadoutPresets"] = new JsonArray()
                };

                SendJsonResponse(context, response, 200);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Locker GET error: {ex.Message}");
                context.Response.StatusCode = 500;
            }
        }

        private static void HandleLockerPut(HttpListenerContext context)
        {
            try
            {
                var parts = context.Request.RawUrl.Split('/');
                if (parts.Length < 8 || parts[5] != "account" || parts[7] != "active-loadout-group")
                    return;

                string deploymentId = parts[4];
                string accountId = parts[6];

                using (var reader = new StreamReader(context.Request.InputStream, context.Request.ContentEncoding))
                {
                    var requestData = JsonNode.Parse(reader.ReadToEnd());
                    if (requestData == null)
                    {
                        context.Response.StatusCode = 400;
                        return;
                    }

                    var evoLocker = JsonNode.Parse(File.ReadAllText(EvoLockerPath));
                    if (evoLocker == null)
                    {
                        context.Response.StatusCode = 500;
                        return;
                    }

                    // loadouts を更新（equippedItemId の最初の2要素のみ保持）
                    if (requestData["loadouts"] is JsonObject loadoutsObj)
                    {
                        foreach (var loadout in loadoutsObj)
                        {
                            if (loadout.Value is JsonObject loadoutData &&
                                loadoutData["loadoutSlots"] is JsonArray slots)
                            {
                                for (int i = 0; i < slots.Count; i++)
                                {
                                    var slot = slots[i] as JsonObject;
                                    if (slot != null)
                                    {
                                        var itemId = slot["equippedItemId"]?.ToString();
                                        if (!string.IsNullOrEmpty(itemId) && itemId.Contains(":"))
                                        {
                                            var trimmed = string.Join(":", itemId.Split(':').Take(2));
                                            Console.WriteLine($"Applied: {trimmed}");
                                            slot["equippedItemId"] = trimmed;
                                        }
                                    }
                                }
                            }
                        }

                        evoLocker = loadoutsObj.DeepClone();
                    }

                    File.WriteAllText(EvoLockerPath, evoLocker.ToString());

                    var response = new JsonObject
                    {
                        ["accountId"] = accountId,
                        ["athenaItemId"] = requestData["athenaItemId"]?.ToString() ?? "9092ba65-01e8-4598-9c09-6de6be10ea49",
                        ["creationTime"] = "0001-01-01T00:00:00Z",
                        ["deploymentId"] = deploymentId,
                        ["loadouts"] = evoLocker,
                        ["namespace"] = "EvoLocker:athena",
                        ["shuffleType"] = "DISABLED",
                        ["updatedTime"] = DateTime.UtcNow.ToString("yyyy-MM-dd'T'HH:mm:ss'Z'")
                    };

                    SendJsonResponse(context, response, 200);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Locker PUT error: {ex.Message}");
                context.Response.StatusCode = 500;
            }
        }

        private static void SendJsonResponse(HttpListenerContext context, JsonObject data, int statusCode)
        {
            var bytes = Encoding.UTF8.GetBytes(data.ToString());
            context.Response.StatusCode = statusCode;
            context.Response.ContentType = "application/json";
            context.Response.ContentLength64 = bytes.Length;
            context.Response.OutputStream.Write(bytes, 0, bytes.Length);
        }

        private static void EnsureDirectoriesExist()
        {
            if (!Directory.Exists(ProfileTemplateDir))
                Directory.CreateDirectory(ProfileTemplateDir);
            if (!Directory.Exists(LoadoutDir))
                Directory.CreateDirectory(LoadoutDir);
        }

        private static void EnsureEvoLockerExists()
        {
            if (!File.Exists(EvoLockerPath))
            {
                var defaultLocker = new JsonObject
                {
                    ["CosmeticLoadout:LoadoutSchema_Character"] = new JsonObject
                    {
                        ["loadoutSlots"] = new JsonArray(),
                        ["shuffleType"] = "DISABLED"
                    }
                };

                File.WriteAllText(EvoLockerPath, defaultLocker.ToString());
            }
        }
    }
}
