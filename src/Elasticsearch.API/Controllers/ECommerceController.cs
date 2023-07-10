using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Elasticsearch.API.DTOs;
using Elasticsearch.API.Models.ECommerceModel;
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


    }
}

