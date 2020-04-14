using Parsimony.Internal;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Parsimony
{
    /// <summary>
    /// A parser for an set of options.
    /// </summary>
    /// <typeparam name="TOptionSet">The type of the option set.</typeparam>
    public class OptionParser<TOptionSet> where TOptionSet : class
    {
        private readonly IReadOnlyList<IOption<TOptionSet>> _options;

        private readonly Func<TOptionSet> _optionSetFactory;

        /// <summary>
        /// Creates a new <see cref="OptionParser{TOptionSet}"/>.
        /// </summary>
        /// <param name="options">The options to parse.</param>
        /// <param name="optionSetFactory">
        /// A factory to get the base instance of <typeparamref name="TOptionSet"/> from.
        /// </param>
        internal OptionParser(IEnumerable<IOption<TOptionSet>> options, Func<TOptionSet> optionSetFactory)
        {
            // TODO: Accept rules (requires/precludes)?
            _options = options?.ToList() ?? throw new ArgumentNullException(nameof(options));

            _optionSetFactory = optionSetFactory ?? throw new ArgumentNullException(nameof(optionSetFactory));
        }

        /// <summary>
        /// Parses an input stream for the options in the option set.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <returns>A <see cref="OptionParseResult{TOptionSet}"/>.</returns>
        public OptionParseResult<TOptionSet> Parse(IEnumerable<string> input)
        {
            var tokens = input?.ToList() ?? throw new ArgumentNullException(nameof(input));

            var assignments = new List<Action<TOptionSet>>();
            var arguments = new List<string>();

            while (tokens.Count > 0)
            {
                (var optionRef, var newInput) = OptionRef.Parse(tokens);

                if (optionRef == null)
                {
                    (tokens, arguments) = ConsumeArguments(tokens, arguments);
                    continue;
                }

                tokens = newInput.ToList();

                var optionName = optionRef.OptionName;
                var option = optionName switch
                {
                    OptionName.Short s => _options.FirstOrDefault(o => o.ShortName == s),
                    OptionName.Long l => _options.FirstOrDefault(o => o.LongName == l),
                    OptionName n => throw new LogicErrorException($"unknown {nameof(OptionName)} subtype '{n.GetType().Name}'"),
                };


                if (option == null)
                    return UnknownOptionError(optionName);

                string? value; // can't mix declarations with expressions :/
                (value, tokens) = GetOptionValue(option, optionRef, tokens);
                if (value == null)
                    return MissingOptionValueError(optionName);

                assignments.Add(option.Parse(value));
            }

            var optionSet = _optionSetFactory.Invoke();
            foreach (var assignment in assignments)
            {
                assignment.Invoke(optionSet);
            }
            return new OptionParseResult<TOptionSet>(optionSet, arguments);
        }

        private (List<string>, List<string>) ConsumeArguments(
            IReadOnlyList<string> tokens, IReadOnlyList<string> arguments)
        {
            var newArguments = arguments.ToList();
            var newTokens = new List<string>();

            // TODO: Support ignoring this?
            if (tokens[0] == "--")
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

        private OptionParseResult<TOptionSet> UnknownOptionError(OptionName optionName) =>
            new OptionParseResult<TOptionSet>(new UnknownOptionError(optionName));

        private (string?, List<string>) GetOptionValue(
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

        private OptionParseResult<TOptionSet> MissingOptionValueError(OptionName optionName) =>
            new OptionParseResult<TOptionSet>(new MissingOptionValueError(optionName));
    }
}
