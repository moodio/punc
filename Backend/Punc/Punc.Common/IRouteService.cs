using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Punc
{
    public interface IRouteService
    {
        Task<RouteInformation> GetRouteInformationAsync(string location, string destination, 
            TravelMode travelMode, int arrivalTimeEpoch); 
    }
}
