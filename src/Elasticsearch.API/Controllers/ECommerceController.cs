using Elasticsearch.API.Repositories;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Elasticsearch.API.Controllers
{
    [Route("api/[controller]/[action]")]
    public class ECommerceController : Controller
    {
        private readonly ECommerceRepository _repository;

        public ECommerceController(ECommerceRepository repository)
        {
            _repository = repository;
        }

        [HttpGet]
        public async Task<IActionResult> TermQuery(string customerFirstName) {
            return Ok(await _repository.TermQuery(customerFirstName));
        }

        [HttpGet]
        public async Task<IActionResult> TermsQuery(List<string> customerFirstNames) {
            return Ok(await _repository.TermsQuery(customerFirstNames));
        }

        [HttpGet]
        public async Task<IActionResult> PrefixQuery(string customerFullName) {
            return Ok(await _repository.PrefixQuery(customerFullName));
        }

        [HttpGet]
        public async Task<IActionResult> RangeQuery(double fromPrice, double toPrice)
        {
            return Ok(await _repository.RangeQuery(fromPrice, toPrice));
        }

        [HttpGet]
        public async Task<IActionResult> MatchAllQuery()
        {
            return Ok(await _repository.MatchAllQuery());
        }

        [HttpGet]
        public async Task<IActionResult> PaginationQuery(int page = 1, int pageSize = 10)
        {
            return Ok(await _repository.PaginationQuery(page, pageSize));
        }

        //Ge*ge or Geo?ge
        [HttpGet]
        public async Task<IActionResult> WildcardQuery(string customerFullName)
        {
            return Ok(await _repository.WildcarQuery(customerFullName));
        }
    }
}

