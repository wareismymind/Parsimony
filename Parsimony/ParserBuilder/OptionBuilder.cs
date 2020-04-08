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
        public char? ShortName
        {
            get => Name?.ShortName;
            set
            {
                if (Name?.ShortName != null) throw new InvalidOperationException();
                SetName(value, LongName);
            }
        }
        public string? LongName
        {
            get => Name?.LongName;
            set
            {
                if (Name?.LongName != null) throw new InvalidOperationException();
                SetName(ShortName, value);
            }
        }
        
        internal PropertySelector<TOption, TProp> Selector { get; }
        internal OptionName Name { get; set; }
        
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

            var opt = new Option<TOption, TProp>(Name, Parser, Selector.Setter);

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
            Name = new OptionName(shortName, longName);
            Selector = new PropertySelector<TOption, TProp>(selector);
        }

        public OptionBuilder(char shortName, Expression<Func<TOption, TProp>> selector)
        {
            Name = new OptionName(shortName);
            Selector = new PropertySelector<TOption, TProp>(selector);
        }

        public OptionBuilder(string longName, Expression<Func<TOption, TProp>> selector)
        {
            Name = new OptionName(longName);
            Selector = new PropertySelector<TOption, TProp>(selector);
        }

        private void SetName(char? shortName, string? longName)
        {
            Name  = (shortName, longName) switch
            {
                (null, string l) => new OptionName(l),
                (char s, null) => new OptionName(s),
                (char s, string l) => new OptionName(s, l),
                _ => throw new ArgumentNullException("Both names cannot be null")
            };
        }
        
    }
}
