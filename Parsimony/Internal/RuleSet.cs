using System;
using System.Collections.Generic;

namespace Parsimony.Internal
{
    internal class RuleSet<T> where T : class, IEquatable<T>
    {
        internal DependencyGraph<T> Requirements { get; }

        internal HashSet<(T, T)> Preclusions { get; }

        internal RuleSet(DependencyGraph<T> requirements, HashSet<(T, T)> preclusions)
        {
            Requirements = requirements ?? throw new ArgumentNullException(nameof(requirements));
            Preclusions = preclusions ?? throw new ArgumentNullException(nameof(preclusions));
        }
    }
}
