using Parsimony.Internal;
using System;

namespace Parsimony
{
    /// <summary>
    /// Indicates that an unknown option was encountered while parsing.
    /// </summary>
    public class UnknownOptionError : OptionParsingError
    {
        // TODO: Expose a factory type that accepts a builder function mapping option name to message that can be used
        // to configure the unknown option message.

        /// <summary>
        /// The name of the unknown option.
        /// </summary>
        public string OptionName { get; }

        internal UnknownOptionError(OptionName optionName) : base($"unknown option '{optionName}'")
        {
            OptionName = optionName ?? throw new ArgumentNullException(nameof(optionName));
        }
    }
}
