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
        /// Starting location
        /// </summary>
        public string Location { get; set; }

        /// <summary>
        /// Address of the destiation
        /// </summary>
        public string Destination { get; set; }

        /// <summary>
        /// Transport method to arrive to destination
        /// </summary>
        public TravelMode TransportMode { get; set; }

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

        public string RefereeEmail { get; set; }
    }
}
