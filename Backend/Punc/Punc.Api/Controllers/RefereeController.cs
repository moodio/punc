using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Punc.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RefereeController : ControllerBase
    {
        private readonly ITimersService _timersService;
        public RefereeController(ITimersService timersService)
        {
            _timersService = timersService;
        }

        [HttpGet("timer/{code}")]
        public async Task<IActionResult> GetRefereeTimer([FromRoute] string code)
        {
            var res = await _timersService.GetRefereeTimerAsync(code);

            if (res == null)
                return new NotFoundResult();

            return new JsonResult(res);
        }

        [HttpPost("timer/{code}")]
        public async Task<IActionResult> ConfirmOntime([FromRoute]string code, [FromQuery]bool onTime)
        {
            var res = await _timersService.ConfirmTimerOnTimeStatusAsync(code, onTime);

            if (res)
            {
                return new OkResult();
            }
            else
            {
                return new StatusCodeResult(400);
            }

        }
    }
}
