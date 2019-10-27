using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;

namespace Punc.Api.ViewModels
{
    public class CreateTimerRequest
    {
        /// <summary>
        /// Email of the customer
        /// </summary>
        public string CustomerEmail { get; set; }

        /// <summary>
        /// Name of the customer if provided
        /// </summary>
        public string CustomerName { get; set; }


        /// <summary>
        /// Starting location
        /// </summary>
        public string Origin { get; set; }

        /// <summary>
        /// Address of the destiation
        /// </summary>
        public string Destination { get; set; }

        /// <summary>
        /// Transport method to arrive to destination
        /// </summary>
        public TravelMode TravelMode { get; set; }

        /// <summary>
        /// Time required to arrive at destination
        /// </summary>
        public DateTime ArrivalTimeUtc { get; set; }

        /// <summary>
        /// Bool for if expert mode is selected
        /// </summary>
        public bool ExpertMode { get; set; } = false;

        /// <summary>
        /// Payment intent used
        /// </summary>
        public string PaymentIntentId { get; set; }

        public TimerConfirmationMethod ConfirmationMethod { get; set; } = TimerConfirmationMethod.None;

        /// <summary>
        /// Email address of referee to email for confirmation
        /// </summary>
        public string RefereeEmail { get; set; }
    }
}
