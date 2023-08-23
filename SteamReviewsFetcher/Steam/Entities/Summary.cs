using System.Text.Json.Serialization;

namespace SteamReviewsFetcher.Steam.Entities
{
    internal sealed class Summary
    {
        [JsonPropertyName("num_reviews")]
        public int NumReviews { get; set; }

        [JsonPropertyName("review_score")]
        public int ReviewScore { get; set; }

        [JsonPropertyName("review_score_desc")]
        public string ReviewDescription { get; set; }

        [JsonPropertyName("total_positive")]
        public int TotalPositive { get; set; }

        [JsonPropertyName("total_negative")]
        public int TotalNegative { get; set; }

        [JsonPropertyName("total_reviews")]
        public int TotalReviews { get; set; }
    }
}
