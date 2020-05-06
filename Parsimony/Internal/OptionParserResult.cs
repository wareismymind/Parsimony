using Parsimony.Errors;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Parsimony.Internal
{
    /// <summary>
    /// The result of parsing a token stream for an option set.
    /// </summary>
    /// <remarks>
    /// Parsing errors are represented by a non-<c>null</c> value for <see cref="Error"/> and
    /// <c>null</c> for <see cref="Assignments"/> and <see cref="Arguments"/>. Successful parse
    /// operations are represented by a <c>null</c> <see cref="Error"/> and non-<c>null</c> values
    /// for <see cref="Assignments"/> and <see cref="Arguments"/>.
    /// </remarks>
    internal class OptionParserResult<TOptions> where TOptions : class
    {
        /// <summary>
        /// The error, if any, that occurred while parsing.
        /// </summary>
        internal OptionParsingError? Error { get; }

        /// <summary>
        /// Assignment actions for the options that were parsed.
        /// </summary>
        internal IReadOnlyDictionary<IOption<TOptions>, Action<TOptions>>? Assignments { get; }

        /// <summary>
        /// The non-option tokens from the input.
        /// </summary>
        internal IEnumerable<string>? Arguments { get; }

        internal OptionParserResult(OptionParsingError error)
        {
            Error = error ?? throw new ArgumentNullException(nameof(error));
        }

        internal OptionParserResult(
            IReadOnlyDictionary<IOption<TOptions>, Action<TOptions>> assignments, IEnumerable<string> arguments)
        {
            Assignments = assignments?.ToDictionary(x => x.Key, x => x.Value) ??
                throw new ArgumentNullException(nameof(assignments));

            Arguments = arguments?.ToList() ?? throw new ArgumentNullException(nameof(arguments));
        }
    }
}
