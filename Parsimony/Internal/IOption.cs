using System;

namespace Parsimony.Internal
{
    /// <summary>
    /// The interface for an option within a typed set.
    /// </summary>
    /// <typeparam name="TOptions">The type of the option set.</typeparam>
    internal interface IOption<TOptions> where TOptions : notnull
    {
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
        public bool IsFlag { get; }

        /// <summary>
        /// Parses <paramref name="input"/> value and returns an action that assigns the result to an option set.
        /// </summary>
        /// <param name="input">The input to parse.</param>
        /// <returns>An action that assigns the parsed value to an option set.</returns>
        public Action<TOptions> Parse(string input);
    }
}
