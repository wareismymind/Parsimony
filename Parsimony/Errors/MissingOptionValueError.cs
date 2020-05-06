using Parsimony.Internal;

namespace Parsimony.Errors
{
    /// <summary>
    /// Indicates that an unknown option was encountered while parsing.
    /// </summary>
    public class MissingOptionValueError : OptionParsingError
    {
        internal MissingOptionValueError(OptionName optionName) : base(optionName) { }
    }
}
