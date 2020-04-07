using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Parsimony.ParserBuilder
{
    public interface IParserBuilder { }

    public class CommandBuilder<TCommand> : IParserBuilder
    {
        public string Name { get; }
        public string HelpText { get; } = "";

        private IList<IOptionBuilder> _builders = new List<IOptionBuilder>();

        public CommandBuilder(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("parameter cannot be null or whitespace", nameof(name));

            Name = name;
        }

        public IOptionBuilder<TCommand, bool> AddFlag<TProp>(string longName, Expression<Func<TCommand, bool>> selector)
        {
            var builder = new FlagOptionBuilder<TCommand>(longName, selector);
            _builders.Add(builder);
            return builder;
        }

        public IOptionBuilder<TCommand, bool> AddFlag(char shortName, Expression<Func<TCommand, bool>> selector)
        {
            var builder = new FlagOptionBuilder<TCommand>(shortName, selector);
            _builders.Add(builder);
            return builder;
        }
    }

    public interface IOptionBuilder
    {
        List<string> Precludes { get; }
        List<string> Requires { get; } 
        
        char? ShortName { get; set; }
        string? LongName { get; set; }

        IOptionParser Build();
    }

    public interface IOptionBuilder<TCommand, TProp> : IOptionBuilder
    {

    }

    public interface IOptionParser { }
}
