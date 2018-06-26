using System;

namespace Scalider.AspNetCore.Navigation
{
    
    /// <summary>
    /// Defines which kind of user a node should be hidden from.
    /// </summary>
    [Flags]
    public enum HideNodeFrom
    {

        /// <summary>
        /// The node should not be hidden for any user.
        /// </summary>
        None,
        
        /// <summary>
        /// The node should be hidden for anonymous users.
        /// </summary>
        Anonymous,
        
        /// <summary>
        /// The node should be hidden for authenticated users.
        /// </summary>
        Authenticated,
        
        /// <summary>
        /// The node should be hidden for all users.
        /// </summary>
        All = Anonymous | Authenticated

    }
    
}