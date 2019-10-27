using System;
using System.Collections.Generic;
using System.Text;

namespace Punc.Data
{
    internal class TimerDataModel
    {
        public Guid Id { get; set; }
        public TimerStatus Status { get; set; }
        public DateTime ArrivalTimeUtc { get; set; }
        public TimerConfirmationMethod ConfirmationMethod { get; set; }
        public string CustomerEmail { get; set; }
        public string CustomerName { get; set; }
        public string Destination { get; set; }
        public DateTime DepartureTimeUtc { get; set; }
        public DateTime EstimatedArrivalTimeUtc { get; set; }
        public bool ExpertMode { get; set; }
        public DateTime LastUpdateUtc { get; set; }
        public string Origin { get; set; }
        public string RefereeEmail { get; set; }
        public TravelMode TravelMode { get; set; }
        public int TravelDistance { get; set; }
        public int TravelDuration { get; set; }
        public string PaymentIntentId { get; set; }
        public TimerErrors Errors { get; set; }

        public static explicit operator Timer(TimerDataModel obj)
        {
            return TimerObjectMapper.Map(obj);
        }

        public static explicit operator TimerDataModel(Timer obj)
        {
            return TimerObjectMapper.Map(obj);
        }
    }
}
