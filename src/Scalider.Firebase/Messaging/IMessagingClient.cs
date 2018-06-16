using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace Scalider.Firebase.Messaging
{
    
    // https://firebase.google.com/docs/cloud-messaging/http-server-ref
    public interface IMessagingClient
    {

        IDownstreamMessage Send([NotNull] RemoteMessage remoteMessage);

        Task<IDownstreamMessage> SendAsync([NotNull] RemoteMessage remoteMessage,
            CancellationToken cancellationToken = default);

    }
    
}