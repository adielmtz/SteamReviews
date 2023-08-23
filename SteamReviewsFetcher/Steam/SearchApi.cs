using SteamReviewsFetcher.Steam.Entities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SteamReviewsFetcher.Steam
{
    internal sealed class SearchApi
    {
        private const string STEAM_SEARCH_ENDPOINT = "https://store.steampowered.com/search/suggest";
        private const string TRADEMARK_SYMBOL = "™";

        private static readonly Regex AppIdRegex = new Regex(@"data-ds-appid=""(\d+)""");
        private static readonly Regex AppNameRegex = new Regex(@"<div\s+class=""match_name\s*""\>(.[^<]+)</div>");

        public static async Task<IList<AppInfo>> SearchGamesAsync(string name)
        {
            using var client = new HttpClient();
            client.BaseAddress = new Uri(STEAM_SEARCH_ENDPOINT);

            var queryParams = new Dictionary<string, string>
            {
                { "term", name },
                { "f", "games" },
                { "cc", "MX" },
                { "realm", "1" },
                { "l", "spanish" },
                { "use_store_query", "1" },
                { "use_search_spellcheck", "1" },
            };

            using var encoder = new FormUrlEncodedContent(queryParams);
            string query = await encoder.ReadAsStringAsync();

            Debug.WriteLine($"GET {STEAM_SEARCH_ENDPOINT}?{query}");

            string response = await client.GetStringAsync($"?{query}");
            return ParseAppInfo(response);
        }

        private static IList<AppInfo> ParseAppInfo(string response)
        {
            string[] segments = response.Split("</a>", StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
            var apps = new List<AppInfo>(segments.Length);

            for (int i = 0; i < segments.Length; i++)
            {
                string segment = segments[i];
                if (segment.Contains(TRADEMARK_SYMBOL))
                {
                    segment = segment.Replace(TRADEMARK_SYMBOL, "");
                }

                Match appIdMatch = AppIdRegex.Match(segment);
                Match appNameMatch = AppNameRegex.Match(segment);

                if (appIdMatch.Success && appNameMatch.Success)
                {
                    var app = new AppInfo
                    {
                        Id = int.Parse(appIdMatch.Groups[1].Value.Trim()),
                        Name = appNameMatch.Groups[1].Value.Trim(),
                    };

                    apps.Add(app);
                }
            }

            return apps;
        }
    }
}
