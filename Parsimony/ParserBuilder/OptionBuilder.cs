using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Parsimony.ParserBuilder
{
    public class OptionBuilder<TCommand, TProp> : IOptionBuilder<TCommand, TProp>
    {
        public string MemberName { get; }
        public List<string> Precludes { get; } = new List<string>();
        public List<string> Requires { get; } = new List<string>();

        internal OptionName Name { get; set; }
        
        //TODO:CN -- HelpBuilder
        private MemberExpression _selector;

        public IOptionParser Build() => throw new NotImplementedException();

        public OptionBuilder(char shortName, string longName, Expression<Func<TCommand, TProp>> selector ) 
        {
            Name = new OptionName(shortName, longName);
            _selector = GetMemberExpression(selector);
        }
        
        public OptionBuilder(char shortName, Expression<Func<TCommand, TProp>> selector)
        {
            Name = new OptionName(shortName);
            _selector = GetMemberExpression(selector);
        }

        public OptionBuilder(string longName, Expression<Func<TCommand, TProp>> selector)
        {
            Name = new OptionName(shortName);
            _selector = GetMemberExpression(selector);
        }

        private OptionBuilder(char? shortName, string? longName, Expression<Func<TCommand, TProp>> selector)
        {
            if (shortName == null)
            {
                
            }

            Name = new OptionName(shortName, longName);

            if (shortName == null && longName == null)
                throw new ArgumentException($"Both {nameof(shortName)} and {nameof(longName)} cannot be null");

            if (shortName != null && !char.IsLetter(shortName.Value))
                throw new ArgumentException("Must be a letter", nameof(shortName));

            if (string.IsNullOrWhiteSpace(longName))
                throw new ArgumentException("Cannot be null or whitespace");



            ShortName = shortName;
            LongName = longName;

            _selector = GetMemberExpression(selector);

            //CN -- Stuff to do with the selector
        }

        private MemberExpression GetMemberExpression(Expression<Func<TCommand,TProp>> selector)
        {
            if (!(selector is MemberExpression member))
                throw new ArgumentException("Must be a member selector expression");

            return member;
        }
    }
}
