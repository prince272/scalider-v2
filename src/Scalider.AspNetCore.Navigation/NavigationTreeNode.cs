using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using JetBrains.Annotations;

namespace Scalider.AspNetCore.Navigation
{

    /// <summary>
    /// Represents a linked tree of <see cref="NavigationNode"/>s.
    /// </summary>
    public class NavigationTreeNode
    {

        private readonly List<NavigationTreeNode> _children = new List<NavigationTreeNode>();

        /// <summary>
        /// Initializes a new instance of the <see cref="NavigationTreeNode"/> class.
        /// </summary>
        /// <param name="node"></param>
        public NavigationTreeNode([NotNull] NavigationNode node)
        {
            Check.NotNull(node, nameof(node));

            Value = node;
        }

        /// <summary>
        /// Gets the navigation node.
        /// </summary>
        public NavigationNode Value { get; }

        /// <summary>
        /// Gets the parent node.
        /// </summary>
        public NavigationTreeNode Parent { get; private set; }

        /// <summary>
        /// Gets a value indicating whether this is the root node of the tree.
        /// </summary>
        public bool IsRootNode => Parent == null;

        /// <summary>
        /// Gets a collection containing all the children of the node.
        /// </summary>
        public IEnumerable<NavigationTreeNode> Children => _children.AsReadOnly();

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
                return false;
            if (ReferenceEquals(this, obj))
                return true;

            return obj.GetType() == GetType() && Equals((NavigationTreeNode)obj);
        }

        /// <inheritdoc />
        [SuppressMessage("ReSharper", "NonReadonlyMemberInGetHashCode")]
        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = _children != null ? _children.GetHashCode() : 0;
                hashCode = (hashCode * 397) ^ (Value != null ? Value.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Parent != null ? Parent.GetHashCode() : 0);

                return hashCode;
            }
        }

        /// <summary>
        /// Adds a child to the tree node.
        /// </summary>
        /// <param name="node">The node to add as child.</param>
        public void AddChild([NotNull] NavigationNode node)
        {
            Check.NotNull(node, nameof(node));
            AddChild(new NavigationTreeNode(node));
        }

        /// <summary>
        /// Adds a child to the tree node.
        /// </summary>
        /// <param name="node">The node to add as child.</param>
        /// <exception cref="ArgumentException">When <paramref name="node"/> already have a parent.</exception>
        public void AddChild([NotNull] NavigationTreeNode node)
        {
            Check.NotNull(node, nameof(node));
            if (node.Parent != null && !Equals(node.Parent, this))
            {
                // The node alreayd has a parent
                throw new ArgumentException(
                    "The specified node already has a parent. You must call RemoveChild() on the " +
                    "child's parent first.",
                    nameof(node)
                );
            }

            node.Parent = this;
            _children.Add(node);
        }

        /// <summary>
        /// Removes a child from this node.
        /// </summary>
        /// <param name="node">The node to be removed.</param>
        /// <returns>
        /// <c>true</c> if <paramref name="node"/> was removed; otherwise, <c>false</c>.
        /// </returns>
        public bool RemoveChild([NotNull] NavigationTreeNode node)
        {
            Check.NotNull(node, nameof(node));
            if (!Equals(node.Parent, this))
            {
                // We can only remove a child from our hireachy
                return node.Parent == null;
            }

            // Try to remove the child
            if (!_children.Remove(node.Parent))
            {
                // It was not possible to remove the child
                return false;
            }

            node.Parent = null;
            return true;
        }

        /// <summary>
        /// Retrieves the first node that matches the given <paramref name="predicate"/>, or <c>null</c> if no
        /// match is found.
        /// </summary>
        /// <param name="predicate">The predicate used to find the node.</param>
        /// <returns>
        /// The node that matches the given <paramref name="predicate"/>, or <c>null</c> if no match is found.
        /// </returns>
        public NavigationTreeNode Find([NotNull] Func<NavigationTreeNode, bool> predicate)
        {
            Check.NotNull(predicate, nameof(predicate));
            if (predicate(this))
                return this;

            var allChildren = GetSelfAndChildren(this).Distinct();
            return allChildren.FirstOrDefault(predicate);
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
        public static bool operator ==(NavigationTreeNode lhs, NavigationTreeNode rhs) =>
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
        public static bool operator !=(NavigationTreeNode lhs, NavigationTreeNode rhs) => !(lhs == rhs);

        private static IEnumerable<NavigationTreeNode> GetSelfAndChildren(NavigationTreeNode node) =>
            new[] {node}.Concat(node.Children.SelectMany(GetSelfAndChildren));

        private bool Equals(NavigationTreeNode other) =>
            Equals(_children, other._children) &&
            Equals(Value, other.Value) &&
            Equals(Parent, other.Parent);

    }

}