using System;
using System.Net.Http;
using System.Threading.Tasks;
using StuartDelivery.Models;
using StuartDelivery.Models.Token;

namespace StuartDelivery
{
    public class Authenticator
    {
        private readonly Environment _environment;

        private OAuth2AccessToken _oAuth2AccessToken;

        private readonly string _clientId;
        private readonly string _clientSecret;

        public Environment Environment
        {
            get {
                return _environment;
            }
        }

        public Authenticator(Environment environment, string apiClientId, string apiClientSecret)
            : this(environment, apiClientId, apiClientSecret, null, null)
        {

        }

        public Authenticator(Environment environment, string apiClientId, string apiClientSecret, string authToken)
            : this(environment, apiClientId, apiClientSecret, authToken, null)
        {
            
        }

        public Authenticator(Environment environment, string apiClientId, string apiClientSecret, string authToken, DateTime? tokenExpiration)
        {
            _environment = environment ?? throw new ArgumentNullException(nameof(environment));
            _clientId = apiClientId ?? throw new ArgumentNullException(nameof(apiClientId));
            _clientSecret = apiClientSecret ?? throw new ArgumentNullException(nameof(apiClientSecret));

            if(authToken != null) {
                _oAuth2AccessToken = new OAuth2AccessToken {
                    AccessToken = authToken,
                    ExpireDate = tokenExpiration.GetValueOrDefault(DateTime.MaxValue) // Assume token never expires if not set
                };
            }
        }

        public virtual async Task<string> GetAccessToken()
        {
            if(_oAuth2AccessToken == null || _oAuth2AccessToken.IsExpired) {
                await RefreshAuthToken();
            }

            return _oAuth2AccessToken.AccessToken;
        }

        public bool HasValidToken
        {
            get
            {
                return _oAuth2AccessToken != null && !_oAuth2AccessToken.IsExpired;
            }
        }

        public async Task RefreshAuthToken()
        {
            var request = new HttpRequestMessage(HttpMethod.Post, "/oauth/token");
            request.AddData(new TokenRequest {
                ClientId = _clientId,
                ClientSecret = _clientSecret,
                Scope = "api",
                GrantType = "client_credentials"
            });

            var client = new HttpClient { BaseAddress = new Uri(_environment.BaseUrl) };
            client.SetUserAgent();

            var response = await client.SendAsync(request).ConfigureAwait(false);

            TokenSuccessResponse tokenResponse;
            if (response.IsSuccessStatusCode)
                tokenResponse = await response.Content.ReadAsAsync<TokenSuccessResponse>().ConfigureAwait(false);
            else
                throw new HttpRequestException($"Access token request failed with message: {response.Content.ReadAsAsync<ErrorResponse>().Result.ErrorDescription}");

            var createdAt = DateTimeOffset.FromUnixTimeSeconds(tokenResponse.CreatedAt);
            var expiresAt = createdAt.AddSeconds(tokenResponse.ExpiresIn);

            _oAuth2AccessToken = new OAuth2AccessToken() {
                AccessToken = tokenResponse.AccessToken,
                ExpireDate = expiresAt.UtcDateTime,
                Scope = tokenResponse.Scope,
                TokenType = tokenResponse.TokenType
            };
        }

    }
}
