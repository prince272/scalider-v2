using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Scalider.Firebase.Messaging.Legacy
{

    public class LegacyMessagingClient : IMessagingClient
    {

        private const string LegacyEndpoint = "fcm/send";

        private static readonly Uri FirebaseServiceUri = new Uri(FirebaseConstants.Messaging.FirebaseApiUri);
        private readonly JsonSerializerSettings _jsonSerializerSettings;
        private readonly object _lock = new object();
        private readonly LegacyMessagingOptions _options;
        private HttpClient _client;

        /// <summary>
        /// Initializes a new instance of the <see cref="LegacyMessagingClient"/> class.
        /// </summary>
        public LegacyMessagingClient(IOptions<LegacyMessagingOptions> options)
        {
            Check.NotNull(options, nameof(options));

            _options = options.Value ?? new LegacyMessagingOptions();
            if (string.IsNullOrWhiteSpace(_options.AuthorizationKey))
            {
            }

            // Create the JSON serializer settings
            _jsonSerializerSettings = new JsonSerializerSettings();
        }

        private HttpClient GetOrCreateClient()
        {
            if (_client == null)
                lock (_lock)
                {
                    if (_client != null)
                        return _client;

                    _client = new HttpClient {BaseAddress = FirebaseServiceUri};
                    _client.DefaultRequestHeaders.Accept.Clear();
                    _client.DefaultRequestHeaders.Accept.Add(
                        new MediaTypeWithQualityHeaderValue("application/json"));

                    _client.DefaultRequestHeaders.TryAddWithoutValidation("Authorization",
                        $"key={_options.AuthorizationKey}");
                }

            return _client;
        }

        #region IMessagingClient Members

        /// <inheritdoc />
        public virtual IDownstreamMessage Send(RemoteMessage remoteMessage)
        {
            Check.NotNull(remoteMessage, nameof(remoteMessage));
            return SendAsync(remoteMessage).GetAwaiter().GetResult();
        }

        /// <inheritdoc />
        public virtual async Task<IDownstreamMessage> SendAsync(RemoteMessage remoteMessage,
            CancellationToken cancellationToken = default)
        {
            Check.NotNull(remoteMessage, nameof(remoteMessage));

            // Serialize message and send message

            var messageJson = JsonConvert.SerializeObject(remoteMessage, _jsonSerializerSettings);
            var content = new StringContent(messageJson, Encoding.UTF8, "application/json");
            Console.WriteLine(messageJson);

            var result = await GetOrCreateClient().PostAsync(LegacyEndpoint, content, cancellationToken);
            return await HandleDownstreamMessageAsync(result);
        }

        #endregion

        #region HandleDownstreamMessageAsync

        // TODO: https://firebase.google.com/docs/cloud-messaging/http-server-ref#interpret-downstream
        protected static async Task<IDownstreamMessage> HandleDownstreamMessageAsync(
            HttpResponseMessage responseMessage)
        {
            Check.NotNull(responseMessage, nameof(responseMessage));

            // Retrieve the body of the response
            if (responseMessage.Content == null) throw new FirebaseException("");

            var responseBody = await responseMessage.Content.ReadAsStringAsync();
            Console.WriteLine(responseBody);

            // Try to parse the resposne as a JSON object
            JObject responseObject;
            try
            {
                responseObject = JObject.Parse(responseBody);
            }
            catch (Exception e)
            {
                // TODO
                throw new FirebaseException(
                    "The response doesn't seem to be valid",
                    e
                );
            }

            // Try to create the response object
            if (responseBody.Contains("message_id"))
                return new TopicDownstreamMessage();
            if (responseBody.Contains("multicast_id"))
                return MulticastDownstreamMessage.FromJsonObject(responseObject);
            
            // TODO: Other cases
            throw new Exception();
        }

        #endregion

    }

}