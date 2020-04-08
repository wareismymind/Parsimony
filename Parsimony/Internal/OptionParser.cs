using System;
using System.Collections.Generic;
using System.Linq;

namespace Parsimony.Internal
{
    internal class OptionParser<TOptions> where TOptions : notnull
    {
        private readonly IReadOnlyList<IOption<TOptions>> _options;

        public OptionParser(IEnumerable<IOption<TOptions>> options)
        {
            _options = options?.ToList() ?? throw new ArgumentNullException(nameof(options));
        }

        public (IEnumerable<Action<TOptions>>, IEnumerable<string>) Parse(IEnumerable<string> input)
        {
            var tokens = input?.ToList() ?? throw new ArgumentNullException(nameof(input));

            var assignments = new List<Action<TOptions>>();
            var arguments = new List<string>();

            while (tokens.Count > 0)
            {
                // TODO: Encapsulate context and refactor

                (var optionRef, var newInput) = OptionRef.Parse(tokens);

                if (optionRef == null)
                {
                    (tokens, arguments) = ConsumeArguments(tokens, arguments);
                    continue;
                }

                var optionName = optionRef.OptionName;
                var option = optionName switch
                {
                    OptionName.Short s => _options.FirstOrDefault(o => o.ShortName == s),
                    OptionName.Long l => _options.FirstOrDefault(o => o.LongName == l),
                    OptionName n => throw new LogicErrorException($"unknown {nameof(OptionName)} subtype '{n.GetType().Name}'"),
                };


                // TODO: Treat these as arguments? Probably just have to put them after "--"
                // TODO: Better error communication.
                if (option == null)
                    throw new Exception($"Unknown option '{optionName}'");


                // TODO: Express this better :/

                // An explicit value can only be assigned to an option with long-name/equals, default is "true"
                var value = option.IsFlag && optionRef.Join != OptionRef.JoinType.Equals
                    ? "true"
                    : optionRef.NextToken;

                // TODO: Better error communication.
                if (value == null)
                    throw new Exception($"Missing required argument for option '{optionName}'");

                assignments.Add(option.Parse(value));

                tokens = newInput.ToList();

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
            }

            return (assignments, arguments);
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
    }
}
