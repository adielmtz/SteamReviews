using SteamReviewsFetcher.Steam.Converters;
using System.Text.Json.Serialization;

namespace SteamReviewsFetcher.Steam.Entities
{
    internal sealed class Review
    {
        [JsonPropertyName("recommendationid")]
        [JsonConverter(typeof(StringToLongConverter))]
        public long Id { get; set; }

        public Author Author { get; set; }

        public string Language { get; set; }

        [JsonPropertyName("review")]
        public string Content { get; set; }

        [JsonPropertyName("voted_up")]
        [JsonConverter(typeof(NumberToBoolConverter))]
        public bool VotedUp { get; set; }

        [JsonPropertyName("votes_up")]
        public int VotesUp { get; set; }
    }
}
