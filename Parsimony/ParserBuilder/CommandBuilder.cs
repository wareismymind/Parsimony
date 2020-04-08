using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Parsimony.ParserBuilder
{
    public interface IParserBuilder { }

    public class CommandBuilder<TOption> : IParserBuilder
        where TOption : notnull
    {
        public string Name { get; }
        public string HelpText { get; } = "";

        private readonly IList<IOptionBuilder<TOption>> _builders = new List<IOptionBuilder<TOption>>();

        public CommandBuilder(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("parameter cannot be null or whitespace", nameof(name));

            Name = name;
        }

        public OptionBuilder<TOption, bool> AddFlag(string longName, Expression<Func<TOption, bool>> selector)
        {
            var builder = new FlagOptionBuilder<TOption>(longName, selector);
            _builders.Add(builder);
            return builder;
        }

        public OptionBuilder<TOption, bool> AddFlag(char shortName, Expression<Func<TOption, bool>> selector)
        {
            var builder = new FlagOptionBuilder<TOption>(shortName, selector);
            _builders.Add(builder);
            return builder;
        }
    }
   
}
