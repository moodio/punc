using Microsoft.AspNetCore.Mvc;
using Punc.Api.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Punc.Api.Controllers
{
    [ApiController()]
    [Route("api/[controller]")]
    public class AutoCompleteController : ControllerBase
    {
        private readonly IAutoCompleteService _autocompleteService;

        public AutoCompleteController(IAutoCompleteService autocompleteService)
        {
            _autocompleteService = autocompleteService;
        }

        [HttpGet("sessionkey")]
        public IActionResult GetSessionKey()
        {
            return new ObjectResult(Guid.NewGuid());
        }

        [HttpGet("search")]
        public async Task<IActionResult> Search([FromQuery]string key, string q, int callNumber)
        {
            if (string.IsNullOrEmpty(key))
            {
                return new ObjectResult(string.Empty);
            }

            var res = await _autocompleteService.Search(key, q);

            var vm = new AutoCompleteResultViewModel()
            {
                CallNumber = callNumber,
                Predictions = res
            };

            return new JsonResult(vm);
        }
    }
}
