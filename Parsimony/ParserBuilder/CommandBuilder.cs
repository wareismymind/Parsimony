using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Parsimony.ParserBuilder
{
    internal interface IParserBuilder { }


    /// <summary>
    /// A builder for commands that allows for type safety and parser logic to be encapsulated
    /// </summary>
    /// <typeparam name="TOption"> The type of options required to execute the command </typeparam>
    public class CommandBuilder<TOption> : IParserBuilder
        where TOption : notnull
    {
        /// <summary>
        /// The name of the command
        /// </summary>
        internal string Name { get; }

        /// <summary>
        /// The help text associated with the command that will be printed if parsing fails
        /// </summary>
        internal string HelpText { get; } = "";

        private readonly IList<IOptionBuilder<TOption>> _builders = new List<IOptionBuilder<TOption>>();

        /// <summary>
        /// Constructs a new command builder for a command with the given name
        /// </summary>
        //CN: TODO -- not done this yet
        public CommandBuilder(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("parameter cannot be null or whitespace", nameof(name));

            Name = name;
        }

        /// <summary>
        /// Adds a new option to the <see cref="CommandBuilder{TOption}"/> and returns an <see cref="OptionBuilder{TOption, TProp}"/>
        /// to configure that option with
        /// </summary>
        /// <typeparam name="TProp"></typeparam>
        /// <param name="longName"> The long name of the option to be created </param>
        /// <param name="selector"> 
        /// a property selector that points to the property that will be set when a value is found in the parsing stream matcing 
        /// this option
        /// </param>
        /// <exception cref="ArgumentException"> <paramref name="longName"/> is invalid </exception>
        /// <exception cref="ArgumentException"> <paramref name="selector"/> is not a valid <see cref="PropertySelector{TInput, TProp} "/></exception>
        /// <exception cref="ArgumentNullException"> <paramref name="longName"/> or <paramref name="selector"/> is null </exception>
        /// <returns></returns>
        public OptionBuilder<TOption, TProp> AddOption<TProp>(string longName, Expression<Func<TOption, TProp>> selector)
        {
            var builder = new OptionBuilder<TOption, TProp>(longName, selector);
            _builders.Add(builder);
            return builder;
        }


        /// <summary>
        /// Adds a new option to the <see cref="CommandBuilder{TOption}"/> and returns an <see cref="OptionBuilder{TOption, TProp}"/>
        /// to configure that option with
        /// </summary>
        /// <typeparam name="TProp"></typeparam>
        /// <param name="shortName"> The short name of the option to be created </param>
        /// <param name="selector"> 
        /// a property selector that points to the property that will be set when a value is found in the parsing stream matcing 
        /// this option
        /// </param>
        /// <exception cref="ArgumentException"> <paramref name="shortName"/> is invalid </exception>
        /// <exception cref="ArgumentException"> <paramref name="selector"/> is not a valid <see cref="PropertySelector{TInput, TProp} "/></exception>
        /// <exception cref="ArgumentNullException"> <paramref name="selector"/> is null </exception>
        /// <returns> a new <see cref="OptionBuilder{TOption, TProp}"/> with the given name set </returns>
        public OptionBuilder<TOption, TProp> AddOption<TProp>(char shortName, Expression<Func<TOption, TProp>> selector)
        {
            var builder = new OptionBuilder<TOption, TProp>(shortName, selector);
            _builders.Add(builder);
            return builder;
        }

    }

}
