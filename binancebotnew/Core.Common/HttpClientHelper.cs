using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Core.Common
{
    public class HttpClientHelper
    {
        public static async Task<HttpResponseMessage> PostAsync(object model, string baseUrl, string requestUri)
        {
            System.Net.ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(baseUrl);

                var serializedRequest = JsonConvert.SerializeObject(model);

                var request = new StringContent(serializedRequest);

                request.Headers.Clear();

                request.Headers.Add("Content-Type", "application/json");

                return await client.PostAsync(requestUri, request);
            }
        }

        public static async Task<HttpResponseMessage> PostAsync(object model, string baseUrl, string requestUri, string hmac = "")
        {
            System.Net.ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(baseUrl);
                if (!string.IsNullOrEmpty(hmac))
                {
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", hmac);
                }

                var serializedRequest = JsonConvert.SerializeObject(model);

                var request = new StringContent(serializedRequest);

                request.Headers.Clear();

                request.Headers.Add("Content-Type", "application/json");

                return await client.PostAsync(requestUri, request);
            }
        }

        public static async Task<HttpResponseMessage> PostAsyncHmac(object model, string baseUrl, string requestUri, string hmac)
        {
            System.Net.ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(baseUrl);
                if (!string.IsNullOrEmpty(hmac))
                {
                    client.DefaultRequestHeaders.Add("HMAC",hmac);
                }

                var serializedRequest = JsonConvert.SerializeObject(model);

                var request = new StringContent(serializedRequest);

                request.Headers.Clear();

                request.Headers.Add("Content-Type", "application/x-www-form-urlencoded");

                return await client.PostAsync(requestUri, request);
            }
        }


        public static async Task<HttpResponseMessage> PostAsyncToken(object model, string baseUrl, string requestUri, string token)
        {
            System.Net.ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(baseUrl);
                if (!string.IsNullOrEmpty(token))
                {
                    client.DefaultRequestHeaders.Add("Authorization", $"Token {token}");
                }

                var serializedRequest = JsonConvert.SerializeObject(model);

                var request = new StringContent(serializedRequest);

                request.Headers.Clear();

                request.Headers.Add("Content-Type", "application/json");

                return await client.PostAsync(requestUri, request);
            }
        }

        public static async Task<HttpResponseMessage> DeleteAsync(string baseUrl, string requestUri, string token = "")
        {
            System.Net.ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(baseUrl);
                if (!string.IsNullOrEmpty(token))
                {
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                }

                var serializedRequest = JsonConvert.SerializeObject(new object());

                var request = new StringContent(serializedRequest);

                request.Headers.Clear();

                request.Headers.Add("Content-Type", "application/json");

                return await client.DeleteAsync(requestUri);
            }
        }

        public static async Task<HttpResponseMessage> GetAsync(string baseUrl, string requestUri)
        {
            System.Net.ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };

            using (var client = new HttpClient())
            {
                var request = new HttpRequestMessage()
                {
                    RequestUri = new Uri($"{baseUrl}{requestUri}"),
                    Method = HttpMethod.Get,
                };
                request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                return await client.SendAsync(request);
            }
        }

        public static async Task<HttpResponseMessage> GetAsync(string baseUrl, string requestUri, string token)
        {
            System.Net.ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(baseUrl);
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                return await client.GetAsync(requestUri);
            }
        }

        public static async Task<HttpResponseMessage> GetAsyncToken(string baseUrl, string requestUri, string token)
        {
            System.Net.ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(baseUrl);
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Token", token);

                return await client.GetAsync(requestUri);
            }
        }
    }
}
