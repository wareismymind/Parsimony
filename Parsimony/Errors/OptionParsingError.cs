using Parsimony.Internal;
using System;

namespace Parsimony.Errors
{
    /// <summary>
    /// An error that occurs while parsing options.
    /// </summary>
    public abstract class OptionParsingError
    {
        /// <summary>
        /// The name of the option.
        /// </summary>
        public string OptionName { get; }

        internal OptionParsingError(OptionName optionName)
        {
            OptionName = optionName ?? throw new ArgumentNullException(nameof(optionName));
        }
    }
}
