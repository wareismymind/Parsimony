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
        char? ShortName { get; set; }
        string? LongName { get; set; }
        List<string> Precludes { get; }
        List<string> Requires { get; } 
        
        IOptionParser Build();
    }

    public class OptionBuilder<TCommand, TProp> : IOptionBuilder<TCommand, TProp>
    {
        public string MemberName { get; }

        public string? LongName { get; set; }
        public char? ShortName { get; set; }
        public List<string> Precludes { get; } = new List<string>();
        public List<string> Requires { get; } = new List<string>();

        //TODO:CN -- HelpBuilder

        public IOptionParser Build() => throw new NotImplementedException();


        public OptionBuilder(char shortName, Expression<Func<TCommand, TProp>> selector)
        {
            if (!char.IsLetter(shortName))
                throw new ArgumentException("Must be a character", nameof(shortName));

            var member = AssertIsMemberExpression(selector);

            MemberName = member.Member.Name;
            ShortName = shortName;
        }

        public OptionBuilder(string longName, Expression<Func<TCommand, TProp>> selector)
        {
            if (string.IsNullOrWhiteSpace(longName))
                throw new ArgumentException("Cannot be null or whitespace", nameof(longName));

            var member = AssertIsMemberExpression(selector);

            MemberName = member.Member.Name;
            LongName = longName;
        }

        private MemberExpression AssertIsMemberExpression(Expression<Func<TCommand,TProp>> selector)
        {
            if (!(selector is MemberExpression member))
                throw new ArgumentException("Must be a member selector expression");

            return member;
        }
    }




    public interface IOptionBuilder<TCommand, TProp> : IOptionBuilder
    {

    }

    public class FlagOptionBuilder<TCommand> : OptionBuilder<TCommand, bool>
    {
        public FlagOptionBuilder(char shortName, Expression<Func<TCommand, bool>> selector) 
            : base(shortName, selector)
        { }

        public FlagOptionBuilder(string longName, Expression<Func<TCommand, bool>> selector) 
            : base(longName, selector)
        { }
    }

    public class Rule<TCommand, TSource, TTarget>
    {
        private string _sourceName;
        private string _targetName;

        public Rule(
            Expression<Func<TCommand, TSource>> sourceSelector,
            Expression<Func<TCommand, TTarget>> targetSelector,
            RuleKind kind)
        {

        }
    }

    public enum RuleKind
    {
        Preclude,
        Require
    }

    public interface IOptionParser { }
}
