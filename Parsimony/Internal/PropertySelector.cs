using System;
using System.Linq.Expressions;
using System.Reflection;

namespace Parsimony.Internal
{
    internal class PropertySelector<TInput, TProp>
    {
        /// <summary>
        /// The name of the member the selector function targets.
        /// </summary>
        internal MemberInfo Member { get; }

        /// <summary>
        /// The setter for the property.
        /// </summary>
        internal Action<TInput, TProp> Setter { get; }

        /// <summary>
        /// Creates a new <see cref="PropertySelector{TInput, TProp}"/> with the given selector function.
        /// </summary>
        /// <param name="property">A property selector function.</param>
        /// <exception cref="ArgumentNullException"><paramref name="property"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="property"/> is not a <see cref="MemberExpression"/> for a writable property of
        /// <typeparamref name="TInput"/>.
        /// </exception>
        internal PropertySelector(Expression<Func<TInput, TProp>> property)
        {
            if (property == null)
                throw new ArgumentNullException(nameof(property));

            if (!(property.Body is MemberExpression mbr))
                throw new ArgumentException("Must be a member selector expression.", nameof(property));

            //TODO:CN -- Handle public mutable fields?
            var prop = mbr.Member as PropertyInfo ??
                throw new ArgumentException($"Must be a property of {nameof(TInput)}.", nameof(property));

            if (!prop.CanWrite)
                throw new ArgumentException("Property must be writable.", nameof(property));

            var instance = Expression.Parameter(typeof(TInput));
            var value = Expression.Parameter(typeof(TProp));
            var setterBody = Expression.Call(instance, prop.GetSetMethod(), value);
            var paramset = new ParameterExpression[] { instance, value };

            Member = mbr.Member;
            Setter = Expression.Lambda<Action<TInput, TProp>>(setterBody, paramset).Compile();
        }
    }
}
