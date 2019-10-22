using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Punc
{
    public class GoogleMapsGeocodingService : IGeocodingService
    {
        private readonly HttpClient _httpClient;

        private readonly string apiEndpoint;
        private readonly string apiKey;
        private readonly string geoCodeRelativeUri;

        public GoogleMapsGeocodingService(HttpClient client, IConfiguration config)
        {
            apiEndpoint = config["GoogleMaps:ApiEndpoint"];
            geoCodeRelativeUri = config["GoogleMaps:Geocode:RelativeUri"];
            apiKey = config["GoogleMaps:Geocode:ApiKey"];

            _httpClient = client;
            _httpClient.BaseAddress = new Uri(apiEndpoint + geoCodeRelativeUri);

        }

        public async Task<string> ReverseLookup(decimal latitude, decimal longitude)
        {
            var callParams = $"?key={apiKey}&latlng={latitude.ToString()},{longitude.ToString()}";
            var res = await _httpClient.GetAsync(callParams);

            if(res.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var resObj = JObject.Parse(await res.Content.ReadAsStringAsync());
                return resObj["results"][0]["formatted_address"].ToString();
            }
            else
            {
                return null;
            }
        }
    }
}
