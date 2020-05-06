using System;
using System.Collections.Generic;

namespace Parsimony.Internal
{
    /// <summary>
    /// Represents a dependency between two instances of <typeparamref name="T"/>.
    /// </summary>
    internal class Dependency<T> : IEquatable<Dependency<T>?> where T : IEquatable<T>
    {
        internal T Dependent { get; }

        internal T Dependee { get; }

        /// <summary>
        /// Creates a new <see cref="Dependency{T}"/>.
        /// </summary>
        /// <param name="dependent">The dependent entity.</param>
        /// <param name="dependee">The depended-upon entity.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="dependent"/> or <paramref name="dependee"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="dependent"/> and <paramref name="dependee"/> are equal.
        /// </exception>
        internal Dependency(T dependent, T dependee)
        {
            Dependent = dependent ?? throw new ArgumentNullException(nameof(dependent));
            Dependee = dependee ?? throw new ArgumentNullException(nameof(dependee));
            if (dependent.Equals(dependee))
                throw new ArgumentException($"Must not equal {nameof(dependent)}.", nameof(dependee));
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as Dependency<T>);
        }

        public bool Equals(Dependency<T>? other)
        {
            return other != null &&
                   EqualityComparer<T>.Default.Equals(Dependent, other.Dependent) &&
                   EqualityComparer<T>.Default.Equals(Dependee, other.Dependee);
        }

        public override int GetHashCode()
        {
            int hashCode = 1245959685;
            hashCode = hashCode * -1521134295 + EqualityComparer<T>.Default.GetHashCode(Dependent);
            hashCode = hashCode * -1521134295 + EqualityComparer<T>.Default.GetHashCode(Dependee);
            return hashCode;
        }

        public static bool operator ==(Dependency<T>? left, Dependency<T>? right)
        {
            // The Equals method accepts null.
#pragma warning disable CS8604 // Possible null reference argument.
            return EqualityComparer<Dependency<T>>.Default.Equals(left, right);
#pragma warning restore CS8604
        }

        public static bool operator !=(Dependency<T>? left, Dependency<T>? right)
        {
            return !(left == right);
        }
    }
}
