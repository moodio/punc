﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Net.Mail;
using System.Text;

namespace Punc
{
    public enum TimerStatus
    {
        None = 0,
        Active = 1,
        TimeToLeave = 2,
        Enroute = 4,
        AwaitingConfirmation = 16,
        OnTime = 32,
        ConfirmedLate = 64,
        UnconfirmedLate = 128,
        Closed = 512, 
        Cancelled = 1024,
        Failed = 2048
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
        RefereeEmailSendError = 4,
        ServerError = 128
    }

    public class Timer
    {
        /// <summary>
        /// How long before departure time in seconds is it considered time to leave
        /// </summary>
        private static TimeSpan leaveTimeBuffer = TimeSpan.FromMinutes(5);

        /// <summary>
        /// id of the timer
        /// </summary>
        public Guid Id { get; set; }

        public TimerStatus Status
        {
            get => GetStatus();
            set => _status = value;
        }

        /// <summary>
        /// Time required to arrive at destination (UTC)
        /// </summary>
        public DateTime ArrivalTimeUtc { get; set; }

        /// <summary>
        /// How to confirm location
        /// </summary>
        public TimerConfirmationMethod ConfirmationMethod { get; set; }


        public MailAddress CustomerEmail { get; set; }

        public string CustomerName { get; set; }

        /// <summary>
        /// Address of the destiation
        /// </summary>
        public string Destination { get; set; }

        /// <summary>
        /// Departure time in Epoch (Seconds/UTC)
        /// </summary>
        public DateTime DepartureTimeUtc { get; set; }

        /// <summary>
        /// Estimated arrival time based on route
        /// </summary>
        public DateTime EstimatedArrivalTimeUtc { get; set; }

        /// <summary>
        /// Bool for if expert mode is selected
        /// </summary>
        public bool ExpertMode { get; set; } = false;

        /// <summary>
        /// Time of the last update of journey time
        /// </summary>
        public DateTime LastUpdateUtc { get; set; }

        /// <summary>
        /// Starting location
        /// </summary>
        public string Origin { get; set; }

        /// <summary>
        /// Email address of the referee
        /// </summary>
        public MailAddress RefereeEmail { get; set; }

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
        /// The id of the payment intent used
        /// </summary>
        public string PaymentIntentId { get; set; }

        /// <summary>
        /// List of errors if timer has failed
        /// </summary>
        public TimerErrors Errors { get; set; } = TimerErrors.None;

        private TimerStatus _status;
        private TimerStatus GetStatus()
        {
            //only update if currently within an active state
            if ((int)_status < 16)
            {
                if (this.ArrivalTimeUtc < DateTime.UtcNow)
                {
                    _status = TimerStatus.AwaitingConfirmation;
                }
                else if (this.DepartureTimeUtc < DateTime.UtcNow)
                {
                    _status = TimerStatus.Enroute;
                }
                else if (this.DepartureTimeUtc - leaveTimeBuffer < DateTime.UtcNow)
                {
                    _status = TimerStatus.TimeToLeave;
                }
                else
                {
                    _status = TimerStatus.Active;
                }
            }

            return _status;
        }
    }
}
