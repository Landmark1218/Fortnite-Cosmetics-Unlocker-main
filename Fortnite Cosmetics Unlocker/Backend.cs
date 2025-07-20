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
using System.Threading;
using System.Threading.Tasks;

namespace Fortnite_Cosmetics_Unlocker
{
    internal class Backend
    {
        public static void Listen()
        {
            new Thread(Start).Start();
        }

        public static void Start()
        {
            HttpListener httpListener = new HttpListener();
            httpListener.Prefixes.Add("http://127.0.0.1:1911/");
            httpListener.Start();

            while (httpListener.IsListening)
            {
                HttpListenerContext httpListenerContext = httpListener.GetContext();

                // Console.WriteLine(httpListenerContext.Request.HttpMethod + " " + httpListenerContext.Request.Url.PathAndQuery);

                // "GET /lightswitch/api/service/bulk/status"
                if (httpListenerContext.Request.HttpMethod == "GET" && httpListenerContext.Request.Url.LocalPath.StartsWith("/lightswitch/api/service/"))
                {
                    JArray response = new JArray
                    {
                        new JObject
                        {
                            ["serviceInstanceId"] = "fortnite",
                            ["status"] = "UP",
                            ["message"] = "https://sakurafn.pages.dev/",
                            ["maintenanceUri"] = null,
                            ["overrideCatalogIds"] = new JArray
                            {
                                "a7f138b2e51945ffbfdacc1af0541053"
                            },
                            ["allowedActions"] = new JArray
                            {
                                "PLAY",
                                "DOWNLOAD"
                            },
                            ["banned"] = false,
                            ["launcherInfoDTO"] = new JObject
                            {
                                ["appName"] = "Fortnite",
                                ["catalogItemId"] = "4fe75bbc5a674f4f9b356b5c90567da5",
                                ["namespace"] = "fn"
                            }
                        }
                    };

                    string data = response.ToString();

                    httpListenerContext.Response.StatusCode = 200;
                    httpListenerContext.Response.ContentType = "application/json";
                    httpListenerContext.Response.ContentLength64 = Encoding.UTF8.GetBytes(data).Length;
                    httpListenerContext.Response.OutputStream.Write(Encoding.UTF8.GetBytes(data), 0, Encoding.UTF8.GetBytes(data).Length);
                }

                // "POST /fortnite/api/game/v2/profile/:accountId/:route/:operation"
                if (httpListenerContext.Request.HttpMethod == "POST" && httpListenerContext.Request.Url.LocalPath.StartsWith("/fortnite/api/game/v2/profile/"))
                {
                    JObject profile = JObject.Parse(File.ReadAllText(Path.Combine(Directory.GetCurrentDirectory(), "profiles", $"{httpListenerContext.Request.QueryString["profileId"]}.json")));

                    if (profile == null)
                    {
                        httpListenerContext.Response.StatusCode = 500; // server internal error
                    }
                    else
                    {
                        if (httpListenerContext.Request.QueryString["rvn"] != "-1")
                        {
                            profile["rvn"] = Convert.ToInt32(httpListenerContext.Request.QueryString["rvn"]) + 1;
                        }

                        JObject response = new JObject
                        {
                            ["profileRevision"] = Convert.ToInt32(profile["rvn"]),
                            ["profileId"] = profile["profileId"],
                            ["profileChangesBaseRevision"] = Convert.ToInt32(profile["rvn"]),
                            ["profileChanges"] = new JArray
                            {
                                new JObject
                                {
                                    ["changeType"] = "fullProfileUpdate",
                                    ["profile"] = profile
                                }
                            },
                            ["profileCommandRevision"] = Convert.ToInt32(profile["commandRevision"]),
                            ["serverTime"] = DateTime.Now.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss.fff'Z'"),
                        };

                        string data = response.ToString();

                        httpListenerContext.Response.StatusCode = 200;
                        httpListenerContext.Response.ContentType = "application/json";
                        httpListenerContext.Response.ContentLength64 = Encoding.UTF8.GetBytes(data).Length;
                        httpListenerContext.Response.OutputStream.Write(Encoding.UTF8.GetBytes(data), 0, Encoding.UTF8.GetBytes(data).Length);
                    }
                }



                // "GET /api/locker/v4/:deploymentId/account/:accountId/items"
                if (httpListenerContext.Request.HttpMethod == "GET" && httpListenerContext.Request.Url.LocalPath.StartsWith("/api/locker/v4/"))
                {
                    if (httpListenerContext.Request.Url.LocalPath.Split('/')[5] == "account" && httpListenerContext.Request.Url.LocalPath.Split('/')[7] == "items")
                    {
                        string deploymentId = httpListenerContext.Request.Url.LocalPath.Split('/')[4];
                        string accountId = httpListenerContext.Request.Url.LocalPath.Split('/')[6];

                        if (Locker.Initialize(deploymentId, accountId))
                        {
                            string data = Locker.Get(deploymentId, accountId).ToString();

                            httpListenerContext.Response.StatusCode = 200;
                            httpListenerContext.Response.ContentType = "application/json";
                            httpListenerContext.Response.ContentLength64 = Encoding.UTF8.GetBytes(data).Length;
                            httpListenerContext.Response.OutputStream.Write(Encoding.UTF8.GetBytes(data), 0, Encoding.UTF8.GetBytes(data).Length);
                        }
                        else
                        {
                            httpListenerContext.Response.StatusCode = 500; // server internal error
                        }
                    }
                }
                
                    // "PUT /api/locker/v4/:deploymentId/account/:accountId/active-loadout-group"
                    if (httpListenerContext.Request.HttpMethod == "PUT" && httpListenerContext.Request.Url.LocalPath.StartsWith("/api/locker/v4/"))
                    {
                        if (httpListenerContext.Request.Url.LocalPath.Split('/')[5] == "account" && httpListenerContext.Request.Url.LocalPath.Split('/')[7] == "active-loadout-group")
                        {
                            using (StreamReader streamReader = new StreamReader(httpListenerContext.Request.InputStream, httpListenerContext.Request.ContentEncoding))
                            {
                                string deploymentId = httpListenerContext.Request.Url.LocalPath.Split('/')[4];
                                string accountId = httpListenerContext.Request.Url.LocalPath.Split('/')[6];

                                if (Locker.Initialize(deploymentId, accountId))
                                {
                                    string data = Locker.Put(deploymentId, accountId, streamReader.ReadToEnd()).ToString();

                                    httpListenerContext.Response.StatusCode = 200;
                                    httpListenerContext.Response.ContentType = "application/json";
                                    httpListenerContext.Response.ContentLength64 = Encoding.UTF8.GetBytes(data).Length;
                                    httpListenerContext.Response.OutputStream.Write(Encoding.UTF8.GetBytes(data), 0, Encoding.UTF8.GetBytes(data).Length);
                                }
                                else
                                {
                                    httpListenerContext.Response.StatusCode = 500; // server internal error
                                }
                            }
                        }
                    }

                    httpListenerContext.Response.Close();
                }
            }
        }
    }
