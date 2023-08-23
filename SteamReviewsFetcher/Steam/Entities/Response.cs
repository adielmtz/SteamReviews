using SteamReviewsFetcher.Steam.Converters;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace SteamReviewsFetcher.Steam.Entities
{
    internal sealed class Response
    {
        [JsonConverter(typeof(NumberToBoolConverter))]
        public bool Success { get; set; }

        public AppInfo Data { get; set; }

        [JsonPropertyName("query_summary")]
        public Summary QuerySummary { get; set; }

        public IList<Review> Reviews { get; set; }

        public string Cursor { get; set; }
    }
}
