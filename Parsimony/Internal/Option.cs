using System;

namespace Parsimony.Internal
{
    /// <summary>
    /// The internal representation of an option within a set.
    /// </summary>
    /// <typeparam name="TOptionSet">The type of the set this option belongs to.</typeparam>
    /// <typeparam name="TOptionValue">The type of the value of this option.</typeparam>
    internal class Option<TOptionSet, TOptionValue>
        : IOption<TOptionSet>
        where TOptionSet : class
    {
        private readonly Func<string, TOptionValue> _valueParser;
        private readonly Action<TOptionSet, TOptionValue> _assignment;

        /// <summary>
        /// The option's short name.
        /// </summary>
        public OptionName.Short? ShortName { get; }

        /// <summary>
        /// The option's long name.
        /// </summary>
        public OptionName.Long? LongName { get; }

        /// <summary>
        /// Indicates whether the option is a flag (bool) type.
        /// </summary>
        public bool IsFlag { get; } = typeof(TOptionValue) == typeof(bool);

        /// <summary>
        /// Creates a new <see cref="Option{TOptions, TValue}"/>.
        /// </summary>
        /// <param name="shortName">The option's short name.</param>
        /// <param name="longName">The option's long name.</param>
        /// <param name="parseValue">The function to use when parsing the option's value.</param>
        /// <param name="assignValue">
        /// The function to use to assign the parsed value into the <typeparamref name="TOptionSet"/>.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="parseValue"/> or <paramref name="assignValue"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// Both <paramref name="shortName"/> and <paramref name="longName"/> are <c>null</c>.
        /// </exception>
        public Option(
            OptionName.Short? shortName,
            OptionName.Long? longName,
            Func<string, TOptionValue> parseValue,
            Action<TOptionSet, TOptionValue> assignValue)
        {
            ShortName = shortName;
            LongName = longName;
            _valueParser = parseValue ?? throw new ArgumentNullException(nameof(parseValue));
            _assignment = assignValue ?? throw new ArgumentNullException(nameof(assignValue));
            if (ShortName == null && LongName == null)
            {
                throw new ArgumentException(
                    $"At least one of {nameof(shortName)} and {nameof(longName)} must be non-null");
            }
        }

        /// <summary>
        /// Parses <paramref name="input"/> value and returns an action that assigns the result to an option set.
        /// </summary>
        /// <param name="input">The input to parse.</param>
        /// <returns>An action that assigns the parsed value to an option set.</returns>
        public Action<TOptionSet> Parse(string input)
        {
            if (input == null) throw new ArgumentNullException(nameof(input));
            // TODO: Communicate error deliberately
            var parsedValue = _valueParser(input);
            return opts => _assignment(opts, parsedValue);
        }
    }
}
