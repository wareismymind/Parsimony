using System;
using System.Collections.Generic;

namespace Parsimony.Options
{
    /// <summary>
    /// Parses a set of CLI options.
    /// </summary>
    /// <typeparam name="TOptionSet">The type of the option set.</typeparam>
    /// <remarks>
    /// The <see cref="OptionSet{TOptionSet}"/> type builds <see cref="IOptionSetParser{TOptionSet}"/> instances for
    /// the options it contains.
    /// </remarks>
    public interface IOptionSetParser<TOptionSet> where TOptionSet : class
    {
        /// <summary>
        /// Parses <paramref name="input"/> for the options.
        /// </summary>
        /// <param name="input">The token stream to parse.</param>
        /// <param name="config">The parsing options.</param>
        /// <returns>A <see cref="ParseResult{TOptions}"/>.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="input"/> or <paramref name="config"/> is <c>null</c>.
        /// </exception>
        ParseResult<TOptionSet> Parse(IEnumerable<string> input, ParserConfig config);
    }

    /// <summary>
    /// Adds a method to <see cref="IOptionSetParser{TOptions}"/> to use a default <see cref="ParserConfig"/>.
    /// </summary>
    public static class IParserExtensions
    {
        /// <summary>
        /// Parses <paramref name="input"/> for the options.
        /// </summary>
        /// <param name="target">The <see cref="IOptionSetParser{TOptions}"/>.</param>
        /// <param name="input">The token stream to parse.</param>
        /// <returns>A <see cref="ParseResult{TOptions}"/>.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="target"/> or <paramref name="input"/> is <c>null</c>.
        /// </exception>
        public static ParseResult<T> Parse<T>(this IOptionSetParser<T> target, IEnumerable<string> input) where T : class
        {
            if (target == null) throw new ArgumentNullException(nameof(target));
            return target.Parse(input, new ParserConfig());
        }
    }
}
