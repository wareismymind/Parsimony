using Parsimony.Internal;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;

namespace Parsimony.ParserBuilder
{
    /// <summary>
    /// A builder class that allows for the construction of the logic, rules and help assocaited with 
    /// options that map directly to the parent <typeparamref name="TOption"/> properties
    /// </summary>
    /// <typeparam name="TOption"> The parent object that has a property of type <typeparamref name="TProp"/> </typeparam>
    /// <typeparam name="TProp"> The parent  </typeparam>
    public class OptionBuilder<TOption, TProp> : IOptionBuilder<TOption>
        where TOption : notnull
    {
        /// <summary>
        /// A list of rules that enforce the logic between <see cref="Option{TOptions, TValue}"/> combinations
        /// </summary>
        internal List<Rule> Rules { get; } = new List<Rule>();

        /// <summary>
        /// The short name of the option being built
        /// </summary>
        /// <exception cref="InvalidOperationException"> The value of <see cref="ShortName"/> has already been set </exception>
        internal OptionName.Short? ShortName
        {
            get => _shortName;
            set
            {
                if (_shortName != null) throw new InvalidOperationException();
                _shortName = value;
            }
        }

        /// <summary>
        /// The long name of the option being built
        /// </summary>
        /// <exception cref="InvalidOperationException"> The value of <see cref="LongName"/> has already been set </exception>
        internal OptionName.Long? LongName
        {
            get => _longName;
            set
            {
                if (_longName != null) throw new InvalidOperationException();
                _longName = value;
            }
        }

        /// <summary>
        /// The selector function used to extract or set values on the parent <typeparamref name="TOption"/>
        /// </summary>
        internal PropertySelector<TOption, TProp> Selector { get; }

        /// <summary>
        /// The parser function to be used to convert input string to the tyope of <typeparamref name="TProp"/>
        /// </summary>
        /// <remarks> 
        /// If this is not set before construction the <see cref="TypeConverter"/> for the class will be reflected off 
        /// and used. If that is incapable of conversion from string building the object will throw an exception
        /// </remarks>
        internal Func<string, TProp>? Parser { get; set; }



        private OptionName.Short? _shortName;
        private OptionName.Long? _longName;

        //TODO:CN -- HelpBuilder


        /// <summary>
        /// Constructs a new option builder with the given names
        /// </summary>
        /// <param name="shortName"> a valid short name for the option </param>
        /// <param name="longName"> a valid long name for the option </param>
        /// <param name="selector"> The property selector that indicates the property this option represents on the parent <typeparamref name="TOption"/> </param>
        /// <exception cref="ArgumentException"> <paramref name="shortName"/> or <paramref name="longName"/> are invalid </exception>
        /// <exception cref="ArgumentException"> <paramref name="selector"/> is not a valid <see cref="PropertySelector{TInput, TProp} "/></exception>
        /// <exception cref="ArgumentNullException"> <paramref name="longName"/> or <paramref name="selector"/> are null </exception>
        public OptionBuilder(char shortName, string longName, Expression<Func<TOption, TProp>> selector)
        {
            _shortName = OptionName.Parse(shortName.ToString()) as OptionName.Short;
            _longName = OptionName.Parse(longName) as OptionName.Long;
            Selector = new PropertySelector<TOption, TProp>(selector);
        }

        /// <summary>
        /// Constructs a new option builder with the given short name
        /// </summary>
        /// <param name="selector"> The property selector that indicates the property this option represents on the parent <typeparamref name="TOption"/> </param>
        /// <param name="shortName"> a valid short name for the option </param>
        /// <exception cref="ArgumentException"> <paramref name="shortName"/> is invalid </exception>
        /// <exception cref="ArgumentException"> <paramref name="selector"/> is not a valid <see cref="PropertySelector{TInput, TProp} "/></exception>
        /// <exception cref="ArgumentNullException"> <paramref name="selector"/> is null </exception>
        public OptionBuilder(char shortName, Expression<Func<TOption, TProp>> selector)
        {
            _shortName = OptionName.Parse(shortName.ToString()) as OptionName.Short;
            Selector = new PropertySelector<TOption, TProp>(selector);
        }


        /// <summary>
        /// Constructs a new option builder with the given short name
        /// </summary>
        /// <param name="longName"> a valid long name for the option </param>
        /// <param name="selector"> The property selector that indicates the property this option represents on the parent <typeparamref name="TOption"/> </param>
        /// <exception cref="ArgumentException"> <paramref name="longName"/> is invalid </exception>
        /// <exception cref="ArgumentException"> <paramref name="selector"/> is not a valid <see cref="PropertySelector{TInput, TProp} "/></exception>
        /// <exception cref="ArgumentNullException"> <paramref name="longName"/> or <paramref name="selector"/> is null </exception>
        public OptionBuilder(string longName, Expression<Func<TOption, TProp>> selector)
        {
            _longName = OptionName.Parse(longName) as OptionName.Long;
            Selector = new PropertySelector<TOption, TProp>(selector);
        }


        /// <summary>
        /// Compiles the rule sets and parser into an option parser of type <see cref="OptionParser{TOptions}"/>
        /// </summary>
        /// <exception cref="InvalidOperationException"> Incompatible rules have been found </exception>
        /// <exception cref="InvalidOperationException"> Duplicate rules are found </exception>
        /// /// <exception cref="InvalidOperationException">
        /// No parser is set and no <see cref="TypeConverter"/> can be found that can perform conversions from string 
        /// </exception>
        /// <returns> A result containing the parser and the ruleset assocaited with this option </returns>
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

            var opt = new Option<TOption, TProp>(_shortName, _longName, Parser, Selector.Setter);

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
    }
}
