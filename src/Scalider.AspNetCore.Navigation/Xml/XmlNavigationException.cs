using System;

namespace Scalider.AspNetCore.Navigation.Xml
{

    /// <summary>
    /// Exception for signalling errors creating a navigation tree from an XML file.
    /// </summary>
    public class XmlNavigationException : Exception
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="XmlNavigationException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        public XmlNavigationException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="XmlNavigationException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="innerException">The inner exception.</param>
        public XmlNavigationException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

    }

}