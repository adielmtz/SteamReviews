using SteamReviewsFetcher.Steam;
using SteamReviewsFetcher.Steam.Entities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace SteamReviewsFetcher
{
    internal static class Program
    {
        private const string STEAM_API_ENDPOINT = "https://store.steampowered.com/appreviews/";
        private const int STEAM_MAX_REVIEWS = 2000;

        private static readonly List<AppInfo> GameList = new List<AppInfo>();

        public static async Task Main()
        {
            while (true)
            {
                GameList.Clear();
                await LookupGamesAsync();

                Console.Write("Language: ");
                string lang = Console.ReadLine()!;

                if (GameList.Count > 0)
                {
                    Console.Clear();

                    foreach (AppInfo app in GameList)
                    {
                        await DownloadGameReviewsAsync(app, lang);
                    }

                    GameList.RemoveAll(app => app.Reviews.Count == 0);
                    Console.Clear();
                    BeginListening();
                }
            }
        }

        private static async Task LookupGamesAsync()
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("Adding game #{0}", GameList.Count + 1);
                Console.Write("Search: ");
                string input = Console.ReadLine()!;

                if (string.IsNullOrEmpty(input))
                {
                    break;
                }

                IList<AppInfo> results = await SearchApi.SearchGamesAsync(input);
                if (results.Count > 0)
                {
                    while (true)
                    {
                        for (int i = 0; i < results.Count; i++)
                        {
                            AppInfo app = results[i];
                            Console.WriteLine("{0}: [{1}] {2}", i + 1, app.Id, app.Name);
                        }

                        Console.Write("> ");
                        input = Console.ReadLine()!;

                        int index;
                        if (!int.TryParse(input, out index) || index < 1 || index >= results.Count)
                        {
                            Console.WriteLine("Invalid index. Try again!");
                            continue;
                        }

                        GameList.Add(results[index - 1]);
                        break;
                    }
                }
                else
                {
                    Console.WriteLine("No results found!");
                    Console.ReadKey();
                }
            }

            Console.Clear();
            Console.WriteLine("{0} games added.", GameList.Count);
        }

        private static void BeginListening()
        {
            var rand = new Random();
            Console.WriteLine("Press enter key to show a random review.");

            while (GameList.Count > 0)
            {
                ConsoleKeyInfo input = Console.ReadKey();
                if (input.Key == ConsoleKey.Escape)
                {
                    break;
                }

                if (input.Key == ConsoleKey.Enter)
                {
                    Console.Clear();
                    int index = rand.Next(0, GameList.Count);
                    AppInfo app = GameList[index];

                    index = rand.Next(0, app.Reviews.Count);
                    Review review = app.Reviews[index];

                    Console.WriteLine("Game: {0}", app.Name);
                    Console.WriteLine();
                    Console.WriteLine(review.Content);
                }
            }
        }

        public static async Task DownloadGameReviewsAsync(AppInfo app, string lang)
        {
            var reviews = new List<Review>(STEAM_MAX_REVIEWS);
            string cursor = "*";

            Console.WriteLine("Fetching reviews [{0}] \"{1}\"", app.Id, app.Name);

            while (reviews.Count < STEAM_MAX_REVIEWS)
            {
                Response response = await MakeRequestAsync(app.Id, lang, cursor);
                if (response != null)
                {
                    if (response.Reviews.Count == 0)
                    {
                        break;
                    }

                    reviews.AddRange(response.Reviews);
                    cursor = response.Cursor;
                    Console.WriteLine("Got {0} reviews. Total: {1}", response.Reviews.Count, reviews.Count);
                }
            }

            app.Reviews = reviews;
        }

        private static async Task<Response> MakeRequestAsync(int appid, string lang, string cursor)
        {
            using var client = new HttpClient();
            client.BaseAddress = new Uri(STEAM_API_ENDPOINT);

            var queryParams = new Dictionary<string, string>
            {
                { "json", "1" },
                { "filter", "recent" },
                { "language", lang },
                { "cursor", cursor },
                { "review_type", "all" },
                { "purchase_type", "all" },
                { "num_per_page", "100" },
                { "filter_offtopic_activity", "0" }
            };

            using var encoder = new FormUrlEncodedContent(queryParams);
            string query = await encoder.ReadAsStringAsync();

            Debug.WriteLine($"GET {STEAM_API_ENDPOINT}?{query}");

            string json = await client.GetStringAsync($"{appid}?{query}");
            return DeserializeSteamJson(json);
        }

        private static Response DeserializeSteamJson(string json)
        {
            var options = new JsonSerializerOptions();
            options.PropertyNameCaseInsensitive = true;
            return JsonSerializer.Deserialize<Response>(json, options)!;
        }
    }
}
