using System;

namespace Parsimony
{
    /// <summary>
    /// An error that occurs while parsing an option set.
    /// </summary>
    public abstract class OptionParsingError
    {
        /// <summary>
        /// A friendly description of the error.
        /// </summary>
        public string Message { get; }

        internal OptionParsingError(string message)
        {
            Message = message ?? throw new ArgumentNullException(nameof(message));
        }
    }
}
