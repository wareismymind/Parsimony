using Parsimony.Errors;
using Parsimony.Exceptions;
using Parsimony.Options;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Parsimony.Internal
{
    /// <summary>
    /// A parser for a set of <see cref="IOption{TOptionSet}"/>.
    /// </summary>
    /// <typeparam name="TOptionSet">The type of the option set.</typeparam>
    internal static class OptionParser<TOptionSet> where TOptionSet : class
    {
        /// <summary>
        /// Parses an input stream for the options in <paramref name="options"/>.
        /// </summary>
        /// <param name="options">The options to parse for.</param>
        /// <param name="config">Configuration settings for the parser.</param>
        /// <param name="input">The input to parse.</param>
        /// <returns>A <see cref="OptionParserResult{TOptionSet}"/>.</returns>
        internal static OptionParserResult<TOptionSet> Parse(
            IEnumerable<IOption<TOptionSet>> options, ParserConfig config, IEnumerable<string> input)
        {
            if (options == null) throw new ArgumentNullException(nameof(options));
            if (config == null) throw new ArgumentNullException(nameof(config));
            var tokens = input?.ToList() ?? throw new ArgumentNullException(nameof(input));

            var assignments = new Dictionary<IOption<TOptionSet>, Action<TOptionSet>>();
            var arguments = new List<string>();

            while (tokens.Count > 0)
            {
                (var optionRef, var newInput) = OptionRef.Parse(tokens);

                if (optionRef == null)
                {
                    (tokens, arguments) = ConsumeArguments(tokens, arguments, config);
                    continue;
                }

                tokens = newInput.ToList();

                var optionName = optionRef.OptionName;
                var option = optionName switch
                {
                    OptionName.Short s => options.FirstOrDefault(o => o.ShortName == s),
                    OptionName.Long l => options.FirstOrDefault(o => o.LongName == l),
                    OptionName n => throw new LogicErrorException(
                        $"Unknown {nameof(OptionName)} subtype '{n.GetType().Name}'."),
                };

                if (option == null)
                    return UnknownOptionError(optionName);

                string? value; // can't mix declarations with expressions
                (value, tokens) = GetOptionValue(option, optionRef, tokens);
                if (value == null)
                    return MissingOptionValueError(optionName);

                var (assignment, error) = option.GetAssignment(value);

                if (error != null)
                    return Error(error);

                if (assignment == null)
                {
                    throw new LogicErrorException(
                        $"{nameof(IOption<TOptionSet>.GetAssignment)} returned null assignment with no error.");
                }

                assignments.Add(option, assignment);
            }

            return new OptionParserResult<TOptionSet>(assignments, arguments);
        }

        private static (List<string>, List<string>) ConsumeArguments(
            IReadOnlyList<string> tokens, IReadOnlyList<string> arguments, ParserConfig options)
        {
            var newArguments = arguments.ToList();
            var newTokens = new List<string>();

            if (options.PosixOptionOrder)
            {
                var remainingTokens = tokens.ToList();
                remainingTokens.Remove("--");
                newArguments.AddRange(remainingTokens);
            }
            else if (tokens[0] == "--")
            {
                newArguments.AddRange(tokens.Skip(1));
            }
            else
            {
                newArguments.Add(tokens[0]);
                newTokens = tokens.Skip(1).ToList();
            }

            return (newTokens, newArguments);
        }

        private static OptionParserResult<TOptionSet> UnknownOptionError(OptionName optionName) =>
            Error(new UnknownOptionError(optionName));

        private static (string?, List<string>) GetOptionValue(
            IOption<TOptionSet> option, OptionRef optionRef, IList<string> tokens)
        {
            // An explicit value can only be assigned to an option with long-name/equals, default is "true"
            var value = option.IsFlag && optionRef.Join != OptionRef.JoinType.Equals
                ? "true"
                : optionRef.NextToken;

            if (value == null)
                return (null, tokens.ToList());

            // Flags can only have values joined by '='. If a flag has a next token that wasn't joined by '=' then
            // the token needs to go back into the input.
            if (optionRef.NextToken != null && option.IsFlag && optionRef.Join != OptionRef.JoinType.Equals)
            {
                // If the token was adjoined then the value is a sequence of short options and needs a leading '-'.
                var token = optionRef.Join == OptionRef.JoinType.Adjoined
                    ? $"-{optionRef.NextToken}"
                    : optionRef.NextToken;

                tokens.Insert(0, token);
            }

            return (value, tokens.ToList());
        }

        private static OptionParserResult<TOptionSet> MissingOptionValueError(OptionName optionName) =>
            Error(new MissingOptionValueError(optionName));

        private static OptionParserResult<TOptionSet> Error(OptionParsingError error) =>
            new OptionParserResult<TOptionSet>(error);
    }
}
