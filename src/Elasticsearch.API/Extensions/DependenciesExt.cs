using Elasticsearch.API.Repositories;
using Elasticsearch.API.Services;

namespace Elasticsearch.API.Extensions
{
    public static class DependenciesExt
	{
		public static void AddDependencies(this IServiceCollection services)
		{
			services.AddScoped<ProductService>();
			services.AddScoped<ProductRepository>();
		}
	}
}

