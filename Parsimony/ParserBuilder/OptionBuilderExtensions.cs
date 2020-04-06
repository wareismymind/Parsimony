using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

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

        
        public static T Precludes<T, TOptions,TProp>(
            this T optionBuilder,
            Expression<Func<TOptions,TProp>> expression )
            where T : IOptionBuilder<T,TOptions>
        {
            if (expression == null)
                throw new ArgumentNullException(nameof(expression));

            if (!(expression is MemberExpression mbr))
                throw new ArgumentException("Expression must be member access expression", nameof(expression));

            var precluded = mbr.Member.Name;
            optionBuilder.Precludes.Add(precluded);

            return optionBuilder;
        }
    }
}
