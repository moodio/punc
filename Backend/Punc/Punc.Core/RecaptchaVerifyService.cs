using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Punc
{
    public class RecaptchaVerifyService
    {

        //minimum score to accept
        const decimal minScore = 0.3m;

        private HttpClient _httpClient;

        private readonly string _apiKey;
        private readonly string _apiEndpoint;

        public RecaptchaVerifyService(HttpClient client, IConfiguration config)
        {

            _apiEndpoint = config["Recaptcha:ApiEndpoint"];
            _apiKey = config["Recaptcha:ApiKey"];

            _httpClient = client;
            _httpClient.BaseAddress = new Uri(_apiEndpoint);
            
        }

        public async Task<bool> VerifyToken(string token, string action)
        {
            var callParams = $"?secret={_apiKey}&response={token}";
            var apiReq = await _httpClient.GetAsync(callParams);

            if (apiReq.IsSuccessStatusCode)
            {
                var resObj = JObject.Parse(await apiReq.Content.ReadAsStringAsync());
                var success = resObj["success"];

                if (!resObj["success"].ToObject<bool>() 
                    || resObj["score"].ToObject<decimal>() < minScore
                    || resObj["action"].ToString() != action)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            else
            {
                return false;
            }

        }
    }
}
