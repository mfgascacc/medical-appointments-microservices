using Appointments.Api.Dtos;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
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

    }
}