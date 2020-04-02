using System;
using System.Linq;

namespace Parsimony
{
    /// <summary>
    /// A parser that consumes all of the remaining input as arguments.
    /// </summary>
    public class ArgumentsParser<TOptions> where TOptions : notnull
    {
        /// <summary>
        /// Parses the remaining arguments from the input.
        /// </summary>
        /// <param name="state">The current parser state.</param>
        /// <returns>The new state.</returns>
        public ParserState<TOptions> Parse(ParserState<TOptions> state)
        {
            if (state == null) throw new ArgumentNullException(nameof(state));
            var arguments = state.Result.Arguments.ToList();
            arguments.AddRange(state.Input);
            var result = new ParseResult<TOptions>(state.Result.Options, arguments);
            return new ParserState<TOptions>(result, Array.Empty<string>());
        }
    }
}
