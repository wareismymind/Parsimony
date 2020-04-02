using System;
using System.Linq;

namespace Parsimony
{
    /// <summary>
    /// A parser that consumes the next token as a <typeparamref name="TValue"/>.
    /// </summary>
    public class OptionValueParser<TOptions, TValue>
        : IParser<TOptions>
        where TOptions : notnull
    {
        private readonly Func<string, TValue> _parse;

        private readonly Action<TOptions, TValue> _assign;

        /// <summary>
        /// Creates a new <see cref="OptionValueParser{TOptions, TValue}"/>.
        /// </summary>
        /// <param name="parse">
        /// The function to use to parse the <typeparamref name="TValue"/> from the input token.
        /// </param>
        /// <param name="assign">
        /// The action to use to assign the <typeparamref name="TValue"/> to the
        /// <typeparamref name="TOptions"/>.
        /// </param>
        public OptionValueParser(Func<string, TValue> parse, Action<TOptions, TValue> assign)
        {
            _parse = parse ?? throw new ArgumentNullException(nameof(parse));
            _assign = assign ?? throw new ArgumentNullException(nameof(assign));
        }

        /// <summary>
        /// Parses the next token from the input as a <typeparamref name="TValue"/>.
        /// </summary>
        /// <param name="state">The current parser state.</param>
        /// <returns>The new state.</returns>
        public ParserState<TOptions> Parse(ParserState<TOptions> state)
        {
            if (state == null) throw new ArgumentNullException(nameof(state));
            var input = state.Input.ToList();
            if (input.Count == 0)
                // TODO: Custom exception
                throw new Exception("Input was empty");

            var value = Parse(input.First());
            Assign(state.Result.Options, value);

            return new ParserState<TOptions>(state.Result, state.Input.Skip(1));
        }

        private TValue Parse(string input)
        {
            try
            {
                return _parse(input);
            }
            catch (Exception ex)
            {
                // TODO: Custom exception
                throw new Exception($"Invalid value for {typeof(TValue).Name} option", ex);
            }
        }

        private void Assign(TOptions options, TValue value)
        {
            try
            {
                _assign(options, value);
            }
            catch (Exception ex)
            {
                // TODO: Custom exception
                throw new Exception($"Assignment of {typeof(TValue).Name} to {typeof(TOptions).Name} failed", ex);
            }
        }
    }
}
