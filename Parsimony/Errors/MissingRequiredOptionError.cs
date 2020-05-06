using Parsimony.Internal;
using System;

namespace Parsimony.Errors
{
    /// <summary>
    /// Indicates that a required option was not found while parsing.
    /// </summary>
    public class MissingRequiredOptionError : OptionParsingError
    {
        /// <summary>
        /// The option that requires <see cref="OptionParsingError.OptionName"/>.
        /// </summary>
        public string RequiredBy { get; }

        internal MissingRequiredOptionError(OptionName optionName, OptionName requiredBy) : base(optionName)
        {
            RequiredBy = requiredBy ?? throw new ArgumentNullException(nameof(optionName));
        }
    }
}
