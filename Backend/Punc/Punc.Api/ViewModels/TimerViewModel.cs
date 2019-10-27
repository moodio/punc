using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;

namespace Punc.Api.ViewModels
{
    public class TimerViewModel
    {
        /// <summary>
        /// id of the timer
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Status of the timer
        /// </summary>
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
        /// Address of the destiation
        /// </summary>
        public string Destination { get; set; }

        /// <summary>
        /// Departure time in Epoch (Seconds/UTC)
        /// </summary>
        public int DepartureTimeEpoch { get; set; }

        public DateTime DepartureTimeUtc { get; set; }

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
        /// Starting location
        /// </summary>
        public string Origin { get; set; }

        /// <summary>
        /// Transport method to arrive to destination
        /// </summary>
        public TravelMode TravelMode { get; set; }

        /// <summary>
        /// Distance in meters of journey
        /// </summary>
        public int TravelDistance { get; set; }

        /// <summary>
        /// Travel time in minutes required to travel to destination
        /// </summary>
        public int TravelDuration { get; set; }


        /// <summary>
        /// List of errors if timer has failed
        /// </summary>
        public TimerErrors Errors { get; set; } = TimerErrors.None;

        public static explicit operator TimerViewModel(Timer timer)
        {
            var res = new TimerViewModel()
            {
                Id = timer.Id,
                ArrivalTimeEpoch = timer.ArrivalTimeUtc.ToUnixEpoch(),
                ArrivalTimeUtc = timer.ArrivalTimeUtc,
                ConfirmationMethod = timer.ConfirmationMethod,
                Destination = timer.Destination,
                DepartureTimeEpoch = timer.DepartureTimeUtc.ToUnixEpoch(),
                DepartureTimeUtc = timer.DepartureTimeUtc,
                Errors = timer.Errors,
                ExpertMode = timer.ExpertMode,
                EstimatedArrivalTimeEpoch = timer.EstimatedArrivalTimeUtc.ToUnixEpoch(),
                EstimatedArrivalTimeUtc = timer.EstimatedArrivalTimeUtc,
                LastUpdate = timer.LastUpdateUtc,
                Origin = timer.Origin,
                Status = timer.Status,
                TravelMode = timer.TravelMode,
                TravelDuration = timer.TravelDuration,
                TravelDistance = timer.TravelDistance
            };

            return res;
        }
    }
}
