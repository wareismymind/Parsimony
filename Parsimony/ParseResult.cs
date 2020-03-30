using System;
using System.Collections.Generic;

namespace Parsimony
{
    /// <summary>
    /// The result of parsing for a <typeparamref name="T"/>.
    /// </summary>
    public class ParseResult<T> where T : notnull, new()
    {
        /// <summary>
        /// The <typeparamref name="T"/> containing the option values.
        /// </summary>
        public T Options { get; private set; }

        /// <summary>
        /// The positional argument values.
        /// </summary>
        /// <remarks>
        /// Positional arguments are all the tokens from the input that the parser did not identify
        /// as an option name, option value, or the "--" token when supported. Depending on the
        /// parser options used, this may include unrecognized options, positional arguments, or
        /// subcommands and their options and arguments.
        /// </remarks>
        public IEnumerable<string> Arguments { get; private set; }

        /// <summary>
        /// Creates a new <see cref="ParseResult{T}"/>.
        /// </summary>
        /// <param name="options">The <typeparamref name="T"/> containing the option values.</param>
        /// <param name="arguments">The non-option</param>
        public ParseResult(T options, IEnumerable<string> arguments)
        {
            Options = options ?? throw new ArgumentNullException(nameof(options));
            Arguments = arguments ?? throw new ArgumentNullException(nameof(arguments));
        }
    }
}
