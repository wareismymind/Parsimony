namespace Parsimony
{
    /// <summary>
    /// The parsing interface for <typeparamref name="TOptions"/>.
    /// </summary>
    public interface IParser<TOptions> where TOptions : notnull
    {
        /// <summary>
        /// Parses the next part of input into the cumulative result.
        /// </summary>
        /// <param name="state">The current parser state.</param>
        /// <returns>The new state.</returns>
        ParserState<TOptions> Parse(ParserState<TOptions> state);
    }
}
