using Parsimony.Errors;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Parsimony.Options
{
    /// <summary>
    /// The result of parsing CLI options.
    /// </summary>
    /// <remarks>
    /// Parsing errors are represented by a non-<c>null</c> value for <see cref="Error"/> and
    /// <c>null</c> for <see cref="Options"/> and <see cref="Arguments"/>. Successful parse
    /// operations are represented by a <c>null</c> <see cref="Error"/> and non-<c>null</c> values
    /// for <see cref="Options"/> and <see cref="Arguments"/>.
    /// </remarks>
    public class ParseResult<TOptions> where TOptions : class
    {
        /// <summary>
        /// The error, if any, that occurred while parsing.
        /// </summary>
        public OptionParsingError? Error { get; }

        /// <summary>
        /// Assignment actions for the options that were parsed.
        /// </summary>
        public TOptions? Options { get; }

        /// <summary>
        /// The non-option tokens from the input.
        /// </summary>
        public IEnumerable<string>? Arguments { get; }

        internal ParseResult(OptionParsingError error)
        {
            Error = error ?? throw new ArgumentNullException(nameof(error));
        }

        internal ParseResult(TOptions options, IEnumerable<string> arguments)
        {
            Options = options ?? throw new ArgumentNullException(nameof(options));
            Arguments = arguments?.ToList() ?? throw new ArgumentNullException(nameof(arguments));
        }
    }
}
