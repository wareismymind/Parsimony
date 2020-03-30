using System;

namespace Parsimony
{
    /// <summary>
    /// A specification for a CLI option.
    /// </summary>
    /// <typeparam name="TOptions">The type of the option set.</typeparam>
    /// <typeparam name="TValue">The type of this option's value.</typeparam>
    public class OptionSpec<TOptions, TValue>
        : OptionSpec<TOptions>
        where TOptions : notnull, new()
    {
        private readonly Func<string, TValue> _parseFn;

        private readonly Action<TOptions, TValue> _setFn;

        /// <summary>
        /// The default value for the option. If <see cref="OptionSpec{TOptions}.Required"/> is
        /// <c>true</c> this value  will be ignored.
        /// </summary>
        public TValue Default { get; private set; }

        /// <summary>
        /// The <see cref="Type"/> of the option.
        /// </summary>
        public override Type OptionType => typeof(TValue);

        // TODO: Create constructors for valid configurations?

        /// <summary>
        /// Creates a new <see cref="OptionSpec{TOptions,TValue}"/>.
        /// </summary>
        /// <param name="shortName">The short name of the option.</param>
        /// <param name="longName">The long name of the option.</param>
        /// <param name="required">Indicates whether the option is required.</param>
        /// <param name="default">
        /// The default value for the option (if 'required' is false).
        /// </param>
        /// <param name="parseFn">The function to use when parsing the option value.</param>
        /// <param name="setFn">
        /// The procedure to use to assign the option value to the <typeparamref name="TOptions"/>.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="parseFn"/> or <paramref name="setFn"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// Both <paramref name="shortName"/> and <paramref name="longName"/> are <c>null</c>.
        /// </exception>
        public OptionSpec(
            char? shortName,
            string? longName,
            bool required,
            TValue @default,
            Func<string, TValue> parseFn,
            Action<TOptions, TValue> setFn)
            : base(shortName, longName, required)
        {
            Default = @default;
            _parseFn = parseFn ?? throw new ArgumentNullException(nameof(parseFn));
            _setFn = setFn ?? throw new ArgumentNullException(nameof(setFn));
        }

        /// <summary>
        /// The function to use when parsing the option value.
        /// </summary>
        public TValue Parse(string input) => _parseFn(input);

        /// <summary>
        /// The procedure to use to assign the option's value to the option set.
        /// </summary>
        public void Set(TOptions options, TValue value) => _setFn(options, value);
    }

    /// <summary>
    /// A specification for a CLI option.
    /// </summary>
    /// <typeparam name="TOptions">The type of the option set.</typeparam>
    public abstract class OptionSpec<TOptions> where TOptions : new()
    {
        /// <summary>
        /// The <see cref="Type"/> of the option.
        /// </summary>
        public abstract Type OptionType { get; }

        /// <summary>
        /// The short name of the option.
        /// </summary>
        public char? ShortName { get; private set; }

        /// <summary>
        /// The long name of the option.
        /// </summary>
        public string? LongName { get; private set; }

        /// <summary>
        /// Indicates whether the option is required.
        /// </summary>
        public bool Required { get; private set; }

        /// <summary>
        /// Creates a new <see cref="OptionSpec{TOptions,TValue}"/>.
        /// </summary>
        /// <param name="shortName">The short name of the option.</param>
        /// <param name="longName">The long name of the option.</param>
        /// <param name="required">Indicates whether the option is required.</param>
        /// <exception cref="ArgumentException">
        /// Both <paramref name="shortName"/> and <paramref name="longName"/> are <c>null</c>.
        /// </exception>
        private protected OptionSpec(
            char? shortName,
            string? longName,
            bool required)
        {
            ShortName = shortName;
            LongName = longName;
            Required = required;

            if (ShortName == null && LongName == null)
            {
                throw new ArgumentException(
                    $"Either {nameof(shortName)} or {nameof(longName)} must have a value");
            }
        }
    }
}
