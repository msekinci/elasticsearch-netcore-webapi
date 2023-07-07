using System.Net;
using Elasticsearch.API.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace Elasticsearch.API.Controllers
{
    [Route("api/[controller]")]
    public class BaseController : ControllerBase
    {
        [NonAction]
        public IActionResult CreateActionResult<T>(ResponseDto<T> response) {
            return new ObjectResult(response.Status == HttpStatusCode.NoContent ? null : response)
            {
                StatusCode = response.Status.GetHashCode()
            };
        }
    }
}

