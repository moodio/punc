using System;
using System.Collections.Generic;
using System.Text;

namespace Punc
{
    public enum TimerStatus
    {
        Active = 1,
        AwaitingConfirmation = 2,
        Ontime = 4,
        Late = 8,
        Cancelled = 64,
        Failed = 128
    }

    [Flags]
    public enum TimerConfirmationMethod
    {
        None = 0,
        Gps = 1,
        LinkConfirmation = 2
    }

    [Flags]
    public enum TimerErrors
    {
        None = 0,
        RouteError = 1,
        PaymentError = 2,
        ServerError = 128
    }

    public class Timer
    {
        /// <summary>
        /// id of the timer
        /// </summary>
        public Guid Id { get; set; }

        public TimerStatus Status { get; set; }
        
        /// <summary>
        /// time required to at destination unix epoch utc time
        /// </summary>
        public int ArrivalTimeEpoch { get; set; }

        /// <summary>
        /// Time required to arrive at destination (UTC)
        /// </summary>
        public DateTime ArrivalTimeUtc { get; set; }

        /// <summary>
        /// How to confirm location
        /// </summary>
        public TimerConfirmationMethod ConfirmationMethod { get; set; }

        /// <summary>
        /// Time of the last update of journey time
        /// </summary>
        public DateTime LastUpdate { get; set; }

        /// <summary>
        /// Starting location
        /// </summary>
        public string Location { get; set; }

        /// <summary>
        /// Address of the destiation
        /// </summary>
        public string Destination { get; set; }

        /// <summary>
        /// Departure time in Epoch (Seconds/UTC)
        /// </summary>
        public int DepartureTimeEpoch { get; set; }

        /// <summary>
        /// Estimated arrival time based on route
        /// </summary>
        public DateTime EstimatedArrivalTimeUtc { get; set; }

        /// <summary>
        /// Estimated arrival in unix time epoch
        /// </summary>
        public int EstimatedArrivalTimeEpoch { get; set; }

        /// <summary>
        /// Bool for if expert mode is selected
        /// </summary>
        public bool ExpertMode { get; set; } = false;

        /// <summary>
        /// Transport method to arrive to destination
        /// </summary>
        public TravelMode TransportMethod { get; set; }

        /// <summary>
        /// Distance in meters of journey
        /// </summary>
        public int TravelDistance { get; set; }

        /// <summary>
        /// Travel time in minutes required to travel to destination
        /// </summary>
        public int TravelDuration { get; set; }

        /// <summary>
        /// The id of the payment intent used
        /// </summary>
        public string PaymentIntentId { get; set; }


        /// <summary>
        /// List of errors if timer has failed
        /// </summary>
        public TimerErrors Errors { get; set; } = TimerErrors.None;
    }
}
