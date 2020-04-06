using System;
using System.Collections.Generic;
using System.Linq;

namespace Parsimony
{
    /// <summary>
    /// The internal representation of an option within a set.
    /// </summary>
    internal class Option<TOptions, TValue> where TOptions : notnull
    {
        private readonly string? _shortSelector;
        private readonly string? _longSelector;
        private readonly string[] _optionSelectors;

        private bool IsFlag { get; } = typeof(TValue) == typeof(bool);
        private bool IsValueOption => !IsFlag;

        private readonly Func<string, TValue> _valueParser;
        private readonly Action<TOptions, TValue> _assignment;

        /// <summary>
        /// The option's name.
        /// </summary>
        public OptionName Name { get; }

        /// <summary>
        /// Creates a new <see cref="Option{TOptions, TValue}"/>.
        /// </summary>
        /// <param name="name">The option's name.</param>
        /// <param name="valueParser">The function to use when parsing the option's value.</param>
        /// <param name="assignment">
        /// The function to use to assign the parsed value into the <typeparamref name="TOptions"/>.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="name"/>, <paramref name="valueParser"/>, or <paramref name="assignment"/> is <c>null</c>.
        /// </exception>
        public Option(OptionName name, Func<string, TValue> valueParser, Action<TOptions, TValue>  assignment)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            _valueParser = valueParser ?? throw new ArgumentNullException(nameof(valueParser));
            _assignment = assignment ?? throw new ArgumentNullException(nameof(assignment));

            _shortSelector = Name.ShortName != null ? $"-{Name.ShortName}" : null;
            _longSelector = Name.LongName != null ? $"--{Name.LongName}" : null;
#pragma warning disable CS8619 // Nullability of reference types in value doesn't match target type.
            _optionSelectors = new[] { _shortSelector, _longSelector }.Where(n => n != null).ToArray();
#pragma warning restore CS8619 // Nullability of reference types in value doesn't match target type.
        }

        public bool CanParse(ParseContext<TOptions> context)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));

            return ParseNameAndValue(context.Input).NameAndValue != null;
        }

        /// <summary>
        /// Attempts to parse the target option from the <see cref="ParseContext{TOptions}.Input"/> of
        /// <paramref name="context"/>.
        /// </summary>
        /// <param name="context">The parse context.</param>
        /// <returns>A <see cref="ParseResult{TOptions, TError}"/>.</returns>
        public ParseResult<TOptions, string> Parse(ParseContext<TOptions> context)
        {
            return new ParseResult<TOptions, string>(context, "not implemented");
        }

        private NameAndValueResult ParseNameAndValue(IEnumerable<string> input)
        {
            var tokens = input.ToList() as IReadOnlyList<string>;

            if (tokens.Count == 0)
                return new NameAndValueResult(null, input);

            return ParseStandaloneFlag(tokens)
                ?? ParseAdjoinedShortNameFlag(tokens)
                ?? ParseSpaceSeparatedOptionAndValue(tokens)
                ?? ParseAdjoinedShortNameOptionAndValue(tokens)
                ?? ParseEqualsJoinedOptionAndValue(tokens)
                ?? new NameAndValueResult(null, input);
        }

        private NameAndValueResult? ParseStandaloneFlag(IReadOnlyList<string> tokens)
        {
            // A standalone flag is token that contains only the short or long name selector of a flag type option.
            if (IsFlag && _optionSelectors.Contains(tokens[0]))
            {
                // Consume the first token as the name and provide a value of "true".
                return new NameAndValueResult(new NameAndValue(tokens[0], "true"), tokens.Skip(1));
            }

            return null;
        }

        private NameAndValueResult? ParseAdjoinedShortNameFlag(IReadOnlyList<string> tokens)
        {
            // An adjoined short name flag is a token that begins with the short name selector of a flag type option
            // and has additional characters appended. The additional characters must be the short names of zero or
            // more additional flag type options optionally followed by the short name of a value type option which is
            // then followed optionally by the value for said option, but this condition will be enforce incrementally
            // as those characters are consumed.
            if (IsFlag && _shortSelector != null && tokens[0].StartsWith(_shortSelector) && tokens[0].Length > 2)
            {
                // Replace the first token with a hyphen prepended to the characters following the short name selector
                // and return the short name selector as the name and a value of "true".
                var input = new List<string> { $"-{tokens[0].Substring(2)}" };
                input.AddRange(tokens.Skip(1));
                return new NameAndValueResult(new NameAndValue(_shortSelector, "true"), input);
            }

            return null;
        }

        private NameAndValueResult? ParseSpaceSeparatedOptionAndValue(IReadOnlyList<string> tokens)
        {
            // A space-separated option and value is token that contains only the short or long name selector of a
            // value type option followed by a token than contains the options's value.
            if (IsValueOption && _optionSelectors.Contains(tokens[0]) && tokens.Count >= 2)
            {
                // Consume the first two tokens as the name and value respectively.
                return new NameAndValueResult(new NameAndValue(tokens[0], tokens[1]), tokens.Skip(2));
            }

            return null;
        }

        private NameAndValueResult? ParseAdjoinedShortNameOptionAndValue(IReadOnlyList<string> tokens)
        {
            // An adjoined short name option and value is a token that begins with the short name selector of a value
            // type option and has additional characters that constitue the option's value.
            if (IsValueOption && _shortSelector != null && tokens[0].StartsWith(_shortSelector) && tokens[0].Length > 2)
            {
                // Consume the first 2 characters of the first token as the name, and the remained of that token as the
                // value.
                return new NameAndValueResult(new NameAndValue(_shortSelector, tokens[0].Substring(2)), tokens.Skip(1));
            }

            return null;
        }

        private NameAndValueResult? ParseEqualsJoinedOptionAndValue(IReadOnlyList<string> tokens)
        {
            // An equals-joined option and value is a token that begins with the long name selector of an option
            // followed by an '=' then followed by the options value.
            if (_longSelector != null)
            {
                var prefix = $"{_longSelector}=";
                if (tokens[0].StartsWith(prefix) && tokens[0].Length > prefix.Length)
                {
                    // Consume the first token and return the long name selector as the name and everything after the
                    // first '=' as the value.
                    return new NameAndValueResult(
                        new NameAndValue(_longSelector, tokens[0].Substring(prefix.Length)), tokens.Skip(1));
                }
            }

            return null;
        }

        private class NameAndValue
        {
            public string Name { get; }
            public string Value { get; }
            public NameAndValue(string name, string value)
            {
                Name = name ?? throw new ArgumentNullException(nameof(name));
                Value = value ?? throw new ArgumentNullException(nameof(value));
            }
        }

        private class NameAndValueResult
        {
            public NameAndValue? NameAndValue { get; }
            public IReadOnlyList<string> Input { get; }
            public NameAndValueResult(NameAndValue? nameAndValue, IEnumerable<string> input)
            {
                NameAndValue = nameAndValue;
                Input = input?.ToList() ?? throw new ArgumentNullException(nameof(input));
            }
        }
    }
}
