using System.Collections.Immutable;
using Elasticsearch.API.Models;
using Nest;

namespace Elasticsearch.API.Repositories
{
    public class ProductRepository
	{
		private readonly ElasticClient _client;
        private const string indexName = "products";

        public ProductRepository(ElasticClient client)
        {
            _client = client;
        }

        public async Task<Product?> SaveAsync(Product newProduct)
        {
            newProduct.Created = DateTime.Now;

            var response = await _client.IndexAsync(newProduct, x => x.Index(indexName).Id(Guid.NewGuid().ToString()));
            if (!response.IsValid) return null;

            newProduct.Id = response.Id;
            return newProduct;
        }

        public async Task<ImmutableList<Product>> GetAllAsync()
        {
            var result = await _client
                .SearchAsync<Product>(s => s.Index(indexName)
                                            .Query(q => q.MatchAll()));

            foreach (var hits in result.Hits)
            {
                hits.Source.Id = hits.Id;
            }

            return result.Documents.ToImmutableList();
        }
    }
}

