using System;
using System.Linq.Expressions;
using System.Reflection;

namespace Parsimony.ParserBuilder
{
    internal class PropertySelector<TInput, TProp>
    {
        /// <summary>
        /// The name of the member the selector function targets
        /// </summary>
        public string MemberName { get; }

        /// <summary>
        /// A setter for the property 
        /// </summary>
        public Action<TInput, TProp> Setter { get; }

        /// <summary>
        /// A getter for the property
        /// </summary>
        public Func<TInput, TProp> Getter { get; }

        /// <summary>
        /// Constructs a new PropertySelector with the given selector function
        /// </summary>
        /// <exception cref="ArgumentNullException"> <paramref name="selector"/> is null </exception>
        /// <exception cref="ArgumentException"> <paramref name="selector"/> is not of type <see cref="MemberExpression"/> </exception>
        /// <exception cref="ArgumentException"> The member selected by <paramref name="selector"/> is not a property </exception>
        /// <exception cref="ArgumentException"> The member selected by <paramref name="selector"/> is not readable and writable </exception>
        /// <param name="selector"> A property selector function</param>
        public PropertySelector(Expression<Func<TInput, TProp>> selector)
        {
            if (selector == null)
                throw new ArgumentNullException(nameof(selector));

            if (!(selector.Body is MemberExpression mbr))
                throw new ArgumentException("Must be a member selector expression");

            //TODO:CN -- Handle convert nodes? casts etc?
            //TODO:CN -- Handle public mutable fields?
            var prop = mbr.Member as PropertyInfo ?? throw new ArgumentException("Member selection must be a property");

            if (!prop.CanWrite)
                throw new ArgumentException("Property must be readable and writable");

            var instance = Expression.Parameter(typeof(TInput));
            var value = Expression.Parameter(typeof(TProp));
            var setterBody = Expression.Call(instance, prop.GetSetMethod(), value);
            var paramset = new ParameterExpression[] { instance, value };

            MemberName = mbr.Member.Name;
            Getter = selector.Compile();
            Setter = Expression.Lambda<Action<TInput, TProp>>(setterBody, paramset).Compile();
        }
    }
}
