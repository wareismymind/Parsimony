using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace Parsimony
{
    /// <summary>
    /// A builder type for <see cref="Parser{TOptions}"/> objects.
    /// </summary>
    /// <typeparam name="TOptions">The type of the options to parse.</typeparam>
    public class ParserBuilder<TOptions> where TOptions : new()
    {
        private List<OptionSpec<TOptions>> _options = new List<OptionSpec<TOptions>>();

        /// <summary>
        /// Creates a new instance of <see cref="ParserBuilder{TOptions}"/>.
        /// </summary>
        public ParserBuilder() { }

        /// <summary>
        /// Adds an option to the parser.
        /// </summary>
        /// <typeparam name="TValue">The type of the option's value.</typeparam>
        /// <param name="shortName">The short name of the option.</param>
        /// <param name="expression">The property or field of <typeparamref name="TOptions"/> to set.</param>
        /// <returns>The <see cref="ParserBuilder{TOptions}"/>.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="expression"/> is <c>null</c>.
        /// </exception>
        public ParserBuilder<TOptions> AddOption<TValue>(
            char shortName,
            Expression<Func<TOptions, TValue>> expression)
        {
            if (expression == null)
                throw new ArgumentNullException(nameof(expression));

            var memberExpression = expression.Body as MemberExpression;

            if (memberExpression == null)
                throw new ArgumentException(
                    "Must be a member-access expression for a property or field",
                    nameof(expression));

            var setMethod = null as Action<TOptions, object>;
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

            setMethod(new TOptions(), new object());

            return this;
        }

        /// <summary>
        /// Builds a <see cref="Parser{TOptions}"/>.
        /// </summary>
        /// <returns>The <see cref="Parser{TOptions}"/>.</returns>
        public Parser<TOptions> Build(Action<ParserConfig> configure)
        {
            var options = new ParserConfig();
            configure(options);
            return new Parser<TOptions>(null, null);
        }
    }
}
