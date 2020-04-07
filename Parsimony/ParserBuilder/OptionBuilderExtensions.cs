using System;
using System.Linq.Expressions;

namespace Parsimony.ParserBuilder
{
    public static class OptionBuilderExtensions
    {
        public static T WithLongName<T>(this T optionBuilder, string longName) where T : IOptionBuilder
        {
            optionBuilder.LongName = longName;
            return optionBuilder;
        }

        public static T WithShortName<T>(this T optionBuilder, char shortName) where T : IOptionBuilder
        {
            optionBuilder.ShortName = shortName;
            return optionBuilder;
        }


        public static IOptionBuilder<TOptions, TProp> Precludes<TOptions, TProp, TTarget>( 
            this IOptionBuilder<TOptions, TProp> optionBuilder,
            Expression<Func<TOptions, TTarget>> expression)
        {
            var selector = new PropertySelector<TOptions, TTarget>(expression);
            var precluded = selector.MemberName;
            optionBuilder.Precludes.Add(precluded);

            return optionBuilder;
        }

        public static IOptionBuilder<TOptions, TProp> Requires<TOptions, TProp, TTarget>(
            this IOptionBuilder<TOptions, TProp> optionBuilder,
            Expression<Func<TOptions, TTarget>> expression)
        {
            var selector = new PropertySelector<TOptions, TTarget>(expression);
            var requires = selector.MemberName;
            optionBuilder.Requires.Add(requires);

            return optionBuilder;
        }

    }
}
