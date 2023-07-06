using Elasticsearch.Net;
using Nest;

namespace Elasticsearch.API.Extensions
{
    public static class ElasticsearchExt
	{
		public static void AddElastic(this IServiceCollection services, IConfiguration configuration)
        {
            var url = configuration.GetSection("Elasticsearch")["Url"]!;
            var username = configuration.GetSection("Elasticsearch")["Username"]!;
            var password = configuration.GetSection("Elasticsearch")["Password"]!;

            var pool = new SingleNodeConnectionPool(new Uri(url));
            var settings = new ConnectionSettings(pool);
            settings.BasicAuthentication(username, password);
            var client = new ElasticClient(settings);
            services.AddSingleton(client);
        }
    }
}

