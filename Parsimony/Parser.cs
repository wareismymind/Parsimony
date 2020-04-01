using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Parsimony
{
    /// <summary>
    /// A CLI parser for <typeparamref name="T"/>
    /// </summary>
    /// <typeparam name="T">The type of the options to parse.</typeparam>
    public class Parser<T> where T : notnull, new()
    {
        private class Option
        {
            public OptionSpec<T> Spec { get; private set; }

            public bool Set { get; set; }

            public Option(OptionSpec<T> spec)
            {
                Spec = spec ?? throw new ArgumentNullException(nameof(spec));
            }
        }

        private class ParserState
        {
            public ParseResult<T> Result { get; }

            public List<string> Input { get; }

            public ParserState(ParseResult<T> result, IEnumerable<string> input)
            {
                Result = result ?? throw new ArgumentNullException(nameof(result));
                Input = input?.ToList() ?? throw new ArgumentNullException(nameof(input));
            }
        }

        private static readonly Regex _shortNameOptionPatern =
            new Regex("^-(?<option>[A-Za-z])(?<rest>.+)?");

        private readonly List<Option> _options;

        private readonly ParserConfig _configuration;

        /// <summary>
        /// Creates a new <see cref="Parser{T}"/>.
        /// </summary>
        /// <param name="optionSpecs">The option specifications.</param>
        /// <param name="configuration">The parser configuration.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="optionSpecs"/> or <paramref name="configuration"/> is <c>null</c>.
        /// </exception>
        public Parser(IEnumerable<OptionSpec<T>> optionSpecs, ParserConfig configuration)
        {
            _options = (optionSpecs ?? throw new ArgumentNullException(nameof(optionSpecs)))
                .Select(spec => new Option(spec)).ToList();

            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));

            // TODO: Validate specs for duplicate long/short names.
        }

        /// <summary>
        /// Parses a <typeparamref name="T"/> from <paramref name="input"/>.
        /// </summary>
        /// <param name="input">The tokens to parse.</param>
        /// <returns>The <see cref="ParseResult{T}"/>.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="input"/> is <c>null</c>.
        /// </exception>
        public ParseResult<T> Parse(IEnumerable<string> input)
        {
            var tokens = (input ?? throw new ArgumentNullException(nameof(input))).ToList();

            var options = new T();
            var arguments = new List<string>(tokens.Count);
            var result = new ParseResult<T>(options, arguments);
            return Parse(result, tokens);
        }

        /// <summary>
        /// Parses a <typeparamref name="T"/> from <paramref name="input"/>.
        /// </summary>
        /// <param name="input">The tokens to parse.</param>
        /// <returns>The <see cref="ParseResult{T}"/>.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="input"/> is <c>null</c>.
        /// </exception>
        public ParseResult<T> Parse(params string[] input)
        {
            return Parse(input as IEnumerable<string>);
        }

        private ParseResult<T> Parse(ParseResult<T> result, List<string> input)
        {
            var state = new ParserState(result, input);
            while (state.Input.Count > 0)
                state = Parse(state);
            return state.Result;
        }

        private ParserState Parse(ParserState state)
        {
            return ConsumeShortNameOption(state)
                ?? ConsumeLongNameOption(state)
                ?? ConsumeArgument(state)
                ?? state;
        }

        private ParserState? ConsumeShortNameOption(ParserState state)
        {
            var options = state.Result.Options;
            var arguments = state.Result.Arguments.ToList();
            var input = state.Input;

            if (input.Count == 0)
                return null;

            var match = _shortNameOptionPatern.Match(input[0]);
            if (!match.Success)
                return null;

            var optionName = match.Groups["option"].Value[0];
            var option = _options.FirstOrDefault(o => o.Spec.ShortName == optionName);
            if (option == null)
                // TODO: Custom exception
                // TODO: Support for unknown options as arguments
                throw new Exception($"Unknown argument '-{optionName}'");

            // Consume the token
            input = input.Skip(1).ToList();

            var optionType = option.Spec.OptionType;
            var typedOption = typeof(OptionSpec<,>).MakeGenericType(new[] { typeof(T), optionType });
            var set = typedOption.GetMethod("Set", new[] { typeof(T), optionType });

            if (optionType == typeof(bool))
            {
                set.Invoke(option.Spec, new object[] { options, true });
                option.Set = true;

                if (match.Groups["rest"].Success)
                    input.Insert(1, $"-{match.Groups["rest"].Value}");

                return new ParserState(new ParseResult<T>(options, arguments), input);
            }

            string nextToken()
            {
                if (input.Count == 0)
                    // TODO: Custom exception
                    throw new Exception($"Option -{optionName} requires an argument");

                var value = input[0];
                input.RemoveAt(0);
                return value;
            }

            var valueToken = match.Groups["rest"].Success
                ? match.Groups["rest"].Value
                : nextToken();

            var parse = typedOption.GetMethod("Parse", new[] { typeof(string) });
            var value = parse.Invoke(option.Spec, new object[] { valueToken });
            set.Invoke(option.Spec, new object[] { options, value });
            option.Set = true;
            return new ParserState(new ParseResult<T>(options, arguments), input);
        }

        private ParserState? ConsumeLongNameOption(ParserState state)
        {
            return null;
        }

        private ParserState? ConsumeArgument(ParserState state)
        {
            if (state.Input.Count == 0)
                return null;

            if (_configuration.DoubleDash && state.Input[0] == "--" || _configuration.OptionsFirst)
            {
                return ConsumeAllArguments(state);
            }

            var options = state.Result.Options;
            var arguments = state.Result.Arguments.ToList();
            arguments.Add(state.Input[0]);
            var input = state.Input.Skip(1);

            return new ParserState(new ParseResult<T>(options, arguments), input);
        }

        private ParserState? ConsumeAllArguments(ParserState state)
        {
            if (state.Input.Count == 0)
                return null;

            if (_configuration.DoubleDash)
            {
                var index = state.Input.IndexOf("--");
                if (index >= 0)
                    state.Input.RemoveAt(index);
            }

            var arguments = state.Result.Arguments.ToList();
            arguments.AddRange(state.Input);
            return new ParserState(
                new ParseResult<T>(state.Result.Options, arguments), Array.Empty<string>());
        }
    }
}
