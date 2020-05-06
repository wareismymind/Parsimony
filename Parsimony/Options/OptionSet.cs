using Parsimony.Internal;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;

namespace Parsimony.Options
{
    /// <summary>
    /// A set of CLI options.
    /// </summary>
    /// <typeparam name="TOptionSet">The type whose fields contain the option values.</typeparam>
    public class OptionSet<TOptionSet> where TOptionSet : class
    {
        private readonly Func<TOptionSet> _defaultOptionsFactory;

        private readonly Dictionary<string, IOption<TOptionSet>> _options =
            new Dictionary<string, IOption<TOptionSet>>();

        private readonly DependencyGraph<string> _requirements = new DependencyGraph<string>();

        private readonly HashSet<(string, string)> _preclusions = new HashSet<(string, string)>();

        /// <summary>
        /// Creates a new <see cref="OptionSet{T}"/>.
        /// </summary>
        /// <param name="defaultOptionsFactory">
        /// A function to produce the default <typeparamref name="TOptionSet"/>.
        /// </param>
        /// <exception cref="InvalidOperationException">
        /// <paramref name="defaultOptionsFactory"/> is <c>null</c> and <typeparamref name="TOptionSet"/> has no
        /// default constructor.
        /// </exception>
        public OptionSet(Func<TOptionSet>? defaultOptionsFactory = null)
        {
            if (defaultOptionsFactory != null)
            {
                _defaultOptionsFactory = defaultOptionsFactory;
                return;
            }

            var defaultCtor = typeof(TOptionSet).GetConstructor(Array.Empty<Type>());
            if (defaultCtor == null)
            {
                throw new InvalidOperationException($"{typeof(TOptionSet).Name} has no default constructor.");
            }
#pragma warning disable CS8603 // Possible null reference return.
            _defaultOptionsFactory = () => defaultCtor.Invoke(Array.Empty<object>()) as TOptionSet;
#pragma warning restore CS8603 // Possible null reference return.
        }

        /// <summary>
        /// Adds a new option to the set.
        /// </summary>
        /// <typeparam name="TOptionValue">The type of the option value.</typeparam>
        /// <param name="shortName">The option's short name.</param>
        /// <param name="property">The property of <typeparamref name="TOptionSet"/> that will hold the option's value.</param>
        /// <param name="helpText">The help-text description of the option.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="property"/> or <paramref name="helpText"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <para><paramref name="shortName"/> is an invalid option name.</para>
        /// <para><paramref name="property"/> is not a writable property of <typeparamref name="TOptionSet"/>.</para>
        /// <para><paramref name="helpText"/> is empty or whitespace.</para>
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// <para><paramref name="shortName"/> already refers to an option in the set.</para>
        /// <para><paramref name="property"/> is already used by an option in the set.</para>
        /// <para>There is no implicit conversion from string to <typeparamref name="TOptionValue"/>.</para>
        /// </exception>
        public void AddOption<TOptionValue>(
            char shortName,
            Expression<Func<TOptionSet, TOptionValue>> property,
            string helpText)
        {
            if (property == null) throw new ArgumentNullException(nameof(property));
            if (helpText == null) throw new ArgumentNullException(nameof(helpText));

            AddOptionImpl(shortName, null, property, null, helpText, null);
        }

        /// <summary>
        /// Adds a new option to the set.
        /// </summary>
        /// <typeparam name="TOptionValue">The type of the option value.</typeparam>
        /// <param name="longName">The option's long name.</param>
        /// <param name="property">
        /// The property of <typeparamref name="TOptionSet"/> that will hold the option's value.
        /// </param>
        /// <param name="helpText">The help-text description of the option.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="longName"/>, <paramref name="property"/>, or <paramref name="helpText"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <para> <paramref name="longName"/> is an invalid option name.</para>
        /// <para><paramref name="property"/> is not a writable property of <typeparamref name="TOptionSet"/>.</para>
        /// <para><paramref name="helpText"/> is empty or whitespace.</para>
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// <para> <paramref name="longName"/> already refers to an option in the set.</para>
        /// <para><paramref name="property"/> is already used by an option in the set.</para>
        /// <para>There is no implicit conversion from string to <typeparamref name="TOptionValue"/>.</para>
        /// </exception>
        public void AddOption<TOptionValue>(
            string longName,
            Expression<Func<TOptionSet, TOptionValue>> property,
            string helpText)
        {
            if (longName == null) throw new ArgumentNullException(nameof(longName));
            if (property == null) throw new ArgumentNullException(nameof(property));
            if (helpText == null) throw new ArgumentNullException(nameof(helpText));

            AddOptionImpl(null, longName, property, null, helpText, null);
        }

        /// <summary>
        /// Adds a new option to the set.
        /// </summary>
        /// <typeparam name="TOptionValue">The type of the option value.</typeparam>
        /// <param name="shortName">The option's short name.</param>
        /// <param name="longName">The option's long name.</param>
        /// <param name="property">
        /// The property of <typeparamref name="TOptionSet"/> that will hold the option's value.
        /// </param>
        /// <param name="helpText">The help-text description of the option.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="longName"/>, <paramref name="property"/>, or <paramref name="helpText"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <para><paramref name="shortName"/> or <paramref name="longName"/> is an invalid option name.</para>
        /// <para><paramref name="property"/> is not a writable property of <typeparamref name="TOptionSet"/>.</para>
        /// <para><paramref name="helpText"/> is empty or whitespace.</para>
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// <para>
        /// <paramref name="shortName"/> or <paramref name="longName"/> already refers to an option in the set.
        /// </para>
        /// <para><paramref name="property"/> is already used by an option in the set.</para>
        /// <para>There is no implicit conversion from string to <typeparamref name="TOptionValue"/>.</para>
        /// </exception>
        public void AddOption<TOptionValue>(
            char shortName,
            string longName,
            Expression<Func<TOptionSet, TOptionValue>> property,
            string helpText)
        {
            if (longName == null) throw new ArgumentNullException(nameof(longName));
            if (property == null) throw new ArgumentNullException(nameof(property));
            if (helpText == null) throw new ArgumentNullException(nameof(helpText));

            AddOptionImpl(shortName, longName, property, null, helpText, null);
        }

        /// <summary>
        /// Adds a new option to the set.
        /// </summary>
        /// <typeparam name="TOptionValue">The type of the option value.</typeparam>
        /// <param name="shortName">The option's short name.</param>
        /// <param name="property">
        /// The property of <typeparamref name="TOptionSet"/> that will hold the option's value.
        /// </param>
        /// <param name="optionValueParser">The function to use to parse the option value.</param>
        /// <param name="helpText">The help-text description of the option.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="property"/>, <paramref name="optionValueParser"/>, or <paramref name="helpText"/> is
        /// <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <para><paramref name="shortName"/> is an invalid option name.</para>
        /// <para><paramref name="property"/> is not a writable property of <typeparamref name="TOptionSet"/>.</para>
        /// <para><paramref name="helpText"/> is empty or whitespace.</para>
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// <para>
        /// <paramref name="shortName"/> already refers to an option in the set.
        /// </para>
        /// <para><paramref name="property"/> is already used by an option in the set.</para>
        /// </exception>
        public void AddOption<TOptionValue>(
            char shortName,
            Expression<Func<TOptionSet, TOptionValue>> property,
            Func<string, TOptionValue> optionValueParser,
            string helpText)
        {
            if (property == null) throw new ArgumentNullException(nameof(property));
            if (optionValueParser == null) throw new ArgumentNullException(nameof(optionValueParser));
            if (helpText == null) throw new ArgumentNullException(nameof(helpText));

            AddOptionImpl(shortName, null, property, optionValueParser, helpText, null);
        }

        /// <summary>
        /// Adds a new option to the set.
        /// </summary>
        /// <typeparam name="TOptionValue">The type of the option value.</typeparam>
        /// <param name="longName">The option's long name.</param>
        /// <param name="property">
        /// The property of <typeparamref name="TOptionSet"/> that will hold the option's value.
        /// </param>
        /// <param name="optionValueParser">The function to use to parse the option value.</param>
        /// <param name="helpText">The help-text description of the option.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="longName"/>, <paramref name="property"/>, <paramref name="optionValueParser"/>,
        /// <paramref name="helpText"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <para><paramref name="longName"/> is an invalid option name.</para>
        /// <para><paramref name="property"/> is not a writable property of <typeparamref name="TOptionSet"/>.</para>
        /// <para><paramref name="helpText"/> is empty or whitespace.</para>
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// <para>
        /// <paramref name="longName"/> already refers to an option in the set.
        /// </para>
        /// <para><paramref name="property"/> is already used by an option in the set.</para>
        /// </exception>
        public void AddOption<TOptionValue>(
            string longName,
            Expression<Func<TOptionSet, TOptionValue>> property,
            Func<string, TOptionValue> optionValueParser,
            string helpText)
        {
            if (longName == null) throw new ArgumentNullException(nameof(longName));
            if (property == null) throw new ArgumentNullException(nameof(property));
            if (optionValueParser == null) throw new ArgumentNullException(nameof(optionValueParser));
            if (helpText == null) throw new ArgumentNullException(nameof(helpText));

            AddOptionImpl(null, longName, property, optionValueParser, helpText, null);
        }

        /// <summary>
        /// Adds a new option to the set.
        /// </summary>
        /// <typeparam name="TOptionValue">The type of the option value.</typeparam>
        /// <param name="shortName">The option's short name.</param>
        /// <param name="longName">The option's long name.</param>
        /// <param name="property">
        /// The property of <typeparamref name="TOptionSet"/> that will hold the option's value.
        /// </param>
        /// <param name="optionValueParser">The function to use to parse the option value.</param>
        /// <param name="helpText">The help-text description of the option.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="longName"/>, <paramref name="property"/>, <paramref name="optionValueParser"/>,
        /// <paramref name="helpText"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <para><paramref name="shortName"/> or <paramref name="longName"/> is an invalid option name.</para>
        /// <para><paramref name="property"/> is not a writable property of <typeparamref name="TOptionSet"/>.</para>
        /// <para><paramref name="helpText"/> is empty or whitespace.</para>
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// <para>
        /// <paramref name="shortName"/> or <paramref name="longName"/> already refers to an option in the set.
        /// </para>
        /// <para><paramref name="property"/> is already used by an option in the set.</para>
        /// </exception>
        public void AddOption<TOptionValue>(
            char shortName,
            string longName,
            Expression<Func<TOptionSet, TOptionValue>> property,
            Func<string, TOptionValue> optionValueParser,
            string helpText)
        {
            if (longName == null) throw new ArgumentNullException(nameof(longName));
            if (property == null) throw new ArgumentNullException(nameof(property));
            if (optionValueParser == null) throw new ArgumentNullException(nameof(optionValueParser));
            if (helpText == null) throw new ArgumentNullException(nameof(helpText));

            AddOptionImpl(shortName, longName, property, optionValueParser, helpText, null);
        }

        /// <summary>
        /// Adds a new option to the set.
        /// </summary>
        /// <typeparam name="TOptionValue">The type of the option value.</typeparam>
        /// <param name="shortName">The option's short name.</param>
        /// <param name="property">
        /// The property of <typeparamref name="TOptionSet"/> that will hold the option's value.
        /// </param>
        /// <param name="helpText">The help-text description of the option.</param>
        /// <param name="helpTextValueName">
        /// The name to use as a placeholder for the option value in help-text examples.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="property"/>, <paramref name="helpText"/>, or <paramref name="helpTextValueName"/> is
        /// <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <para><paramref name="shortName"/> is an invalid option name.</para>
        /// <para><paramref name="property"/> is not a writable property of <typeparamref name="TOptionSet"/>.</para>
        /// <para><paramref name="helpText"/> is empty or whitespace.</para>
        /// <para><paramref name="helpTextValueName"/> is not a single, alpha-numeric token.</para>
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// <para>
        /// <paramref name="shortName"/> already refers to an option in the set.
        /// </para>
        /// <para><paramref name="property"/> is already used by an option in the set.</para>
        /// </exception>
        public void AddOption<TOptionValue>(
            char shortName,
            Expression<Func<TOptionSet, TOptionValue>> property,
            string helpText,
            string helpTextValueName)
        {
            if (property == null) throw new ArgumentNullException(nameof(property));
            if (helpText == null) throw new ArgumentNullException(nameof(helpText));
            if (helpTextValueName == null) throw new ArgumentNullException(nameof(helpTextValueName));

            AddOptionImpl(shortName, null, property, null, helpText, helpTextValueName);
        }

        /// <summary>
        /// Adds a new option to the set.
        /// </summary>
        /// <typeparam name="TOptionValue">The type of the option value.</typeparam>
        /// <param name="longName">The option's long name.</param>
        /// <param name="property">
        /// The property of <typeparamref name="TOptionSet"/> that will hold the option's value.
        /// </param>
        /// <param name="helpText">The help-text description of the option.</param>
        /// <param name="helpTextValueName">
        /// The name to use as a placeholder for the option value in help-text examples.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="longName"/>, <paramref name="property"/>, <paramref name="helpText"/>, or
        /// <paramref name="helpTextValueName"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <para><paramref name="longName"/> is an invalid option name.</para>
        /// <para><paramref name="property"/> is not a writable property of <typeparamref name="TOptionSet"/>.</para>
        /// <para><paramref name="helpText"/> is empty or whitespace.</para>
        /// <para><paramref name="helpTextValueName"/> is not a single, alpha-numeric token.</para>
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// <para>
        /// <paramref name="longName"/> already refers to an option in the set.
        /// </para>
        /// <para><paramref name="property"/> is already used by an option in the set.</para>
        /// </exception>
        public void AddOption<TOptionValue>(
            string longName,
            Expression<Func<TOptionSet, TOptionValue>> property,
            string helpText,
            string helpTextValueName)
        {
            if (longName == null) throw new ArgumentNullException(nameof(longName));
            if (property == null) throw new ArgumentNullException(nameof(property));
            if (helpText == null) throw new ArgumentNullException(nameof(helpText));
            if (helpTextValueName == null) throw new ArgumentNullException(nameof(helpTextValueName));

            AddOptionImpl(null, longName, property, null, helpText, helpTextValueName);
        }

        /// <summary>
        /// Adds a new option to the set.
        /// </summary>
        /// <typeparam name="TOptionValue">The type of the option value.</typeparam>
        /// <param name="shortName">The option's short name.</param>
        /// <param name="longName">The option's long name.</param>
        /// <param name="property">The property of <typeparamref name="TOptionSet"/> that will hold the option's value.</param>
        /// <param name="helpText">The help-text description of the option.</param>
        /// <param name="helpTextValueName">
        /// The name to use as a placeholder for the option value in help-text examples.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="longName"/>, <paramref name="property"/>, <paramref name="helpText"/>, or
        /// <paramref name="helpTextValueName"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <para><paramref name="shortName"/> or <paramref name="longName"/> is an invalid option name.</para>
        /// <para><paramref name="property"/> is not a writable property of <typeparamref name="TOptionSet"/>.</para>
        /// <para><paramref name="helpText"/> is empty or whitespace.</para>
        /// <para><paramref name="helpTextValueName"/> is not a single, alpha-numeric token.</para>
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// <para>
        /// <paramref name="shortName"/> or <paramref name="longName"/> already refers to an option in the set.
        /// </para>
        /// <para><paramref name="property"/> is already used by an option in the set.</para>
        /// </exception>
        public void AddOption<TOptionValue>(
            char shortName,
            string longName,
            Expression<Func<TOptionSet, TOptionValue>> property,
            string helpText,
            string helpTextValueName)
        {
            if (longName == null) throw new ArgumentNullException(nameof(longName));
            if (property == null) throw new ArgumentNullException(nameof(property));
            if (helpText == null) throw new ArgumentNullException(nameof(helpText));
            if (helpTextValueName == null) throw new ArgumentNullException(nameof(helpTextValueName));

            AddOptionImpl(shortName, longName, property, null, helpText, helpTextValueName);
        }

        /// <summary>
        /// Adds a new option to the set.
        /// </summary>
        /// <typeparam name="TOptionValue">The type of the option value.</typeparam>
        /// <param name="shortName">The option's short name.</param>
        /// <param name="property">
        /// The property of <typeparamref name="TOptionSet"/> that will hold the option's value.
        /// </param>
        /// <param name="optionValueParser">The function to use to parse the option value.</param>
        /// <param name="helpText">The help-text description of the option.</param>
        /// <param name="helpTextValueName">
        /// The name to use as a placeholder for the option value in help-text examples.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="property"/>, <paramref name="optionValueParser"/>, <paramref name="helpText"/>, or
        /// <paramref name="helpTextValueName"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <para><paramref name="shortName"/> is an invalid option name.</para>
        /// <para><paramref name="property"/> is not a writable property of <typeparamref name="TOptionSet"/>.</para>
        /// <para><paramref name="helpText"/> is empty or whitespace.</para>
        /// <para><paramref name="helpTextValueName"/> is not a single, alpha-numeric token.</para>
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// <para>
        /// <paramref name="shortName"/> already refers to an option in the set.
        /// </para>
        /// <para><paramref name="property"/> is already used by an option in the set.</para>
        /// </exception>
        public void AddOption<TOptionValue>(
            char shortName,
            Expression<Func<TOptionSet, TOptionValue>> property,
            Func<string, TOptionValue> optionValueParser,
            string helpText,
            string helpTextValueName)
        {
            if (property == null) throw new ArgumentNullException(nameof(property));
            if (optionValueParser == null) throw new ArgumentNullException(nameof(optionValueParser));
            if (helpText == null) throw new ArgumentNullException(nameof(helpText));
            if (helpTextValueName == null) throw new ArgumentNullException(nameof(helpTextValueName));

            AddOptionImpl(shortName, null, property, optionValueParser, helpText, helpTextValueName);
        }

        /// <summary>
        /// Adds a new option to the set.
        /// </summary>
        /// <typeparam name="TOptionValue">The type of the option value.</typeparam>
        /// <param name="longName">The option's long name.</param>
        /// <param name="property">
        /// The property of <typeparamref name="TOptionSet"/> that will hold the option's value.
        /// </param>
        /// <param name="optionValueParser">The function to use to parse the option value.</param>
        /// <param name="helpText">The help-text description of the option.</param>
        /// <param name="helpTextValueName">
        /// The name to use as a placeholder for the option value in help-text examples.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="longName"/>, <paramref name="property"/>, <paramref name="optionValueParser"/>,
        /// <paramref name="helpText"/>, or <paramref name="helpTextValueName"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <para><paramref name="longName"/> is an invalid option name.</para>
        /// <para><paramref name="property"/> is not a writable property of <typeparamref name="TOptionSet"/>.</para>
        /// <para><paramref name="helpText"/> is empty or whitespace.</para>
        /// <para><paramref name="helpTextValueName"/> is not a single, alpha-numeric token.</para>
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// <para>
        /// <paramref name="longName"/> already refers to an option in the set.
        /// </para>
        /// <para><paramref name="property"/> is already used by an option in the set.</para>
        /// </exception>
        public void AddOption<TOptionValue>(
            string longName,
            Expression<Func<TOptionSet, TOptionValue>> property,
            Func<string, TOptionValue> optionValueParser,
            string helpText,
            string helpTextValueName)
        {
            if (longName == null) throw new ArgumentNullException(nameof(longName));
            if (property == null) throw new ArgumentNullException(nameof(property));
            if (optionValueParser == null) throw new ArgumentNullException(nameof(optionValueParser));
            if (helpText == null) throw new ArgumentNullException(nameof(helpText));
            if (helpTextValueName == null) throw new ArgumentNullException(nameof(helpTextValueName));

            AddOptionImpl(null, longName, property, optionValueParser, helpText, helpTextValueName);
        }

        /// <summary>
        /// Adds a new option to the set.
        /// </summary>
        /// <typeparam name="TOptionValue">The type of the option value.</typeparam>
        /// <param name="shortName">The option's short name.</param>
        /// <param name="longName">The option's long name.</param>
        /// <param name="property">
        /// The property of <typeparamref name="TOptionSet"/> that will hold the option's value.
        /// </param>
        /// <param name="optionValueParser">The function to use to parse the option value.</param>
        /// <param name="helpText">The help-text description of the option.</param>
        /// <param name="helpTextValueName">
        /// The name to use as a placeholder for the option value in help-text examples.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="longName"/>, <paramref name="property"/>, <paramref name="optionValueParser"/>,
        /// <paramref name="helpText"/>, or <paramref name="helpTextValueName"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <para><paramref name="shortName"/> or <paramref name="longName"/> is an invalid option name.</para>
        /// <para><paramref name="property"/> is not a writable property of <typeparamref name="TOptionSet"/>.</para>
        /// <para><paramref name="helpText"/> is empty or whitespace.</para>
        /// <para><paramref name="helpTextValueName"/> is not a single, alpha-numeric token.</para>
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// <para>
        /// <paramref name="shortName"/> or <paramref name="longName"/> already refers to an option in the set.
        /// </para>
        /// <para><paramref name="property"/> is already used by an option in the set.</para>
        /// </exception>
        public void AddOption<TOptionValue>(
            char shortName,
            string longName,
            Expression<Func<TOptionSet, TOptionValue>> property,
            Func<string, TOptionValue> optionValueParser,
            string helpText,
            string helpTextValueName)
        {
            if (longName == null) throw new ArgumentNullException(nameof(longName));
            if (property == null) throw new ArgumentNullException(nameof(property));
            if (optionValueParser == null) throw new ArgumentNullException(nameof(optionValueParser));
            if (helpText == null) throw new ArgumentNullException(nameof(helpText));
            if (helpTextValueName == null) throw new ArgumentNullException(nameof(helpTextValueName));

            AddOptionImpl(shortName, longName, property, optionValueParser, helpText, helpTextValueName);
        }

        /// <summary>
        /// Gets an <see cref="IRuleBuilder{TOptionSet}"/> for the option associated with <paramref name="property"/>.
        /// </summary>
        /// <param name="property">The option to build rules for.</param>
        /// <returns>The <see cref="IRuleBuilder{TOptionSet}"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="property"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="property"/> is not a writable property of <typeparamref name="TOptionSet"/>.
        /// </exception>
        public IRuleBuilder<TOptionSet> Option<TOptionValue>(Expression<Func<TOptionSet, TOptionValue>> property)
        {
            if (property == null) throw new ArgumentNullException(nameof(property));

            // PropertySelector throws the right exceptions for us.
            var selector = new PropertySelector<TOptionSet, TOptionValue>(property);

            return new RuleBuilder(this, selector.Member.Name);
        }

        /// <summary>
        /// Builds a parser for the option set as currently configured.
        /// </summary>
        /// <returns>The <see cref="IOptionSetParser{TOptions}"/>.</returns>
        /// <exception cref="InvalidOperationException">
        /// The state of the <see cref="OptionSet{T}"/> is invalid.
        /// </exception>
        public IOptionSetParser<TOptionSet> BuildParser()
        {
            void ensurePropertyIsOption(string property)
            {
                if (!_options.ContainsKey(property))
                    throw new InvalidOperationException($"Property {property} is not used by an option.");
            }

            foreach (var requirement in _requirements)
            {
                ensurePropertyIsOption(requirement.Dependent);
                ensurePropertyIsOption(requirement.Dependee);
            }

            foreach (var (first, second) in _preclusions)
            {
                ensurePropertyIsOption(first);
                ensurePropertyIsOption(second);
            }

            foreach (var requirement in _requirements)
            {
                if (_preclusions.Contains((requirement.Dependent, requirement.Dependee))
                    || _preclusions.Contains((requirement.Dependee, requirement.Dependent)))
                {
                    var dependent = DescriptiveName(_options[requirement.Dependent]);
                    var dependee = DescriptiveName(_options[requirement.Dependee]);

                    throw new InvalidOperationException(
                        $"Option {dependee} is both required and precluded by option {dependee}.");
                }
            }

            var ruleSet = new RuleSet<string>(_requirements, _preclusions);
            return new OptionSetParser<TOptionSet>(
                _defaultOptionsFactory, new Dictionary<string, IOption<TOptionSet>>(_options), ruleSet);
        }

        private void AddOptionImpl<TOptionValue>(
            char? shortName,
            string? longName,
            Expression<Func<TOptionSet, TOptionValue>> property,
            Func<string, TOptionValue>? optionValueParser,
            string helpText,
            string? helpTextValueName)
        {
            var optShortName = null as OptionName.Short;
            if (shortName.HasValue)
            {
                optShortName = OptionName.Parse($"{shortName}") as OptionName.Short ??
                    throw new ArgumentException("Invalid short option name.", nameof(shortName));
            }

            var optLongName = null as OptionName.Long;
            if (longName != null)
            {
                optLongName = OptionName.Parse(longName) as OptionName.Long ??
                    throw new ArgumentException("Invalid long option name.", nameof(longName));
            }

            // PropertySelector throws the right exceptions for us.
            var selector = new PropertySelector<TOptionSet, TOptionValue>(property);

            if (string.IsNullOrWhiteSpace(helpText))
                throw new ArgumentException("Must not be empty or whitespace.", nameof(helpText));

            if (helpTextValueName != null)
            {
                if (helpTextValueName.Length == 0
                    || helpTextValueName.Any(c => !(char.IsLetter(c) || char.IsDigit(c) || c == '_' || c == '-')))
                {
                    throw new ArgumentException("Must a single alpha-numeric token.", nameof(helpTextValueName));
                }
            }

            helpTextValueName ??= $"<{typeof(TOptionSet).Name}>";

            if (optShortName != null && _options.Values.Any(o => o.ShortName == optShortName))
                throw new InvalidOperationException($"Set already contains option '{optShortName}'.");

            if (optLongName != null && _options.Values.Any(o => o.LongName == optLongName))
                throw new InvalidOperationException($"Set already contains option '{optLongName}'.");

            var memberName = selector.Member.Name;
            if (_options.TryGetValue(memberName, out var existingOption))
            {
                throw new InvalidOperationException(
                    $"Property '{memberName}' is already assigned to {DescriptiveName(existingOption)}");
            }

            optionValueParser ??= GetDefaultParser<TOptionValue>();

            var option = new Option<TOptionSet, TOptionValue>(optShortName, optLongName, optionValueParser, selector.Setter);
            _options.Add(memberName, option);
        }

        private Func<string, TOptionValue> GetDefaultParser<TOptionValue>()
        {
            var converter = TypeDescriptor.GetConverter(typeof(TOptionValue));
            if (!converter.CanConvertFrom(typeof(string)))
                throw new InvalidOperationException($"Type {typeof(TOptionValue).Name} has no default parser.");

            return (string input) => (TOptionValue)converter.ConvertFromString(input);
        }

        private string DescriptiveName(IOption<TOptionSet> option)
        {
            if (option.LongName == null)
                return $"-{option.ShortName}";
            if (option.ShortName == null)
                return $"--{option.LongName}";
            return $"-{option.ShortName}|--{option.LongName}";
        }

        private class RuleBuilder : IRuleBuilder<TOptionSet>
        {
            private readonly OptionSet<TOptionSet> _parent;
            private readonly string _targetOptionProperty;

            public RuleBuilder(OptionSet<TOptionSet> parent, string target)
            {
                _parent = parent;
                _targetOptionProperty = target;
            }
            
            public IRuleBuilder<TOptionSet> Requires<TOptionValue>(Expression<Func<TOptionSet, TOptionValue>> property)
            {
                if (property == null) throw new ArgumentNullException(nameof(property));

                // PropertySelector throws the right exceptions for us.
                var selector = new PropertySelector<TOptionSet, TOptionValue>(property);

                var memberName = selector.Member.Name;
                if (memberName == _targetOptionProperty)
                    throw new InvalidOperationException("The given property matches the target option's property.");

                _parent._requirements.Add(new Dependency<string>(_targetOptionProperty, memberName));

                return this;
            }

            public IRuleBuilder<TOptionSet> Precludes<TOptionValue>(Expression<Func<TOptionSet, TOptionValue>> property)
            {
                if (property == null) throw new ArgumentNullException(nameof(property));

                // PropertySelector throws the right exceptions for us.
                var selector = new PropertySelector<TOptionSet, TOptionValue>(property);

                var memberName = selector.Member.Name;
                if (memberName == _targetOptionProperty)
                    throw new InvalidOperationException("The given property matches the target option's property.");

                _parent._preclusions.Add((_targetOptionProperty, memberName));

                return this;
            }

        }
    }
}

