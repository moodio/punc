using System;
using System.Collections.Generic;
using System.Text;

namespace Punc
{
    /// <summary>
    /// Class for timer details visible to a referee
    /// </summary>
    public class RefereeTimer
    {
        public DateTime ArrivalTimeUtc { get; set; }

        public string CustomerName { get; set; }

        public string Destination { get; set; }

        public TimerStatus Status { get; set; }

        public static explicit operator RefereeTimer(Timer timer)
        {
            if (timer == null)
                return null;

            return new RefereeTimer()
            {
                ArrivalTimeUtc = timer.ArrivalTimeUtc,
                CustomerName = timer.CustomerName,
                Destination = timer.Destination,
                Status = timer.Status
            };
        }
    }
}
