using SteamReviewsFetcher.Steam.Converters;
using System.Text.Json.Serialization;

namespace SteamReviewsFetcher.Steam.Entities
{
    internal sealed class Author
    {
        [JsonPropertyName("steamid")]
        [JsonConverter(typeof(StringToLongConverter))]
        public long Id { get; set; }

        [JsonPropertyName("num_games_owned")]
        public int GamesOwned { get; set; }

        [JsonPropertyName("num_reviews")]
        public int NumReviews { get; set; }

        [JsonPropertyName("playtime_forever")]
        public int PlaytimeForever { get; set; }

        [JsonPropertyName("playtime_at_review")]
        public int PlaytimeAtReview { get; set; }
    }
}
