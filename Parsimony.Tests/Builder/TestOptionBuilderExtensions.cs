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
        private readonly OptionBuilder<TestDummy, int> _emptyBuilder = new OptionBuilder<TestDummy, int>(x => x.IntProp);


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
        public void WithBothNames_BothNamesvalid_SetsBothNames()
        {
            _emptyBuilder.WithLongName("doot")
                .WithShortName('d');

            Assert.Equal("doot", _emptyBuilder.LongName);
            Assert.Equal('d', _emptyBuilder.ShortName);
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
            Assert.Contains(_longNameBuilder.Precludes, x => x == "StringProp");
        }

        [Fact]
        public void Requires_ValidMemberSelector_AddsNameToPrecludesList()
        {
            _longNameBuilder.Requires(x => x.StringProp);
            Assert.Contains(_longNameBuilder.Requires, x => x == "StringProp");
        }
    }
}
