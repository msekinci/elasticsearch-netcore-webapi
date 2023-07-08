﻿using System.Collections.Immutable;
using Elastic.Clients.Elasticsearch;
using Elasticsearch.API.DTOs;
using Elasticsearch.API.Models;

namespace Elasticsearch.API.Repositories
{
    public class ProductRepository
	{
		private readonly ElasticsearchClient _client;
        private const string indexName = "products";

        public ProductRepository(ElasticsearchClient client)
        {
            _client = client;
        }

        public async Task<Product?> SaveAsync(Product newProduct)
        {
            newProduct.Created = DateTime.Now;

            var response = await _client.IndexAsync(newProduct, x => x.Index(indexName).Id(Guid.NewGuid().ToString()));
            if (!response.IsValidResponse) return null;

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
                hits!.Source!.Id = hits.Id;
            }

            return result.Documents.ToImmutableList();
        }

        public async Task<Product?> GetByIdAsync(string id)
        {
            var response = await _client.GetAsync<Product>(id, x => x.Index(indexName));
            if (!response.IsValidResponse)
            {
                return null;
            }

            response!.Source!.Id = response.Id;
            return response.Source;
        }

        public async Task<UpdateResponse<Product>?> UpdateAsync(ProductUpdateDto updateProduct) {
            var response = await _client
                .UpdateAsync<Product, ProductUpdateDto>(indexName, updateProduct.id, x => x.Doc(updateProduct));

            return response;
        }

        public async Task<DeleteResponse> DeleteAsync(string id)
        {
            var response = await _client
                .DeleteAsync<Product>(id, x => x.Index(indexName));

            return response;
        }
    }
}

