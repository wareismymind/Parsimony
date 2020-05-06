
using Parsimony.Internal;

namespace Parsimony.Errors
{
    /// <summary>
    /// Indicates that an unknown option was encountered while parsing.
    /// </summary>
    public class UnknownOptionError : OptionParsingError
    {
        internal UnknownOptionError(OptionName optionName) : base(optionName) { }
    }
}
