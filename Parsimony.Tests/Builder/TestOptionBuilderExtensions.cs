using Parsimony.ParserBuilder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Parsimony.Tests.Builder
{
    public class TestOptionBuilderExtensions
    {
        private readonly OptionBuilder<TestDummy, int> _longNameBuilder = new OptionBuilder<TestDummy, int>("doot", x => x.IntProp);
        private readonly OptionBuilder<TestDummy, int> _shortNameBuilder = new OptionBuilder<TestDummy, int>('d', x => x.IntProp);


        [Fact]
        public void WithLongName_InvalidName_Throws()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => _shortNameBuilder.WithLongName("123"));
        }

        [Fact]
        public void WithLongName_NameAlreadySet_Throws()
        {
            Assert.Throws<InvalidOperationException>(() => _longNameBuilder.WithLongName("dawt"));
        }

        [Fact]
        public void WithShortName_InvalidShortName_Throws()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => _longNameBuilder.WithShortName('-'));
        }

        [Fact]
        public void WithShortName_NameAlreadySet_Throws()
        {
            Assert.Throws<InvalidOperationException>(() => _shortNameBuilder.WithShortName('z'));
        }

        [Fact]
        public void Precludes_NotMemberSelector_Throws()
        {
            Assert.Throws<ArgumentException>(() => _shortNameBuilder.Precludes(x => x.GetMeADoot()));
        }

        [Fact]
        public void WithLongName_ValidName_SetsName()
        {
            _shortNameBuilder.WithLongName("doot");
            Assert.Equal("doot", _shortNameBuilder.LongName);
        }

        [Fact]
        public void WithShortName_ValidShortName_SetsName()
        {
            _longNameBuilder.WithShortName('d');
            Assert.Equal('d', _longNameBuilder.ShortName);
        }

        [Fact]
        public void Precludes_ValidMemberSelector_AddsNameToPrecludesList()
        {
            _longNameBuilder.Precludes(x => x.StringProp);
            var rule = _longNameBuilder.Rules.Single();
            
            Assert.Equal("IntProp", rule.PropertyName);
            Assert.Equal("StringProp", rule.Target);
            Assert.Equal(RuleKind.Precludes, rule.Kind);
        }

        [Fact]
        public void Requires_ValidMemberSelector_AddsNameToPrecludesList()
        {
            _longNameBuilder.Requires(x => x.StringProp);
            
            var rule = _longNameBuilder.Rules.Single();
            Assert.Equal("IntProp", rule.PropertyName);
            Assert.Equal("StringProp", rule.Target);
            Assert.Equal(RuleKind.Requires, rule.Kind);
        }
    }
}
