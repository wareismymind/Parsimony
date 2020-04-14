using System;
using System.Collections.Generic;
using System.Linq;

namespace Parsimony
{
    /// <summary>
    /// The result of parsing a token stream for an option set.
    /// </summary>
    /// <remarks>
    /// Parsing errors are represented by a non-<c>null</c> value for <see cref="Error"/> and
    /// <c>null</c> for <see cref="OptionSet"/> and <see cref="Arguments"/>. Successful parse
    /// operations are represented by a <c>null</c> <see cref="Error"/> and non-<c>null</c> values
    /// for <see cref="OptionSet"/> and <see cref="Arguments"/>.
    /// </remarks>
    public class OptionParseResult<TOptionSet> where TOptionSet : class
    {
        /// <summary>
        /// The error, if any, that occurred while parsing.
        /// </summary>
        public OptionParsingError? Error { get; }

        /// <summary>
        /// The option set that was parsed.
        /// </summary>
        public TOptionSet? OptionSet { get; }

        /// <summary>
        /// The non-option tokens from the input.
        /// </summary>
        public IEnumerable<string>? Arguments { get; }

        /// <summary>
        /// Creates a new error-representing <see cref="OptionParseResult{TOptionSet}"/>.
        /// </summary>
        /// <param name="error">The error.</param>
        internal OptionParseResult(OptionParsingError error)
        {
            Error = error ?? throw new ArgumentNullException(nameof(error));
        }

        /// <summary>
        /// Creates a new <see cref="OptionParseResult{TOptionSet}"/>.
        /// </summary>
        /// <param name="optionSet">The option set that was parsed.</param>
        /// <param name="arguments">The non-option tokens from the input.</param>
        internal OptionParseResult(TOptionSet optionSet, IEnumerable<string> arguments)
        {
            OptionSet = optionSet ?? throw new ArgumentNullException(nameof(optionSet));
            Arguments = arguments?.ToList() ?? throw new ArgumentNullException(nameof(arguments));
        }
    }
}
