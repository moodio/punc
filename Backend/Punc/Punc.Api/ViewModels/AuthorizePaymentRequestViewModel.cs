using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Punc.Api.ViewModels
{
    public class AuthorizePaymentRequestViewModel
    {
        public string PaymentMethodId { get; set; }

        public int Amount = 200;

        public string Currency = "usd";
    }
}
