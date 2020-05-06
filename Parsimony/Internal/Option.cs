using Parsimony.Errors;
using Parsimony.Exceptions;
using System;

namespace Parsimony.Internal
{
    /// <summary>
    /// The internal representation of an option within a set.
    /// </summary>
    /// <typeparam name="TOptionSet">The type of the set this option belongs to.</typeparam>
    /// <typeparam name="TOptionValue">The type of this option's value.</typeparam>
    internal class Option<TOptionSet, TOptionValue> : IOption<TOptionSet> where TOptionSet : class
    {
        private readonly Func<string, TOptionValue> _valueParser;
        private readonly Action<TOptionSet, TOptionValue> _assignment;

        /// <inheritdoc/>
        public OptionName.Short? ShortName { get; }

        /// <inheritdoc/>
        public OptionName.Long? LongName { get; }

        /// <inheritdoc/>
        public bool IsFlag { get; } = typeof(TOptionValue) == typeof(bool);

        /// <summary>
        /// Creates a new <see cref="Option{TOptionSet, TOptionValue}"/>.
        /// </summary>
        /// <param name="shortName">The option's short name.</param>
        /// <param name="longName">The option's long name.</param>
        /// <param name="parseValue">
        /// The function to use when parsing the option's value. This function should throw an exception to indicate a
        /// parsing failure. The exceptions message will be used as the message for the
        /// <see cref="OptionValueFormatError"/> returned by <see cref="GetAssignment(string)"/>.
        /// </param>
        /// <param name="assignValue">
        /// The function to use to assign the parsed value into the <typeparamref name="TOptionSet"/>.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="parseValue"/> or <paramref name="assignValue"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// Both <paramref name="shortName"/> and <paramref name="longName"/> are both <c>null</c>.
        /// </exception>
        internal Option(
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
                    $"At least one of {nameof(shortName)} and {nameof(longName)} must be non-null.");
            }
        }

        /// <inheritdoc/>
        public (Action<TOptionSet>?, OptionValueFormatError?) GetAssignment(string input)
        {
            if (input == null) throw new ArgumentNullException(nameof(input));

            try
            {
                var parsedValue = _valueParser(input);
                return (opts => _assignment(opts, parsedValue), null);
            }
            catch (Exception ex)
            {
                // At least one of LongName and ShortName is non-null
                var name = LongName as OptionName ?? ShortName as OptionName;
                if (name == null)
                {
                    var longNameName = nameof(Option<TOptionSet, TOptionValue>.LongName);
                    var shortNameName = nameof(Option<TOptionSet, TOptionValue>.ShortName);
                    throw new LogicErrorException($"Both {longNameName} and {shortNameName} were null.");
                }

                var message = ex.Message; // TODO: Support a configurable message for the option?
                var error = new OptionValueFormatError(name, input, message);
                return (null, error);
            }
        }
    }
}
