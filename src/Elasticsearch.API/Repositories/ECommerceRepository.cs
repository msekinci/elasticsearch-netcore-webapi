using System;
using System.Collections.Immutable;
using Elastic.Clients.Elasticsearch;
using Elastic.Clients.Elasticsearch.QueryDsl;
using Elasticsearch.API.Models.ECommerceModel;

namespace Elasticsearch.API.Repositories
{
	public class ECommerceRepository
	{
		private readonly ElasticsearchClient _client;
        private const string indexName = "kibana_sample_data_ecommerce";

        public ECommerceRepository(ElasticsearchClient client)
        {
            _client = client;
        }

        public async Task<ImmutableList<ECommerce>?> TermQuery(string customerFirstName)
        {
            // first way
            //var result = await _client
            //    .SearchAsync<ECommerce>(s =>
            //        s.Index(indexName).Query(q =>
            //            q.Term(t =>
            //                t.Field("customer_first_name.keyword").Value(customerFirstName))));

            // second way
            //var result = await _client
            //    .SearchAsync<ECommerce>(s =>
            //        s.Index(indexName).Query(q =>
            //            q.Term(t =>
            //                t.CustomerFirstName.Suffix("keyword"),customerFirstName)));

            //third way
            var termQuery = new TermQuery("customer_first_name.keyword")
            {
                Value = customerFirstName,
                CaseInsensitive = true
            };
            var result = await _client.SearchAsync<ECommerce>(s => s.Index(indexName).Query(termQuery));

            return ConvertImmutableList(result);
        }

        public async Task<ImmutableList<ECommerce>?> TermsQuery(List<string> customerFirstNameList)
        {
            List<FieldValue> terms = new List<FieldValue>();
            customerFirstNameList.ForEach(x => terms.Add(x));


            //1.way
            //var termsQuery = new TermsQuery()
            //{
            //    Field = "customer_first_name.keyword",
            //    Terms = new TermsQueryField(terms.AsReadOnly())
            //};

            //var result = await _client.SearchAsync<ECommerce>(s => s.Index(indexName).Query(termsQuery));

            //2.way
            var result = await _client.SearchAsync<ECommerce>(s => s
                .Index(indexName)
                .Query(q => q
                    .Terms(t => t
                        .Field(f => f.CustomerFirstName
                            .Suffix("keyword"))
                            .Terms(new TermsQueryField(terms.AsReadOnly())))));


            return ConvertImmutableList(result);
        }

        public async Task<ImmutableList<ECommerce>> PrefixQuery(string customerFullName)
        {
            var result = await _client.SearchAsync<ECommerce>(s => s
                .Index(indexName)
                .Query(q => q
                    .Prefix(p => p
                        .Field(f => f
                            .CustomerFullName.Suffix("keyword"))
                        .CaseInsensitive(true)
                        .Value(customerFullName))));

            return ConvertImmutableList(result);
        }

        public async Task<ImmutableList<ECommerce>> RangeQuery(double fromPrice, double toPrice)
        {
            var result = await _client.SearchAsync<ECommerce>(s => s
                .Index(indexName)
                .Query(q => q
                    .Range(r => r
                        .NumberRange(nr => nr
                            .Field(f => f.TaxfulTotalPrice).Gte(fromPrice).Lte(toPrice)))));

            return ConvertImmutableList(result);
        }

        public async Task<ImmutableList<ECommerce>> MatchAllQuery()
        {
            var result = await _client.SearchAsync<ECommerce>(s => s
                .Index(indexName)
                .Query(q => q
                    .MatchAll()));

            return ConvertImmutableList(result);
        }

        public async Task<ImmutableList<ECommerce>> PaginationQuery(int page, int pageSize)
        {
            var pageFrom = (page - 1) * pageSize;
            var result = await _client.SearchAsync<ECommerce>(s => s
                .Index(indexName)
                .Size(pageSize)
                .From(pageFrom)
                .Query(q => q
                    .MatchAll()));

            return ConvertImmutableList(result);
        }

        public async Task<ImmutableList<ECommerce>> WildcarQuery(string customerFullName)
        {
            var result = await _client.SearchAsync<ECommerce>(s => s
                .Index(indexName)
                .Query(q => q
                    .Wildcard(w => w
                        .Field(f => f
                            .CustomerFullName.Suffix("keyword"))
                        .CaseInsensitive(true)
                        .Wildcard(customerFullName))));

            return ConvertImmutableList(result);
        }

        private ImmutableList<ECommerce> ConvertImmutableList(SearchResponse<ECommerce> result)
        {
            foreach (var item in result.Hits)
            {
                item.Source!.Id = item.Id;
            }

            return result.Documents.ToImmutableList();
        }
    }
}

