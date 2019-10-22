using System;
using System.Collections.Generic;
using System.Text;

namespace Punc
{
    public class RouteInformation
    {
        public bool Success { get; set; }

        /// <summary>
        /// Arrival time in unix epoch
        /// </summary>
        public int ArrivalTime { get; set; }

        /// <summary>
        /// Departure time in unux epoch
        /// </summary>
        public int DepartureTime { get; set; }

        /// <summary>
        /// Travel time in minutes
        /// </summary>
        public int TravelDuration { get; set; }

        /// <summary>
        /// Travel distance in meters
        /// </summary>
        public int TravelDistance { get; set; }
    }
}
