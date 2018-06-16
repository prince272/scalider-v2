namespace Scalider.Firebase.Messaging
{
    
    public interface IDownstreamMessage
    {

        /// <summary>
        /// Gets a value indicating the unique identifier for the downstream message.
        /// </summary>
        string Id { get; }

    }
    
}