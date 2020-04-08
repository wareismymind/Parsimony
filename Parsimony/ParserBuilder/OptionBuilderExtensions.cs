using System;
using System.Linq.Expressions;

namespace Parsimony.ParserBuilder
{
    public static class OptionBuilderExtensions
    {
        public static OptionBuilder<TOption, TProp> WithLongName<TOption, TProp>(this OptionBuilder<TOption, TProp> builder, string longName)
            where TOption : notnull
        {
            builder.LongName = longName;
            return builder;
        }

        public static OptionBuilder<TOption, TProp> WithShortName<TOption, TProp>(this OptionBuilder<TOption, TProp> builder, char shortName)
            where TOption : notnull
        {
            builder.ShortName = shortName;
            return builder;
        }


        public static OptionBuilder<TOption, TProp> Precludes<TOption, TProp, TTarget>(
            this OptionBuilder<TOption, TProp> optionBuilder,
            Expression<Func<TOption, TTarget>> expression)
            where TOption : notnull
        {
            var selector = new PropertySelector<TOption, TTarget>(expression);
            var rule = new Rule(RuleKind.Precludes, optionBuilder.Selector.MemberName, selector.MemberName);
            optionBuilder.Rules.Add(rule);

            return optionBuilder;
        }

        public static OptionBuilder<TOption, TProp> Requires<TOption, TProp, TTarget>(
            this OptionBuilder<TOption, TProp> optionBuilder,
            Expression<Func<TOption, TTarget>> expression)
            where TOption : notnull
        {
            var selector = new PropertySelector<TOption, TTarget>(expression);
            var rule = new Rule(RuleKind.Requires, optionBuilder.Selector.MemberName, selector.MemberName);
            optionBuilder.Rules.Add(rule);

            return optionBuilder;
        }

    }
}
