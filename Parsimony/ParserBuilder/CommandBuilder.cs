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

        private readonly IList<IOptionBuilder> _builders = new List<IOptionBuilder>();

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
        List<Rule> Rules { get; }

        char? ShortName { get; set; }
        string? LongName { get; set; }

        IOptionParser Build();
    }

    public interface IOptionBuilder<TCommand, TProp> : IOptionBuilder
    {

    }

    public interface IOptionParser { }


    public class OptionParserBuildResult
    {
        IOptionParser Parser { get; set; }
        IEnumerable<Rule> Rules { get; set; } = new List<Rule>();
    }


    //TODO:CN -- Can probably just make into a bunch of separate types and pattern match 
    // on them for the impls
    public class Rule
    {
        public string PropertyName { get; }
        public string? Target { get; }
        public RuleKind Kind { get; }

        public Rule(RuleKind kind, string propertyName, string? target)
        {
            //CN: Check for stuff
            Kind = kind;
            PropertyName = propertyName;
            Target = target;
        }
    }

    public enum RuleKind
    {
        Precludes,
        Requires,
        Implies
    }
   
}
