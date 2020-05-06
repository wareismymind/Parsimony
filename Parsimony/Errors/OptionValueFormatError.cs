using Parsimony.Internal;
using System;

namespace Parsimony.Errors
{
    /// <summary>
    /// Indicates that the token supplied for an option value does not represent a valid value for the option's type.
    /// </summary>
    public class OptionValueFormatError : OptionParsingError
    {
        /// <summary>
        /// The invalid value.
        /// </summary>
        public string Value { get; }

        /// <summary>
        /// A description of what would constitute a valid value for the option.
        /// </summary>
        public string Message { get; }

        internal OptionValueFormatError(OptionName optionName, string value, string message) : base(optionName)
        {
            Value = value ?? throw new ArgumentNullException(nameof(value));
            Message = message ?? throw new ArgumentNullException(nameof(value));
        }
    }
}
