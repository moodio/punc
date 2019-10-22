using Microsoft.AspNetCore.Mvc;
using Punc.Api.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Punc.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TimersController : ControllerBase
    {
        private readonly RecaptchaVerifyService _recaptchaService;
        private readonly ITimersService _timersService;
        

        public TimersController(RecaptchaVerifyService recaptchaService, ITimersService timersService)
        {
            _recaptchaService = recaptchaService;
            _timersService = timersService;
        }

        [HttpPost()]
        public async Task<IActionResult> CreateTimer([FromBody]CreateTimerRequestViewModel req)
        {
            //validate the request
            var val = req.Validate(null);
            if (val.Any())
            {
                return new BadRequestObjectResult(val);
            }

            //verify token
            var tokenValid = await _recaptchaService.VerifyToken(req.VerifyToken, "starttimer");
            if(!tokenValid){
                return new BadRequestObjectResult(new { error = "Invalid recaptcha token" });
            }

            //create the timer
            var timer = await _timersService.CreateTimerAsync(req);

            //return result based on status
            if(timer.Status == TimerStatus.Active)
            {
                return new OkObjectResult(timer);
            }
            else
            {
                switch (timer.Errors)
                {
                    case TimerErrors.PaymentError:
                        return new BadRequestObjectResult(new { error = "Invalid payment information" });
                    case TimerErrors.RouteError:
                        return new BadRequestObjectResult(new { error = "Unable to find route between locations." });
                    default:
                        return new StatusCodeResult(500);
                }
            }
        }
    
        /// <summary>
        /// Get a timer by it's id
        /// </summary>
        /// <param name="timerId"></param>
        /// <returns></returns>
        [HttpGet("{timerId}")]
        public async Task<IActionResult> GetTimer([FromRoute]Guid timerId)
        {
            var res = await _timersService.GetTimerAsync(timerId);
            if (res != null)
            {
                return new OkObjectResult(res);
            }
            else
            {
                return new StatusCodeResult(404);
            }
        }
    
    }
}
