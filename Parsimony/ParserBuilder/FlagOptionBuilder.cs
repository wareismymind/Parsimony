using System;
using System.Linq.Expressions;

namespace Parsimony.ParserBuilder
{
    public class FlagOptionBuilder<TCommand> : OptionBuilder<TCommand, bool>
        where TCommand : notnull
    {
        public FlagOptionBuilder(char shortName, Expression<Func<TCommand, bool>> selector) 
            : base(shortName, selector)
        { }

        public FlagOptionBuilder(string longName, Expression<Func<TCommand, bool>> selector) 
            : base(longName, selector)
        { }
    }
}
