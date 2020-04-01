using System;
using System.Collections.Generic;

namespace Parsimony
{
    /// <summary>
    /// The state while parsing for a <typeparamref name="TOptions"/>.
    /// </summary>
    public class ParserState<TOptions> where TOptions : notnull
    {
        /// <summary>
        /// The cumulative result of the parse operation.
        /// </summary>
        public ParseResult<TOptions> Result { get; }

        /// <summary>
        /// The remaining input to parse.
        /// </summary>
        public IEnumerable<string> Input { get; }

        /// <summary>
        /// Creates a new <see cref="ParserState{TOptions}"/>.
        /// </summary>
        /// <param name="result">The cumulative result of the parse operation.</param>
        /// <param name="input">The remaining input to parse.</param>
        public ParserState(ParseResult<TOptions> result, IEnumerable<string> input)
        {
            Result = result ?? throw new ArgumentNullException(nameof(result));
            Input = input ?? throw new ArgumentNullException(nameof(input));
        }
    }
}
