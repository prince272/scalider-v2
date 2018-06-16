using System;

namespace Scalider.Firebase
{
    
    public class FirebaseException : Exception
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="FirebaseException"/> class.
        /// </summary>
        /// <param name="message"></param>
        public FirebaseException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FirebaseException"/> class.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="innerException"></param>
        public FirebaseException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
        
    }
    
}