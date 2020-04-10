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
        /// Indicates how the next-token was joined to the option name.
        /// </summary>
        public enum JoinType
        {
            Adjoined, // Describes a short option with extra characters appended
            Equals, // Describes a long option joined to a value by an '='
            Space, // Describes an option name with a subsequent token
        }

        /// <summary>
        /// The name of the referenced option.
        /// </summary>
        public OptionName OptionName { get; }

        /// <summary>
        /// The token, if any, following the referenced option name.
        /// </summary>
        /// <remarks>
        /// This may be an adjoined token in the case of a short option name, a token joined by an '=' in the case of
        /// a long option name, or the subsequent token in either case.
        /// </remarks>
        public string? NextToken { get; }

        /// <summary>
        /// Indicates how the next-token was joined to the option name.
        /// </summary>
        public JoinType Join { get; }

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
        public static (OptionRef?, IEnumerable<string>) Parse(IEnumerable<string> input)
        {
            if (input == null) throw new ArgumentNullException(nameof(input));
            var tokens = input.ToList();
            if (tokens.Count == 0) throw new ArgumentOutOfRangeException(nameof(input), "Must not be empty");

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

            var optionName = OptionName.Parse(match.Groups["name"].Value);
            if (optionName == null)
                // TODO: Create LogicErrorException
                throw new Exception("LOGIC ERROR: option name in option ref regex is not a valid option name");


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

            var optionName = OptionName.Parse(match.Groups["name"].Value);
            if (optionName == null)
                // TODO: Create LogicErrorException
                throw new Exception("LOGIC ERROR: option name in option ref regex is not a valid option name");

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
