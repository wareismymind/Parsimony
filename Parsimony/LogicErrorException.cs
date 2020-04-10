using System;

namespace Parsimony
{
    /// <summary>
    /// Thrown when the developers made a mistake.
    /// </summary>
    public class LogicErrorException : Exception
    {
        /// <summary>
        /// Creates a new <see cref="LogicErrorException"/> with the given message.
        /// </summary>
        /// <param name="message">The message.</param>
        public LogicErrorException(string message) : base(message) { }

        /// <summary>
        /// Creates a new <see cref="LogicErrorException"/> with the given message and inner exception.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="innerException">The inner exception that caused this exception.</param>
        public LogicErrorException(string message, Exception innerException) : base(message, innerException) { }
    }
}
