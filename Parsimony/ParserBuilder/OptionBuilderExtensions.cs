using Parsimony.Internal;
using System;
using System.ComponentModel;
using System.Linq.Expressions;

namespace Parsimony.ParserBuilder
{
    /// <summary>
    /// Extension methods to configure an OptionBuilder instance
    /// </summary>
    public static class OptionBuilderExtensions
    {
        /// <summary> Sets the long name for the option being built </summary>
        /// <typeparam name="TOption"> The underlying option type containing the property <typeparamref name="TProp"/> </typeparam>
        /// <typeparam name="TProp"> The type of the property that the built <typeparamref name="TOption"/> maps to </typeparam>
        /// <param name="builder"> The <see cref="OptionBuilder{TOption, TProp}"/> that you want to configure </param>
        /// <param name="longName"> The long name of the option </param>
        /// <exception cref="ArgumentException"> If <paramref name="longName"/> is not a valid long name for an option </exception>
        /// <exception cref="InvalidOperationException"> <paramref name="builder"/> has had a its long name set previously </exception>
        /// <exception cref="ArgumentNullException"> if <paramref name="builder"/> or <paramref name="longName"/> are <c>null</c></exception>
        /// <returns> The input <see cref="OptionBuilder{TOption, TProp}"/></returns>
        public static OptionBuilder<TOption, TProp> WithLongName<TOption, TProp>(this OptionBuilder<TOption, TProp> builder, string longName)
            where TOption : notnull
        {
            if (builder == null)
                throw new ArgumentNullException(nameof(builder));

            builder.LongName = OptionName.Parse(longName) as OptionName.Long
                ?? throw new ArgumentException($"Could not convert value to long name '{longName}'", nameof(longName));
            return builder;
        }

        /// <summary> Sets the short name for the option being built </summary>
        /// <typeparam name="TOption"> The underlying option type containing the property <typeparamref name="TProp"/> </typeparam>
        /// <typeparam name="TProp"> The type of the property that the built <typeparamref name="TOption"/> maps to </typeparam>
        /// <param name="builder"> The <see cref="OptionBuilder{TOption, TProp}"/> that you want to configure </param>
        /// <param name="shortName"> The long name of the option </param>
        /// <exception cref="ArgumentException"> <paramref name="shortName"/> is not a valid long name for a flag </exception>
        /// <exception cref="InvalidOperationException"> <paramref name="builder"/> has had a its short name set previously </exception>
        /// <exception cref="ArgumentNullException"> <paramref name="builder"/> or <paramref name="shortName"/> are <c>null</c></exception>
        /// <returns> The input <see cref="OptionBuilder{TOption, TProp}"/></returns>
        public static OptionBuilder<TOption, TProp> WithShortName<TOption, TProp>(this OptionBuilder<TOption, TProp> builder, char shortName)
            where TOption : notnull
        {
            if (builder == null)
                throw new ArgumentNullException(nameof(builder));

            builder.ShortName = OptionName.Parse(shortName.ToString()) as OptionName.Short
                ?? throw new ArgumentException($"Could not convert value to short name '{shortName}'", nameof(shortName));
            return builder;
        }


        /// <summary> Sets a custom parser to be used to parse the string inputs </summary>
        /// <typeparam name="TOption"> The underlying option type containing the property <typeparamref name="TProp"/> </typeparam>
        /// <typeparam name="TProp"> The type of the property that the built <typeparamref name="TOption"/> maps to </typeparam>
        /// <param name="builder"> The <see cref="OptionBuilder{TOption, TProp}"/> that you want to configure </param>
        /// <param name="parser">
        /// The parser that will be used to convert commandline inputs to values of type <typeparamref name="TProp"/> 
        /// </param>
        /// <exception cref="ArgumentNullException"> <paramref name="builder"/> or <paramref name="parser"/> are null </exception>
        /// <returns> The input <see cref="OptionBuilder{TOption, TProp}"/></returns>
        /// <remarks> If no parser is set a default <see cref="TypeConverter"/> will be reflected </remarks>
        public static OptionBuilder<TOption, TProp> WithParser<TOption, TProp>(
            this OptionBuilder<TOption, TProp> builder,
            Func<string, TProp> parser)
            where TOption : notnull
        {
            if (builder == null)
                throw new ArgumentNullException(nameof(builder));
            builder.Parser = parser ?? throw new ArgumentNullException(nameof(parser));
            return builder;
        }


        /// <summary>
        /// Sets a property on the parent <typeparamref name="TOption"/> that is incompatible with this option
        /// </summary>
        /// <typeparam name="TOption"> The underlying option type containing the property <typeparamref name="TProp"/> </typeparam>
        /// <typeparam name="TProp"> The type of the property that the built <typeparamref name="TOption"/> maps to </typeparam>
        /// <typeparam name="TTarget"> The type of the paramter to be precluded </typeparam>
        /// <param name="builder"> The <see cref="OptionBuilder{TOption, TProp}"/> that you want to configure </param>
        /// <param name="expression">
        /// A property selector expression that targets the incompatible property on the <typeparamref name="TOption"/> 
        /// </param>
        /// <exception cref="ArgumentNullException"> <paramref name="builder"/> or <paramref name="expression"/> are null </exception>
        /// <exception cref="ArgumentException"> 
        /// <paramref name="expression"/> is not a property selector expression targeting a property that is both 
        /// readable and writeable 
        /// </exception>
        /// <returns> The input <see cref="OptionBuilder{TOption, TProp}"/></returns>
        public static OptionBuilder<TOption, TProp> Precludes<TOption, TProp, TTarget>(
            this OptionBuilder<TOption, TProp> builder,
            Expression<Func<TOption, TTarget>> expression)
            where TOption : notnull
        {
            if (builder == null)
                throw new ArgumentNullException(nameof(builder));

            var selector = new PropertySelector<TOption, TTarget>(expression);
            var rule = new Rule(RuleKind.Precludes, builder.Selector.MemberName, selector.MemberName);
            builder.Rules.Add(rule);

            return builder;
        }


        /// <summary>
        /// Sets a property on the parent <typeparamref name="TOption"/> that is required to be set if this property is set
        /// </summary>
        /// <typeparam name="TOption"> The underlying option type containing the property <typeparamref name="TProp"/> </typeparam>
        /// <typeparam name="TProp"> The type of the property that the built <typeparamref name="TOption"/> maps to </typeparam>
        /// <typeparam name="TTarget"> The type of the required parameter </typeparam>
        /// <param name="builder"> The <see cref="OptionBuilder{TOption, TProp}"/> that you want to configure </param>
        /// <param name="expression">
        /// A property selector expression that targets the required property on the <typeparamref name="TOption"/> 
        /// </param>
        /// <exception cref="ArgumentNullException"> <paramref name="builder"/> or <paramref name="expression"/> are null </exception>
        /// <exception cref="ArgumentException"> 
        /// <paramref name="expression"/> is not a property selector expression targeting a property that is both 
        /// readable and writeable 
        /// </exception>
        /// <returns> The input <see cref="OptionBuilder{TOption, TProp}"/></returns>
        public static OptionBuilder<TOption, TProp> Requires<TOption, TProp, TTarget>(
            this OptionBuilder<TOption, TProp> builder,
            Expression<Func<TOption, TTarget>> expression)
            where TOption : notnull
        {
            if (builder == null)
                throw new ArgumentNullException(nameof(builder));

            var selector = new PropertySelector<TOption, TTarget>(expression);
            var rule = new Rule(RuleKind.Requires, builder.Selector.MemberName, selector.MemberName);
            builder.Rules.Add(rule);

            return builder;
        }

    }
}
