using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Parsimony.Internal
{
    /// <summary>
    /// A graph of <see cref="Dependency{T}"/> objects.
    /// </summary>
    /// <remarks>
    /// The graph reflects both the explicitly added dependencies and the dependencies they imply. For example if A
    /// depends on B and B depends on C then the graph will reflect that A depends on C.
    /// </remarks>
    internal class DependencyGraph<T> : IEnumerable<Dependency<T>> where T : IEquatable<T>
    {
        private readonly List<Dependency<T>> _dependencies = new List<Dependency<T>>();

        /// <summary>
        /// Creates a new, empty <see cref="DependencyGraph{T}"/>.
        /// </summary>
        internal DependencyGraph() { }

        /// <summary>
        /// Adds a dependency to the graph if it's not already present.
        /// </summary>
        /// <param name="newDependency">The dependency to add.</param>
        /// <returns>
        /// <c>true</c> if the dependency was added to the graph, or <c>false</c> if it was already present.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="newDependency"/> is <c>null</c>.</exception>
        internal bool Add(Dependency<T> newDependency)
        {
            if (newDependency == null) throw new ArgumentNullException(nameof(newDependency));

            if (_dependencies.Contains(newDependency)) return false;

            _dependencies.Add(newDependency);

            var inheritedDependees = _dependencies
                .Where(d => d.Dependent.Equals(newDependency.Dependee))
                .Select(d => d.Dependee)
                .ToList();

            var newDependees = new List<T>();
            foreach (var inheritedDependee in inheritedDependees)
            {
                if (newDependency.Dependent.Equals(inheritedDependee))
                    continue;

                if (Add(new Dependency<T>(newDependency.Dependent, inheritedDependee)))
                    newDependees.Add(inheritedDependee);
            }

            var ancestorDependents = _dependencies
                .Where(d => d.Dependee.Equals(newDependency.Dependent))
                .Select(d => d.Dependent)
                .ToList();

            foreach (var ancestorDependent in ancestorDependents)
            {
                if (ancestorDependent.Equals(newDependency.Dependee))
                    continue;

                Add(new Dependency<T>(ancestorDependent, newDependency.Dependee));
            }

            return true;
        }

        /// <inheritdoc/>
        public IEnumerator<Dependency<T>> GetEnumerator()
        {
            return ((IEnumerable<Dependency<T>>)_dependencies).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable<Dependency<T>>)_dependencies).GetEnumerator();
        }
    }
}
