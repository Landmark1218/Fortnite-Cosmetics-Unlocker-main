using Fiddler;
using System;

namespace Fortnite_Cosmetics_Unlocker
{
    internal static class FiddlerHandlers
    {
        public static void OnBeforeRequest(Session session)
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
                Console.WriteLine($"OnBeforeRequest error: {ex.Message}");
            }
        }

        public static void OnBeforeResponse(Session session)
        {
            // :)
        }
    }
}
