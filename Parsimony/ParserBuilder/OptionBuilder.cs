using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Parsimony.ParserBuilder
{
    public class OptionBuilder<TCommand, TProp> : IOptionBuilder<TCommand, TProp>
    {
        public List<Rule> Rules { get; } = new List<Rule>();

        internal PropertySelector<TCommand,TProp> Selector { get; }
        internal OptionName? Name { get; set; }
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

        //TODO:CN -- HelpBuilder

        public IOptionParser Build() => throw new NotImplementedException();

        public OptionBuilder(char shortName, string longName, Expression<Func<TCommand, TProp>> selector)
        {
            Name = new OptionName(shortName, longName);
            Selector = new PropertySelector<TCommand, TProp>(selector);
        }

        public OptionBuilder(char shortName, Expression<Func<TCommand, TProp>> selector)
        {
            Name = new OptionName(shortName);
            Selector = new PropertySelector<TCommand, TProp>(selector);
        }

        public OptionBuilder(string longName, Expression<Func<TCommand, TProp>> selector)
        {
            Name = new OptionName(longName);
            Selector = new PropertySelector<TCommand, TProp>(selector);
        }

        public OptionBuilder(Expression<Func<TCommand, TProp>> selector)
        {
            Selector = new PropertySelector<TCommand, TProp>(selector);
        }

        private void SetName(char? shortName, string? longName)
        {
            var res = (shortName, longName) switch
            {
                (null, string l) => new OptionName(l),
                (char s, null) => new OptionName(s),
                (char s, string l) => new OptionName(s, l),
                _ => throw new ArgumentNullException("Both names cannot be null")
            };

            Name = res;
        }
    }
}
