using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace SteamReviewsFetcher.Steam.Entities
{
    internal sealed class AppInfo
    {
        [JsonPropertyName("steam_appid")]
        public int Id { get; set; }

        public string Name { get; set; }

        public IList<Review> Reviews { get; set; }
    }
}
