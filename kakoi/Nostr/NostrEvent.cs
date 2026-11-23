using System.Text.Json.Serialization;
using NNostr.Client.JsonConverters;

namespace NNostr.Client
{
    public class NostrEvent
    {
        [JsonPropertyName("id")]
        public string Id { get; set; } = string.Empty;

        [JsonPropertyName("pubkey")]
        public string PublicKey { get; set; } = string.Empty;

        [JsonPropertyName("created_at")]
        [JsonConverter(typeof(UnixTimestampSecondsJsonConverter))]
        public DateTimeOffset? CreatedAt { get; set; }

        [JsonPropertyName("kind")]
        public int Kind { get; set; }

        [JsonPropertyName("content")]
        [JsonConverter(typeof(StringEscaperJsonConverter))]
        public string? Content { get; set; }

        [JsonPropertyName("tags")]
        public List<NostrEventTag> Tags { get; set; } = [];

        [JsonPropertyName("sig")]
        public string Signature { get; set; } = string.Empty;
    }
}
