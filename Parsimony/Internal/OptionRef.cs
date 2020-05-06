using Parsimony.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Parsimony.Internal
{
    /// <summary>
    /// A reference to an option found in a token stream.
    /// </summary>
    internal class OptionRef
    {
        private static readonly Regex _shortNamePattern =
            new Regex($"^-(?<name>{OptionName.ShortNamePattern})(?<next>.+)?$");

        private static readonly Regex _longNamePattern =
            new Regex($"^--(?<name>{OptionName.LongNamePattern})(=(?<value>.*))?$");

        /// <summary>
        /// Indicates how the next token was joined to the option name.
        /// </summary>
        internal enum JoinType
        {
            /// <summary>
            /// A short option with extra characters appended.
            /// </summary>
            /// <remarks>
            /// The extra characters may be additional options following a short name flag, or the value for a short
            /// name option.
            /// </remarks>
            Adjoined,

            /// <summary>
            /// Describes a long option joined to a value by an '='.
            /// </summary>
            Equals,

            /// <summary>
            /// Describes an option name with a subsequent token.
            /// </summary>
            /// <remarks>
            /// The token may be the option value, an additional option, or an argument token.
            /// </remarks>
            Space,
        }

        /// <summary>
        /// The name of the referenced option.
        /// </summary>
        internal OptionName OptionName { get; }

        /// <summary>
        /// The token, if any, following the referenced option name.
        /// </summary>
        /// <remarks>
        /// This may be an adjoined token in the case of a short option name, a token joined by an '=' in the case of
        /// a long option name, or the subsequent token following a short or long name.
        /// </remarks>
        internal string? NextToken { get; }

        /// <summary>
        /// Indicates how the next token, if present, was joined to the option name.
        /// </summary>
        /// <remarks>
        /// If <see cref="NextToken"/> is <c>null</c> then this value has no meaning.
        /// </remarks>
        internal JoinType Join { get; }

        private OptionRef(OptionName optionName, string? nextToken, JoinType join = default)
        {
            OptionName = optionName ?? throw new ArgumentNullException(nameof(optionName));
            NextToken = nextToken;
            Join = join;
        }

        /// <summary>
        /// Parses an option reference from a stream of tokens.
        /// </summary>
        /// <param name="input">The token stream to parse.</param>
        /// <returns>
        /// If an <see cref="OptionRef"/> was found then it will be returned along with the remaining tokens from
        /// <paramref name="input"/>. If no <see cref="OptionRef"/> was found then <c>null</c> will be returned along
        /// with the complete <paramref name="input"/>.
        /// </returns>
        internal static (OptionRef?, IEnumerable<string>) Parse(IEnumerable<string> input)
        {
            if (input == null) throw new ArgumentNullException(nameof(input));
            var tokens = input.ToList();
            if (tokens.Count == 0) throw new ArgumentOutOfRangeException(nameof(input), "Must not be empty.");

            (var optionRef, var newInput) = ParseShortOptionRef(tokens);
            if (optionRef != null)
                return (optionRef, newInput);

            return ParseLongOptionRef(tokens);
        }

        private static (OptionRef?, IEnumerable<string>) ParseShortOptionRef(IReadOnlyList<string> input)
        {
            var match = _shortNamePattern.Match(input[0]);

            if (!match.Success)
                return (null, input);

            var name = match.Groups["name"].Value;
            var optionName = OptionName.Parse(name);
            if (optionName == null)
            {
                throw new LogicErrorException(
                    $"Invalid option name '{name}' was matched by {nameof(OptionRef)} as a short name.");
            }


            if (match.Groups["next"].Success)
            {
                var next = match.Groups["next"].Value;
                var optionRef = new OptionRef(optionName, next, JoinType.Adjoined);
                return (optionRef, input.Skip(1));
            }

            if (input.Count > 1)
            {
                var next = input[1];
                var optionRef = new OptionRef(optionName, next, JoinType.Space);
                return (optionRef, input.Skip(2));
            }

            return (new OptionRef(optionName, null), input.Skip(1));
        }

        private static (OptionRef?, IEnumerable<string>) ParseLongOptionRef(IReadOnlyList<string> input)
        {
            var match = _longNamePattern.Match(input[0]);

            if (!match.Success)
                return (null, input);

            var name = match.Groups["name"].Value;
            var optionName = OptionName.Parse(name);
            if (optionName == null)
            {
                throw new LogicErrorException(
                    $"Invalid option name '{name}' was matched by {nameof(OptionRef)} as a long name.");
            }

            if (match.Groups["value"].Success)
            {
                var next = match.Groups["value"].Value;
                var optionRef = new OptionRef(optionName, next, JoinType.Equals);
                return (optionRef, input.Skip(1));
            }

            if (input.Count > 1)
            {
                var next = input[1];
                var optionRef = new OptionRef(optionName, next, JoinType.Space);
                return (optionRef, input.Skip(2));
            }

            return (new OptionRef(optionName, null), input.Skip(1));
        }
    }
}
