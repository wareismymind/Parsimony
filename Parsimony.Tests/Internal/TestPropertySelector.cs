using Moq;
using Parsimony.Internal;
using System;
using System.Linq.Expressions;
using Xunit;

namespace Parsimony.Tests.Internal
{
    public class TestPropertySelector
    {

#nullable disable
        [Fact]
        public void Construct_SelectorNull_Throws()
        {
            Assert.Throws<ArgumentNullException>("property", () => new PropertySelector<Thing, bool>(null));
        }
#nullable enable

        [Fact]
        public void Construct_ExpressionIsNotMemberExpression_Throws()
        {
            Func<Thing, bool> nonMember = (_) => false;
            Assert.Throws<ArgumentException>("property", () => Construct(x => nonMember(x)));
        }

        [Fact]
        public void Construct_ExpressionIsNotPropertyAccess_Throws()
        {
            Assert.Throws<ArgumentException>("property", () => Construct(x => x.Field));
        }

        [Fact]
        public void Construct_TargetedPropertyHasNoSetter_Throws()
        {
            Assert.Throws<ArgumentException>("property", () => Construct(x => x.ReadOnly));
        }

        [Fact]
        public void Construct_ExpressionIsValidSelector_PostConditionsMet()
        {
            var underTest = Construct(x => x.Property);
            var expected = typeof(Thing).GetMember("Property")[0];
            Assert.Equal(expected, underTest.Member);

            var target = new Thing();
            underTest.Setter(target, 10);
            Assert.Equal(10, target.Property);
        }

        private PropertySelector<Thing, TProp> Construct<TProp>(Expression<Func<Thing, TProp>> expression)
        {
            return new PropertySelector<Thing, TProp>(expression);
        }

        internal class Thing
        {
            public int Property { get; set; }

            public int Field;

            public int ReadOnly => 22;
        }
    }
}
