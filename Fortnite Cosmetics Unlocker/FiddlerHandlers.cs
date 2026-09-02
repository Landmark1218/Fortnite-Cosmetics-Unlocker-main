using Fiddler;
using System;

namespace Fortnite_Cosmetics_Unlocker
{
    internal static class FiddlerHandlers
    {
        public static void OnBeforeRequest(Session session)
        {
            if (session.RequestHeaders["User-Agent"].Split('/')[0] == "FortniteGame" &&
                session.PathAndQuery.StartsWith("/content/api/pages/fortnite-game/"))
            {
                session.fullUrl = "http://localhost:1911" + session.PathAndQuery;
            }

            if (session.RequestHeaders["User-Agent"].Split('/')[0] == "Fortnite")
            {
                //Console.WriteLine($"OnBeforeRequest error: {ex.Message}");
                if (session.PathAndQuery.StartsWith("/fortnite/api/game/v2/profile/") ||
                    session.PathAndQuery.StartsWith("/api/locker/v4/"))
                {
                    session.fullUrl = "http://localhost:1911" + session.PathAndQuery;
                }
            }
        }

        public static void OnBeforeResponse(Session session)
        {
            // :)
        }
    }
}
