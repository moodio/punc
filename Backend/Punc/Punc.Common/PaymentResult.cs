using System;
using System.Collections.Generic;
using System.Text;

namespace Punc
{
    public class PaymentResult
    {
        public bool Success { get; set; }

        public string ClientSecret { get; set; }

        public string PaymentIntentId { get; set; }

        public bool RequiresClientAction { get; set; }

    }
}
