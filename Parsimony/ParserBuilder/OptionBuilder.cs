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
            Name = new OptionName(longName);
            _selector = GetMemberExpression(selector);
        }
        
        private MemberExpression GetMemberExpression(Expression<Func<TCommand,TProp>> selector)
        {
            if (!(selector is MemberExpression member))
                throw new ArgumentException("Must be a member selector expression");

            return member;
        }
    }
}
