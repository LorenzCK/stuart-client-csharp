using StuartDelivery.Abstract;
using StuartDelivery.Concrete;
using System;

namespace StuartDelivery
{
    public class StuartApi
    {
        private readonly WebClient _client;

        private IAddress _address;
        private IJob _job;

        public IAddress Address { get { return _address ?? (_address = new Address(_client)); } }
        public IJob Job { get { return _job ?? (_job = new Job(_client)); } }

        public Authenticator Authenticator
        {
            get
            {
                return _client.Authenticator;
            }
        }

        /// <summary>
        /// Initializes the Stuart API.
        /// </summary>
        public static StuartApi Initialize(Environment environment, string clientId, string clientSecret)
        {
            var authenticator = new Authenticator(environment, clientId, clientSecret);
            var webClient = new WebClient(authenticator);

            return new StuartApi(webClient);
        }

        /// <summary>
        /// Initializes the Stuart API with a preset authentication token (without expiration timestamp).
        /// </summary>
        public static StuartApi Initialize(Environment environment, string clientId, string clientSecret, string authToken)
        {
            var authenticator = new Authenticator(environment, clientId, clientSecret, authToken);
            var webClient = new WebClient(authenticator);

            return new StuartApi(webClient);
        }

        /// <summary>
        /// Initializes the Stuart API with a preset authentication token.
        /// </summary>
        public static StuartApi Initialize(Environment environment, string clientId, string clientSecret, string authToken, DateTime tokenExpiration)
        {
            var authenticator = new Authenticator(environment, clientId, clientSecret, authToken, tokenExpiration);
            var webClient = new WebClient(authenticator);

            return new StuartApi(webClient);
        }

        private StuartApi(WebClient webClient)
        {
            _client = webClient;
        }
    }
}
