using System.Text.Json.Serialization;

namespace Elasticsearch.API.Models
{
    public class ElasticBase
	{
        [JsonPropertyName("_id")]
        public string Id { get; set; } = null!;
    }
}

