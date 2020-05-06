using System;

namespace Parsimony.Exceptions
{
    /// <summary>
    /// Thrown when there's a bug in Parsimony.
    /// </summary>
#pragma warning disable CA1032 // Implement standard exception constructors
    public class LogicErrorException : Exception
#pragma warning restore CA1032
    {
        internal LogicErrorException(string message) : base(message) { }

        internal LogicErrorException(string message, Exception innerException) : base(message, innerException) { }
    }
}
