using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Punc.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HealthController : ControllerBase
    {
        IConfiguration _config;

        public HealthController(IConfiguration config)
        {
            _config = config;
        }

        [HttpGet()]
        public IActionResult GetHealth()
        {
            var echo = _config["Health:Echo"] ?? "none";
            var res = new
            {
                HealthEcho = echo
            };

            return new JsonResult(res);
        }
    }
}
