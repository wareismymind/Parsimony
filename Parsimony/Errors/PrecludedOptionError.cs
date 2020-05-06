using Parsimony.Internal;
using System;

namespace Parsimony.Errors
{
    /// <summary>
    /// Indicates that a precluded option was found while parsing.
    /// </summary>
    public class PrecludedOptionError : OptionParsingError
    {
        /// <summary>
        /// The option that precludes <see cref="OptionParsingError.OptionName"/>.
        /// </summary>
        public string PrecludedBy { get; }

        internal PrecludedOptionError(OptionName optionName, OptionName precludedBy) : base(optionName)
        {
            PrecludedBy = precludedBy ?? throw new ArgumentNullException(nameof(optionName));
        }
    }
}
