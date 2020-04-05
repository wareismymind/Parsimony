using System;
using System.Collections.Generic;
using System.Linq;

namespace Parsimony
{
    // TODO: Better summary

    /// <summary>
    /// The context of a parse operation.
    /// </summary>
    /// <typeparam name="TOptions">The type of the option set being parsed.</typeparam>
    internal class ParseContext<TOptions> where TOptions : notnull
    {
        /// <summary>
        /// The assignments to make to the <typeparamref name="TOptions"/>.
        /// </summary>
        public IReadOnlyList<Action<TOptions>> Assignments { get; }

        /// <summary>
        /// The positional arguments.
        /// </summary>
        public IReadOnlyList<string> Arguments { get; }

        /// <summary>
        /// The input to parse.
        /// </summary>
        public IReadOnlyList<string> Input { get;  }

        /// <summary>
        /// Creates a new <see cref="ParseContext{TOptions}"/>.
        /// </summary>
        /// <param name="assignments">The actions to perform on the <typeparamref name="TOptions"/>.</param>
        /// <param name="arguments">The positional arguments.</param>
        /// <param name="input">The input to parse.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="assignments"/>, <paramref name="arguments"/>, or <paramref name="input"/> is <c>null</c>.
        /// </exception>
        public ParseContext(
            IEnumerable<Action<TOptions>> assignments, IEnumerable<string> arguments, IEnumerable<string> input)
        {
            Assignments = assignments?.ToList() ?? throw new ArgumentNullException(nameof(assignments));
            Arguments = arguments?.ToList() ?? throw new ArgumentNullException(nameof(arguments));
            Input = input?.ToList() ?? throw new ArgumentNullException(nameof(input));
        }
    }
}
