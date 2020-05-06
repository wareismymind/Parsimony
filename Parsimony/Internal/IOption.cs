using Parsimony.Errors;
using System;

namespace Parsimony.Internal
{
    /// <summary>
    /// The interface for an option within a typed set.
    /// </summary>
    /// <typeparam name="TOptionSet">The type of the set this option belongs to.</typeparam>
    internal interface IOption<TOptionSet> where TOptionSet : class
    {
        /// <summary>
        /// The short name of the option.
        /// </summary>
        /// <remarks>
        /// At least one of <see cref="ShortName"/> and <see cref="LongName"/> must be non-<c>null</c>.
        /// </remarks>
        OptionName.Short? ShortName { get; }

        /// <summary>
        /// The long name of the option.
        /// </summary>
        /// <remarks>
        /// At least one of <see cref="ShortName"/> and <see cref="LongName"/> must be non-<c>null</c>.
        /// </remarks>
        OptionName.Long? LongName { get; }

        /// <summary>
        /// Indicates whether option is a flag/<see cref="bool"/> or not.
        /// </summary>
        bool IsFlag { get; }

        /// <summary>
        /// Parses <paramref name="input"/> value and returns an action that assigns the result to an option set, or an
        /// <see cref="OptionValueFormatError"/> if <paramref name="input"/> is not a valid value for the option.
        /// </summary>
        /// <param name="input">The input to parse.</param>
        /// <returns>The assignment action or error.</returns>
        (Action<TOptionSet>?, OptionValueFormatError?) GetAssignment(string input);
    }
}
