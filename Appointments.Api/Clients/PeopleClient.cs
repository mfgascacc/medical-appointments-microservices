using Appointments.Api.Dtos;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Web;

namespace Appointments.Api.Clients
{
    public class PeopleClient : IPeopleClient
    {
        private readonly string _baseUrl;
        
        public PeopleClient()
        {
            _baseUrl = ConfigurationManager.AppSettings["PeopleApiBaseUrl"];
        }

        public PeoplePersonResponse GetPersonById(Guid id)
        {
            using (var client = new HttpClient())

            {
                var token = GetPeopleApiToken(client);
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                var requestUrl = $"{_baseUrl.TrimEnd('/')}/api/people/{id}";
                var response = client.GetAsync(requestUrl).Result;
                if (response.StatusCode == HttpStatusCode.NotFound)
                {
                    return null;
                }
                response.EnsureSuccessStatusCode();
                var json = response.Content.ReadAsStringAsync().Result;
                return JsonConvert.DeserializeObject<PeoplePersonResponse>(json);
            }

        }

        private string GetPeopleApiToken(HttpClient client)
        {
            var authUrl = $"{_baseUrl.TrimEnd('/')}/api/auth/token";
            var payload = new
            {
                username = ConfigurationManager.AppSettings["PeopleApiAuthUsername"] ?? "admin",
                password = ConfigurationManager.AppSettings["PeopleApiAuthPassword"] ?? "admin123"
            };

            var content = new StringContent(
                JsonConvert.SerializeObject(payload),
                Encoding.UTF8,
                "application/json");

            var response = client.PostAsync(authUrl, content).Result;
            response.EnsureSuccessStatusCode();
            var json = response.Content.ReadAsStringAsync().Result;
            var tokenResponse = JsonConvert.DeserializeObject<PeopleApiTokenResponse>(json);
            if (tokenResponse == null || string.IsNullOrWhiteSpace(tokenResponse.AccessToken))
            {
                throw new InvalidOperationException("People API token was not generated.");
            }

            return tokenResponse.AccessToken;
        }

        private class PeopleApiTokenResponse
        {
            [JsonProperty("accessToken")]
            public string AccessToken { get; set; }
        }

    }
}