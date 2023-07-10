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

            foreach (var item in result.Hits)
            {
                item.Source!.Id = item.Id;
            }

            return result.Documents.ToImmutableList();
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
            

            foreach (var item in result.Hits)
            {
                item.Source!.Id = item.Id;
            }

            return result.Documents.ToImmutableList();
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

            foreach (var item in result.Hits)
            {
                item.Source!.Id = item.Id;
            }

            return result.Documents.ToImmutableList();
        }
    }
}

