 using System;

 // ReSharper disable once CheckNamespace
 namespace BCrypt.Net
 {

     /// <summary>
     /// Exception for signalling parse errors.
     /// </summary>
     public class HashInformationParseException : Exception
     {

         /// <summary>
         /// Initializes a new instance of the <see cref="HashInformationParseException"/> class.
         /// </summary>
         public HashInformationParseException()
         {
         }

         /// <summary>
         /// Initializes a new instance of the <see cref="HashInformationParseException"/> class.
         /// </summary>
         /// <param name="message">The message.</param>
         public HashInformationParseException(string message)
             : base(message)
         {
         }

         /// <summary>
         /// Initializes a new instance of the <see cref="HashInformationParseException"/> class.
         /// </summary>
         /// <param name="message">The message.</param>
         /// <param name="innerException">The inner exception.</param>
         public HashInformationParseException(string message, Exception innerException)
             : base(message, innerException)
         {
         }

     }

 }