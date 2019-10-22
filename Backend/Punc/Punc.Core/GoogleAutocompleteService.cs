using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Punc
{
    public class GoogleAutocompleteService : IAutoCompleteService
    {
        private HttpClient _httpClient;

        private readonly string _apiEndpoint;
        private readonly string _apiKey;
        private readonly string _autoCompleteRelativeUri;

        public GoogleAutocompleteService(HttpClient client, IConfiguration config)
        {
            _httpClient = client;

            _apiEndpoint = config["GoogleMaps:ApiEndpoint"];
            _autoCompleteRelativeUri = config["GoogleMaps:AutoComplete:RelativeUri"];
            _apiKey = config["GoogleMaps:AutoComplete:ApiKey"];

            _httpClient.BaseAddress = new Uri(_apiEndpoint + _autoCompleteRelativeUri);
        }

        public async Task<List<string>> Search(string sessionKey, string query, string location = null)
        {
            var callParams = $"?key={_apiKey}&sessiontoken={sessionKey}&input={query}";
            if (!String.IsNullOrWhiteSpace(location))
            {
                callParams += "&location={location}&radiusa=20000";
            }

            var res = await _httpClient.GetAsync(callParams);

            if(res.StatusCode == HttpStatusCode.OK)
            {
                var resObj = JObject.Parse(await res.Content.ReadAsStringAsync());

                return resObj["predictions"]
                    .Select(p => p["description"].ToString()).ToList();

            }

            return new List<string>();


        }
    }
}
