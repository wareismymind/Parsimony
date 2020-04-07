using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Parsimony.ParserBuilder
{
    public class PropertySelector<TInput, TProp>
    {
        public string MemberName { get; }
        public Action<TInput,TProp> Setter { get; }
        public Func<TInput, TProp> Getter { get; }

        public PropertySelector(Expression<Func<TInput, TProp>> selector)
        {
            if (selector == null)
                throw new ArgumentNullException(nameof(selector));

            if (!(selector.Body is MemberExpression mbr) || selector.Body.NodeType != ExpressionType.MemberAccess)
                throw new ArgumentException("Must be a member selector expression");

            var prop = mbr.Member as PropertyInfo ?? throw new ArgumentException("Member selection must be a property");

            if (!prop.CanRead || !prop.CanWrite)
                throw new ArgumentException("Property must be readable and writable");


            var instance = Expression.Parameter(typeof(TInput));
            var value = Expression.Parameter(typeof(TProp));
            var setterBody = Expression.Call(instance, prop.GetSetMethod(),value);
            var paramset = new ParameterExpression[] { instance, value };

            MemberName = mbr.Member.Name;
            Getter = selector.Compile();
            Setter = Expression.Lambda<Action<TInput, TProp>>(setterBody, paramset).Compile();
        }
    }
}
