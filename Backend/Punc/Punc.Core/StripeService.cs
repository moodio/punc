using Microsoft.Extensions.Configuration;
using Stripe;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Punc
{
    public class StripeService
    {
        private readonly string _stripeClientSecret;
        private PaymentIntentService _paymentIntentService;

        public StripeService(IConfiguration config)
        {
            _stripeClientSecret = config["Stripe:ClientSecret"];
        }

        /// <summary>
        /// Creates a payment intent and returns the payment intent
        /// </summary>
        /// <param name="amount_cents"></param>
        /// <returns></returns>
        public async Task<PaymentResult> CreatePaymentIntent(string paymentMethodId, int amount_cents = 200)
        {
            var paymentIntentService = new PaymentIntentService();
            
            var options = new PaymentIntentCreateOptions()
            {
                Amount = amount_cents,
                Confirm = true,
                Currency = "usd",
                CaptureMethod = "manual",
                ConfirmationMethod = "manual",
                PaymentMethodId = paymentMethodId
            };

            //setup the request options
            var intent = await paymentIntentService.CreateAsync(options, GetRequestOptions());

            var requiresClientAction = intent.Status == "requires_action" 
                                        && intent.NextAction.Type == "use_stripe_sdk";

            var res = new PaymentResult()
            {
                Success = requiresClientAction == true
                            || intent.Status == "requires_capture"
                            || intent.Status == "succeeded",
                ClientSecret = intent.ClientSecret,
                PaymentIntentId = intent.Id,
                RequiresClientAction = requiresClientAction
            };

            return res;
        }


        /// <summary>
        /// validate a payment id
        /// </summary>
        /// <param name="paymentIntentId"></param>
        /// <returns></returns>
        public async Task<bool> ValidatePaymentIntent(string paymentIntentId)
        {
            var paymentService = new PaymentIntentService();

            try
            {
                var res = await paymentService.GetAsync(paymentIntentId, requestOptions: GetRequestOptions());

                return (res != null);
            } catch(Exception e)
            {
                return false;
            }
        }


        private RequestOptions GetRequestOptions()
        {
            //setup the request options
           return new RequestOptions() { ApiKey = _stripeClientSecret };
        }
    }
}
