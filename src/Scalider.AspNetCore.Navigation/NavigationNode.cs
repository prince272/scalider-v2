using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using JetBrains.Annotations;

namespace Scalider.AspNetCore.Navigation
{

    /// <summary>
    /// Provides a holder for all the properties of a linked document.
    /// </summary>
    public class NavigationNode
    {

        /// <summary>
        /// Gets or sets a value indicating the identifier of the linked document.
        /// </summary>
        [UsedImplicitly]
        public string Key { get; set; }

        /// <summary>
        /// Gots or sets a value indicating whether the linked document is enabled.
        /// </summary>
        [UsedImplicitly]
        public bool Enabled { get; set; }

        /// <summary>
        /// Gets or sets the title of the linked document.
        /// </summary>
        [UsedImplicitly]
        public string Title { get; set; }
        
        /// <summary>
        /// Gets or sets a value indicating where to open the linked document.
        /// </summary>
        [UsedImplicitly]
        public string LinkTarget { get; set; }

        /// <summary>
        /// Gets or sets a value indicating the name of the MVC route.
        /// </summary>
        [UsedImplicitly]
        public string RouteName { get; set; }

        /// <summary>
        /// Gets or sets a value indicating the name of the area that is used to build the MVC url.
        /// </summary>
        [UsedImplicitly]
        public string Area { get; set; }

        /// <summary>
        /// Gets or sets a value indicating the name of the controller that is used to build the MVC url.
        /// </summary>
        [UsedImplicitly]
        public string Controller { get; set; }

        /// <summary>
        /// Gets or sets a value indicating the name of the action that is used to build the MVC url.
        /// </summary>
        [UsedImplicitly]
        public string Action { get; set; }
        
        /// <summary>
        /// Gets or sets a dictionary containing the aditional route parameters.
        /// </summary>
        [UsedImplicitly]
        public IDictionary<string, string> RouteParameters { get; set; }

        /// <summary>
        /// Gets or sets a value indicating the fixed url of the linked document.
        /// </summary>
        [UsedImplicitly]
        public string Url { get; set; }

        /// <summary>
        /// Gets or sets the roles required by the user to access the linked document.
        /// </summary>
        [UsedImplicitly]
        public string Roles { get; set; }

        /// <summary>
        /// Gets or sets the name of the policy used to determine if the user have access to the linked document.
        /// </summary>
        [UsedImplicitly]
        public string AuthorizationPolicyName { get; set; }

        /// <summary>
        /// Gets or sets the kind of user the linked document should be hidden from.
        /// </summary>
        [UsedImplicitly]
        public HideNodeFrom HideNodeFrom { get; set; }

        /// <summary>
        /// Gets or sets the class name of the linked document element.
        /// </summary>
        [UsedImplicitly]
        public string ClassName { get; set; }

        /// <summary>
        /// Gets or sets the icon of the linked document element.
        /// </summary>
        [UsedImplicitly]
        public string Icon { get; set; }

        /// <inheritdoc />
        public override string ToString() => string.IsNullOrEmpty(Title) ? base.ToString() : Title;

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
                return false;
            if (ReferenceEquals(this, obj))
                return true;

            return obj.GetType() == GetType() && Equals((NavigationNode)obj);
        }

        /// <inheritdoc />
        [SuppressMessage("ReSharper", "NonReadonlyMemberInGetHashCode")]
        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = Key != null ? Key.GetHashCode() : 0;
                hashCode = (hashCode * 397) ^ Enabled.GetHashCode();
                hashCode = (hashCode * 397) ^ (Title != null ? Title.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Area != null ? Area.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Controller != null ? Controller.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Action != null ? Action.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (RouteName != null ? RouteName.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Url != null ? Url.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Roles != null ? Roles.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^
                           (AuthorizationPolicyName != null ? AuthorizationPolicyName.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (int)HideNodeFrom;
                hashCode = (hashCode * 397) ^ (ClassName != null ? ClassName.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Icon != null ? Icon.GetHashCode() : 0);

                return hashCode;
            }
        }

        /// <summary>
        /// Indicates whether the values of two specified <see cref="NavigationTreeNode" /> have the same value.
        /// </summary>
        /// <param name="lhs">The first object to compare.</param>
        /// <param name="rhs">The second object to compare.</param>
        /// <returns>
        /// <c>true</c> if the value of <paramref name="lhs" /> is the same as the value of <paramref name="rhs" />;
        /// otherwise, <c>false</c>.
        /// </returns>
        public static bool operator ==(NavigationNode lhs, NavigationNode rhs) =>
            lhs?.Equals((object)rhs) ?? Equals(rhs, null);

        /// <summary>
        /// Indicates whether the values of two specified <see cref="NavigationTreeNode" /> have different values.
        /// </summary>
        /// <param name="lhs">The first object to compare.</param>
        /// <param name="rhs">The second object to compare.</param>
        /// <returns>
        /// <c>true</c> if the value of <paramref name="lhs" /> is different from the value of
        /// <paramref name="rhs" />; otherwise, <c>false</c>.
        /// </returns>
        public static bool operator !=(NavigationNode lhs, NavigationNode rhs) => !(lhs == rhs);

        private bool Equals(NavigationNode other) =>
            string.Equals(Key, other.Key) &&
            Enabled == other.Enabled &&
            string.Equals(Title, other.Title) &&
            string.Equals(Area, other.Area) &&
            string.Equals(Controller, other.Controller) &&
            string.Equals(Action, other.Action) &&
            string.Equals(RouteName, other.RouteName) &&
            string.Equals(Url, other.Url) &&
            string.Equals(Roles, other.Roles) &&
            string.Equals(AuthorizationPolicyName, other.AuthorizationPolicyName) &&
            HideNodeFrom == other.HideNodeFrom &&
            string.Equals(ClassName, other.ClassName) &&
            string.Equals(Icon, other.Icon);

    }

}