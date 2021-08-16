using System;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace StuartDelivery
{
    public class WebClient
    {
        private readonly HttpClient _client;
        private readonly Authenticator _authenticator;

        public Authenticator Authenticator
        {
            get
            {
                return _authenticator;
            }
        }

        public WebClient(Authenticator authenticator)
        {
            _authenticator = authenticator;

            _client = new HttpClient { BaseAddress = new Uri(authenticator.Environment.BaseUrl) };
            _client.SetUserAgent();
        }

        public async Task<HttpResponseMessage> GetAsync(string uri)
        {
            return await _client.SendAsync(await CreateRequest(HttpMethod.Get, uri, null));
        }

        public async Task<HttpResponseMessage> PostAsync<TModel>(string uri, TModel model)
        {
            return await _client.SendAsync(await CreateRequest(HttpMethod.Post, uri, model));
        }

        public async Task<HttpResponseMessage> PostAsync(string uri)
        {
            return await _client.SendAsync(await CreateRequest(HttpMethod.Post, uri, null));
        }

        public async Task<HttpResponseMessage> PutAsync<TModel>(string uri, TModel model)
        {
            return await _client.SendAsync(await CreateRequest(HttpMethod.Put, uri, model));
        }

        public async Task<HttpResponseMessage> PatchAsync<TModel>(string uri, TModel model)
        {
            return await _client.SendAsync(await CreateRequest(new HttpMethod("PATCH"), uri, model));
        }

        public async Task<HttpResponseMessage> DeleteAsync(string uri)
        {
            return await _client.SendAsync(await CreateRequest(HttpMethod.Delete, uri, null));
        }

        private async Task<HttpRequestMessage> CreateRequest(HttpMethod method, string uri, object data)
        {
            var request = new HttpRequestMessage(method, uri);
            request.AddData(data);

            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", await _authenticator.GetAccessToken());

            return request;
        }
    }
}
