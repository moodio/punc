using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Punc.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SubscriptionsController : ControllerBase
    {
        private readonly ISubscribersService _subscribersService;

        public SubscriptionsController(ISubscribersService subscriberService)
        {
            _subscribersService = subscriberService;
        }

        [HttpPost("subscribe")]
        public async Task<IActionResult> Subscribe([FromQuery] string email)
        {
            try
            {
                var e = new MailAddress(email);
            }
            catch
            {
                return BadRequest();
            }

            var res = await _subscribersService.Subscribe(email);
            if (res)
            {
                return new OkResult();
            }
            else
            {
                return new StatusCodeResult(500);
            }
        }

        [HttpPost("unsubscribe")]
        public async Task<IActionResult> Unsubscribe([FromQuery] string email)
        {
            try
            {
                var e = new MailAddress(email);
            }
            catch
            {
                return BadRequest();
            }

            var res = await _subscribersService.Unsubscribe(email);
            if (res)
            {
                return new OkResult();
            }
            else
            {
                return new StatusCodeResult(500);
            }
        }


    }
}
