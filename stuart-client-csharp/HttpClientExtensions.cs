using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Reflection;
using System.Text;

namespace StuartDelivery
{
    internal static class HttpClientExtensions
    {
        private static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
        {
            NullValueHandling = NullValueHandling.Ignore
        };

        public static void AddData(this HttpRequestMessage request, object data)
        {
            if (data != null)
            {
                request.Content = new ObjectContent(data.GetType(), data, new JsonMediaTypeFormatter
                {
                    SerializerSettings = Settings
                });
            }
        }

        public static void SetUserAgent(this HttpClient client) {
            try
            {
                var versionNumber = Assembly.GetExecutingAssembly().GetName().Version.ToString();
                client.DefaultRequestHeaders.Add("User-Agent", $"stuart-client-csharp/{versionNumber}");
            }
            catch (Exception)
            {
                // Use default version
                client.DefaultRequestHeaders.Add("User-Agent", "stuart-client-csharp/1.0");
            }
        }
    }
}
