using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace Parsimony
{
    /// <summary>
    /// A builder type for <see cref="Parser{T}"/> objects.
    /// </summary>
    /// <typeparam name="T">The type of the options to parse.</typeparam>
    public class ParserBuilder<T> where T : new()
    {
        private List<OptionSpec<T>> _options = new List<OptionSpec<T>>();

        /// <summary>
        /// Creates a new instance of <see cref="ParserBuilder{T}"/>.
        /// </summary>
        public ParserBuilder() { }

        /// <summary>
        /// Adds an option to the parser.
        /// </summary>
        /// <typeparam name="TValue">The type of the option's value.</typeparam>
        /// <param name="shortName">The short name of the option.</param>
        /// <param name="expression">The property or field of <typeparamref name="T"/> to set.</param>
        /// <returns>The <see cref="ParserBuilder{T}"/>.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="expression"/> is <c>null</c>.
        /// </exception>
        public ParserBuilder<T> AddOption<TValue>(
            char shortName,
            Expression<Func<T, TValue>> expression)
        {
            if (expression == null)
                throw new ArgumentNullException(nameof(expression));

            var memberExpression = expression.Body as MemberExpression;

            if (memberExpression == null)
                throw new ArgumentException(
                    "Must be a member-access expression for a property or field",
                    nameof(expression));

            var setMethod = null as Action<T, object>;
            var member = memberExpression.Member;

            switch (member)
            {
#pragma warning disable IDE0059 // Unnecessary assignment of a value
                // This is not unnecessary, the warning is a bug
                case FieldInfo fi:
#pragma warning restore IDE0059 // Unnecessary assignment of a value
                    setMethod = (opts, v) =>
                    {
                        fi.SetValue(opts, v);
                    };
                    break;

#pragma warning disable IDE0059 // Unnecessary assignment of a value
                // This is not unnecessary, the warning is a bug
                case PropertyInfo pi:
#pragma warning restore IDE0059 // Unnecessary assignment of a value
                    setMethod = (opts, v) =>
                    {
                        pi.SetValue(opts, v);
                    };
                    break;

                default:
                    throw new ArgumentException("Must be a member-access expression for a property or field", nameof(expression));
            }

            setMethod(new T(), new object());

            return this;
        }

        /// <summary>
        /// Builds a <see cref="Parser{T}"/>.
        /// </summary>
        /// <returns>The <see cref="Parser{T}"/>.</returns>
        public Parser<T> Build(Action<ParserConfig> configure)
        {
            var options = new ParserConfig();
            configure(options);
            return new Parser<T>(null, null);
        }
    }
}
