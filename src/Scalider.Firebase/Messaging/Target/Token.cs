namespace Scalider.Firebase.Messaging.Target
{
    
    public sealed class Token : RemoteTarget
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="Token"/> class.
        /// </summary>
        /// <param name="token"></param>
        public Token(string token)
        {
            Check.NotNullOrEmpty(token, nameof(token));

            Value = token;
        }
        
        public string Value { get; }
        
    }
    
}