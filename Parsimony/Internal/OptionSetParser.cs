using Parsimony.Errors;
using Parsimony.Exceptions;
using Parsimony.Options;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Parsimony.Internal
{
    /// <summary>
    /// The <see cref="IOptionSetParser{TOptionSet}"/> implementation used by <see cref="OptionSet{TOptionSet}"/>.
    /// </summary>
    /// <typeparam name="TOptionSet">The type of the option set.</typeparam>
    internal class OptionSetParser<TOptionSet> : IOptionSetParser<TOptionSet> where TOptionSet : class
    {
        private readonly Func<TOptionSet> _defaultOptionsFactory;
        private readonly IReadOnlyDictionary<string, IOption<TOptionSet>> _options;
        private readonly RuleSet<string> _rules;

        internal OptionSetParser(
            Func<TOptionSet> defaultOptionsFactory,
            IReadOnlyDictionary<string, IOption<TOptionSet>> options, 
            RuleSet<string> rules)
        {
            _defaultOptionsFactory = defaultOptionsFactory ?? throw new ArgumentNullException(nameof(defaultOptionsFactory));
            _options = options ?? throw new ArgumentNullException(nameof(options));
            _rules = rules ?? throw new ArgumentNullException(nameof(rules));
        }

        public ParseResult<TOptionSet> Parse(IEnumerable<string> input, ParserConfig config)
        {
            if (input == null) throw new ArgumentNullException(nameof(input));
            if (config == null) throw new ArgumentNullException(nameof(config));

            var parseOptionsResult = OptionParser<TOptionSet>.Parse(_options.Values, config, input);

            if (parseOptionsResult.Error != null)
                return new ParseResult<TOptionSet>(parseOptionsResult.Error);

            var assignments = parseOptionsResult.Assignments;

            foreach (var req in _rules.Requirements)
            {
#pragma warning disable CS8602 // Dereference of a possibly null reference.
                if (assignments.ContainsKey(_options[req.Dependent]))
#pragma warning restore CS8602 // Dereference of a possibly null reference.
                {
                    if (!assignments.ContainsKey(_options[req.Dependee]))
                    {
                        var missingOption = _options[req.Dependee].LongName as OptionName ?? _options[req.Dependee].ShortName;
                        var requiredBy = _options[req.Dependent].LongName as OptionName ?? _options[req.Dependent].ShortName;

#pragma warning disable CS8604 // Possible null reference argument.
                        return new ParseResult<TOptionSet>(new MissingRequiredOptionError(missingOption, requiredBy));
#pragma warning restore CS8604 // Possible null reference argument.
                    }
                }
            }

            foreach (var (precluder, precludee) in _rules.Preclusions)
            {
#pragma warning disable CS8602 // Dereference of a possibly null reference.
                if (assignments.ContainsKey(_options[precluder]))
#pragma warning restore CS8602 // Dereference of a possibly null reference.
                {
                    if (assignments.ContainsKey(_options[precludee]))
                    {
                        var precludedOption = _options[precludee].LongName as OptionName ?? _options[precludee].ShortName;
                        var precludedBy = _options[precluder].LongName as OptionName ?? _options[precluder].ShortName;

#pragma warning disable CS8604 // Possible null reference argument.
                        return new ParseResult<TOptionSet>(new PrecludedOptionError(precludedOption, precludedBy));
#pragma warning restore CS8604 // Possible null reference argument.
                    }
                }
            }

            var optionValues = _defaultOptionsFactory();

            // If error was null then Assignments and Arguments are not.
#pragma warning disable CS8602 // Dereference of a possibly null reference.
#pragma warning disable CS8604 // Possible null reference argument.

            foreach (var assignment in assignments)
            {
                assignment.Value(optionValues);
            }

            return new ParseResult<TOptionSet>(optionValues, parseOptionsResult.Arguments);

#pragma warning restore CS8604
#pragma warning restore CS8602
        }
    }
}
