using Microsoft.Extensions.Configuration;
using Stripe;
using System;
using System.Collections.Generic;
using System.Net.Mail;
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

        public async Task<bool> CapturePaymentIntentAsync(string paymentIntentId, MailAddress receiptEmail)
        {
            var paymentService = new PaymentIntentService();

            //add the receipt email
            var emailRes = await AddIntentReceiptEmail(paymentIntentId, receiptEmail);

            var options = new PaymentIntentCaptureOptions()
            {
                StatementDescriptorSuffix = "NowLeave.com"
            };

            try
            {
                var res = await paymentService.CaptureAsync(paymentIntentId, options, GetRequestOptions());
                return res.Status.Equals("succeeded", StringComparison.InvariantCultureIgnoreCase);
            }
            catch (Exception e)
            {
                return false;
            }
        }


        /// <summary>
        /// Creates a payment intent and returns the payment intent
        /// </summary>
        /// <param name="amount_cents"></param>
        /// <returns></returns>
        public async Task<PaymentResult> CreatePaymentIntentAsync(string paymentMethodId, int amount_cents = 300)
        {
            var paymentIntentService = new PaymentIntentService();
            
            var options = new PaymentIntentCreateOptions()
            {
                Amount = amount_cents,
                Confirm = true,
                Currency = "usd",
                CaptureMethod = "manual",
                ConfirmationMethod = "manual",
                PaymentMethodId = paymentMethodId,
                StatementDescriptor = "NowLeave.com"
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

        public async Task<bool> ReleasePaymentIntentAsync(string paymentIntentId)
        {
            var paymentService = new PaymentIntentService();
            var options = new PaymentIntentCancelOptions()
            {
                CancellationReason = "abandoned"
            };
            var res = await paymentService.CancelAsync(paymentIntentId,options, GetRequestOptions());
            return res.Status.Equals("canceled");
        }

        /// <summary>
        /// validate a payment id
        /// </summary>
        /// <param name="paymentIntentId"></param>
        /// <returns></returns>
        public async Task<bool> ValidatePaymentIntentAsync(string paymentIntentId)
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

        /// <summary>
        /// Add receipt email to an existing intent
        /// </summary>
        /// <param name="paymentIntentId"></param>
        /// <param name="receiptEmail"></param>
        /// <returns></returns>
        private async Task<bool> AddIntentReceiptEmail(string paymentIntentId, MailAddress receiptEmail)
        {
            var service = new PaymentIntentService();
            var options = new PaymentIntentUpdateOptions()
            {
                ReceiptEmail = receiptEmail.Address
            };
            var res = await service.UpdateAsync(paymentIntentId, options, GetRequestOptions());

            return true;
        }

        private RequestOptions GetRequestOptions()
        {
            //setup the request options
           return new RequestOptions() { ApiKey = _stripeClientSecret };
        }
    }
}
