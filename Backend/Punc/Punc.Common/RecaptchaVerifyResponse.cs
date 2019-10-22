using System;
using System.Collections.Generic;
using System.Text;

namespace Punc
{
    public class RecaptchaVerifyResponse
    {
        public bool Success { get; set; }

        public string Hostname { get; set; }

        public decimal Score { get; set; }

        public string Action { get; set; }
    }
}
