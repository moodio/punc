using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Punc
{
    public class GoogleMapsDirectionsService : IRouteService
    {
        private readonly HttpClient _client;

        private readonly string _apiEndpoint;
        private readonly string _directionsRelativeUri;
        private readonly string _apiKey;

        public GoogleMapsDirectionsService(HttpClient client, IConfiguration config)
        {
            _apiEndpoint = config["GoogleMaps:ApiEndpoint"];
            _directionsRelativeUri = config["GoogleMaps:Directions:RelativeUri"];
            _apiKey = config["GoogleMaps:Directions:ApiKey"];

            _client = client;
            _client.BaseAddress = new Uri(_apiEndpoint + _directionsRelativeUri);
        }

        public async Task<RouteInformation> GetRouteInformationAsync(string location, string destination, 
            TravelMode travelMode, int arrivalTimeEpoch)
        {
            //setup the call params
            var callParams = $"?key={_apiKey}&origin={location}&destination={destination}&mode={travelMode.ToString().ToLower()}";
            if (travelMode == TravelMode.Transit)
            {
                callParams += $"&arrival_time={arrivalTimeEpoch.ToString()}";
            }

            //call the maps api
            var apiReq = await _client.GetAsync(callParams);

            //return the results
            var res = new RouteInformation()
            {
                Success = apiReq.IsSuccessStatusCode
            };

            if (res.Success)
            {
                var jo = JObject.Parse(await apiReq.Content.ReadAsStringAsync());

                //check route was actually created
                if (!jo["routes"].HasValues)
                {
                    res.Success = false;
                    return res;
                }

                var joRoute = jo["routes"][0]["legs"][0];

                //get duration and distance
                res.TravelDistance = joRoute["distance"]["value"].ToObject<int>();
                res.TravelDuration = joRoute["duration"]["value"].ToObject<int>();

                if (travelMode == TravelMode.Transit)
                {
                    //calculate departure time based on route information
                    res.ArrivalTime = joRoute["arrival_time"]["value"].ToObject<int>();
                    res.DepartureTime = joRoute["departure_time"]["value"].ToObject<int>();
                }
                else
                {
                    //calculate departure time based on arrival time
                    res.ArrivalTime = arrivalTimeEpoch;
                    res.DepartureTime = arrivalTimeEpoch - res.TravelDuration; 
                }
            }

            return res;
        }
    }
}
