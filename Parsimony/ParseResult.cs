using System;

namespace Parsimony
{
    /// <summary>
    /// The result of an attempt to parse input.
    /// </summary>
    /// <typeparam name="TOptions">The type of the options being parsed.</typeparam>
    /// <typeparam name="TError">The type of the error that might be returned.</typeparam>
    internal class ParseResult<TOptions, TError>
        where TOptions : notnull
        where TError : class
    {
        /// <summary>
        /// The new context.
        /// </summary>
        /// <remarks>
        /// If the parsing is successful the context should reflect the result. If parsing failed the context should be
        /// unchanged compared to the context that was input the parse operation.
        /// </remarks>
        public ParseContext<TOptions> Context { get; }

        /// <summary>
        /// The error the occured, if any, during parsing.
        /// </summary>
        public TError? Error { get; }

        /// <summary>
        /// Creates a new <see cref="ParseResult{TOptions, TError}"/>.
        /// </summary>
        /// <param name="context">The new context.</param>
        public ParseResult(ParseContext<TOptions> context)
        {
            Context = context ?? throw new ArgumentNullException(nameof(context));
        }

        /// <summary>
        /// Creates a new <see cref="ParseResult{TOptions, TError}"/>.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="error">The error that occured during parsing.</param>
        public ParseResult(ParseContext<TOptions> context, TError error)
        {
            Context = context ?? throw new ArgumentNullException(nameof(context));
            Error = error ?? throw new ArgumentNullException(nameof(error));
        }
    }
}
