using Parsimony.Internal;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;

namespace Parsimony.ParserBuilder
{
    public class OptionBuilder<TOption, TProp> : IOptionBuilder<TOption>
        where TOption : notnull
    {
        public List<Rule> Rules { get; } = new List<Rule>();
        internal OptionName.Short? ShortName
        {
            get => _shortName;
            set
            {
                if (_shortName != null) throw new InvalidOperationException();
                _shortName = value;
            }
        }
        internal OptionName.Long? LongName
        {
            get => _longName;
            set
            {
                if (_longName != null) throw new InvalidOperationException();
                _longName = value;
            }
        }
        
        internal PropertySelector<TOption, TProp> Selector { get; }
        
        private OptionName.Short? _shortName;
        private OptionName.Long? _longName;
        
        internal Func<string,TProp>? Parser { get; set; }


        //TODO:CN -- HelpBuilder
        
        
        internal OptionParserBuildResult<TOption> Build()
        {
            var requiredSet = EnsureNoDuplicateTargets(Rules.Where(x => x.Kind == RuleKind.Requires));
            var precludesSet = EnsureNoDuplicateTargets(Rules.Where(x => x.Kind == RuleKind.Precludes));

            if (precludesSet.Any(x => requiredSet.Contains(x)))
                throw new InvalidOperationException("Cannot both require and preclude a property");

            if (Parser == null)
            {
                var converter = TypeDescriptor.GetConverter(typeof(TProp));
                if (!converter.CanConvertFrom(typeof(string)))
                {
                    throw new InvalidOperationException(
                        $"The given type {typeof(TOption).Name} cannot be converted from string," +
                        $" specify a parser with the extension method 'WithParser'");
                }

                Parser = (string input) => (TProp)converter.ConvertFromString(input);
            }

            var opt = new Option<TOption, TProp>(_shortName, _longName, Parser, Selector.Setter);

            return new OptionParserBuildResult<TOption>(opt, Rules);
        }

        private HashSet<string> EnsureNoDuplicateTargets(IEnumerable<Rule> rules)
        {
            var targets = rules.Select(x => x.Target).ToList();
            var targetSet = new HashSet<string>(targets);

            if (targetSet.Count != targets.Count)
            {
                targets.Except(targetSet);
                throw new InvalidOperationException("Cannot set requires for a property twice");
            }

            return targetSet;
        }

        public OptionBuilder(char shortName, string longName, Expression<Func<TOption, TProp>> selector)
        {
            _shortName = OptionName.Parse(shortName.ToString()) as OptionName.Short;
            _longName = OptionName.Parse(longName) as OptionName.Long;
            Selector = new PropertySelector<TOption, TProp>(selector);
        }

        public OptionBuilder(char shortName, Expression<Func<TOption, TProp>> selector)
        {
            _shortName = OptionName.Parse(shortName.ToString()) as OptionName.Short;
            Selector = new PropertySelector<TOption, TProp>(selector);
        }

        public OptionBuilder(string longName, Expression<Func<TOption, TProp>> selector)
        {
            _longName = OptionName.Parse(longName) as OptionName.Long;
            Selector = new PropertySelector<TOption, TProp>(selector);
        }
        
    }
}
