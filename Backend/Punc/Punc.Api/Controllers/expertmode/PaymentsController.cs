using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Punc.Api.Controllers.expertmode
{
    [ApiController]
    [Route("api/expertmode/[controller]")]
    public class PaymentsController : ControllerBase
    {
        private StripeService _stripeService;

        public PaymentsController(StripeService stripeService)
        {
            _stripeService = stripeService;
        }

        [HttpPost("authorize")]
        public async Task<IActionResult> AuthorizePayment([FromBody]string paymentMethodId)
        {
            var res = await _stripeService.CreatePaymentIntentAsync(paymentMethodId);
            return new OkObjectResult(res);
        }
    }
}
