using Parsimony.ParserBuilder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Parsimony.Tests.Builder
{
    public class TestPropertySelector
    {
#nullable disable //Testing construction
        [Fact]
        public void Construct_SelectorNull_Throws()
        {
            Assert.Throws<ArgumentNullException>(() => new PropertySelector<TestDummy, bool>(null));
        }
#nullable enable

        [Fact]
        public void Construct_ExpressionIsNotMemberExpression_Throws()
        {
            Assert.Throws<ArgumentException>(() => Construct(x => x.GetMeADoot()));
        }

        [Fact]
        public void Construct_ExpressionIsNotMemberAccessNodeType_Throws()
        {
            Assert.Throws<ArgumentException>(() => Construct(x => (int)x.IntProp));
        }

        [Fact]
        public void Construct_ExpressionIsNotPropertyAccess_Throws()
        {
            Assert.Throws<ArgumentException>(() => Construct(x => x.NotAProperty));
        }

        [Fact]
        public void Construct_TargetedPropertyIsReadOnly_Throws()
        {
            Assert.Throws<ArgumentException>(() => Construct(x => x.ReadOnly));
        }


        [Fact]
        public void Construct_ExpressionIsValidSelector_PostConditionsMet()
        {
            var underTest = Construct(x => x.IntProp);
            var target = new TestDummy();

            underTest.Setter(target, 10);
            var res = underTest.Getter(target);
            

            Assert.Equal(nameof(target.IntProp), underTest.MemberName);
            Assert.Equal(target.IntProp, res);
        }

        private PropertySelector<TestDummy, T> Construct<T>(Expression<Func<TestDummy, T>> expression)
        {
            return new PropertySelector<TestDummy, T>(expression);
        
        }

    }
}
