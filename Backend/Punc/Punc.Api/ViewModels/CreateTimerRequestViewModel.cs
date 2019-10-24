﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;

namespace Punc.Api.ViewModels
{
    public class CreateTimerRequestViewModel : CreateTimerRequest, IValidatableObject
    { 
        /// <summary>
        /// Google recaptcha token
        /// </summary>
        public string VerifyToken { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var res = new List<ValidationResult>();

            if (String.IsNullOrEmpty(this.VerifyToken))
            {
                res.Add(new ValidationResult("Recaptcha token not provided"));
            }

            if (String.IsNullOrEmpty(this.Location) || this.Location.Length < 10)
            {
                res.Add(new ValidationResult("Invalid starting location"));
            }

            if (String.IsNullOrEmpty(this.Destination) || this.Destination.Length < 10)
            {
                res.Add(new ValidationResult("Invalid destination"));
            }

            if (this.ArrivalTimeUtc.ToUniversalTime() < DateTime.UtcNow)
            {
                res.Add(new ValidationResult("Arrival time cannot be in the past Dr Who!"));
            }

            if (this.ArrivalTimeUtc.ToUniversalTime() < DateTime.UtcNow.AddMinutes(5f))
            {
                res.Add(new ValidationResult("Must leave atleast 5 minutes between current time and arrival time"));
            }

            if (this.ExpertMode)
            {
                if(String.IsNullOrWhiteSpace(this.PaymentIntentId))
                {
                    res.Add(new ValidationResult("Expert mode requires payment and confirmation!"));
                }
                
                if(this.ConfirmationMethod == TimerConfirmationMethod.LinkConfirmation)
                {
                    if (String.IsNullOrEmpty(this.RefereeEmail))
                    {
                        res.Add(new ValidationResult("Must include referee email for link confirmed timers"));
                    }

                    try
                    {
                        var _ = new MailAddress(this.RefereeEmail);
                    }
                    catch
                    {
                        res.Add(new ValidationResult("Invalid email address"));
                    }
                }
            }

            return res;
        }
    }
}
