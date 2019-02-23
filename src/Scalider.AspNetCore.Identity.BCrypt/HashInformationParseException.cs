 using System;

 namespace Scalider.AspNetCore.Identity
 {

     /// <summary>
     /// Exception for signalling parse errors.
     /// </summary>
     internal class HashInformationParseException : Exception
     {

         /// <summary>
         /// Initializes a new instance of the <see cref="HashInformationParseException"/> class.
         /// </summary>
         /// <param name="message">The message.</param>
         public HashInformationParseException(string message)
             : base(message)
         {
         }

     }

 }