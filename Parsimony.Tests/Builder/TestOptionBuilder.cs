using Parsimony.ParserBuilder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Parsimony.Tests
{
    public class TestOptionBuilder
    {

        //Disabling nullable for ctor tests
#nullable disable
        [Fact]
        public void ConstructLongName_LongNameNull_Throws()
        {
            Assert.Throws<ArgumentNullException>(() => new OptionBuilder<TestDummy, bool>(null as string, x => x.BoolProp));
        }

        [Fact]
        public void ConstructBothNames_LongNameNull_Throws()
        {
            Assert.Throws<ArgumentNullException>(() => new OptionBuilder<TestDummy, bool>('w', null, x => x.BoolProp));
        }

        [Fact]
        public void ConstructLongName_SelectorNull_Throws()
        {
            Assert.Throws<ArgumentNullException>(() => new OptionBuilder<TestDummy, bool>("waka", null));
        }

        [Fact]
        public void ConstructShortName_SelectorNull_Throws()
        {
            Assert.Throws<ArgumentNullException>(() => new OptionBuilder<TestDummy, bool>('w', null));
        }

        [Fact]
        public void ConstructBothNames_SelectorNull_Throws()
        {
            Assert.Throws<ArgumentNullException>(() => new OptionBuilder<TestDummy, bool>('w', "waka", null));
        }

#nullable enable

        [Fact]
        public void Construct_SelectorNotMember_Throws()
        {
            Assert.Throws<ArgumentException>(() => new OptionBuilder<TestDummy, bool>("waka", x => x.GetMeADoot()));
        }

        [Fact]
        public void ConstructBothNames_BothNamesDefined_ConstructsWithNames()
        {
            var underTest = new OptionBuilder<TestDummy, bool>('w', "waka", x => x.BoolProp);
            Assert.Equal("waka", underTest?.LongName ?? "");
            Assert.Equal("w", underTest?.ShortName ?? "");

        }

        [Fact]
        public void Build_RequirePrecluded_Throws()
        {
            var underTest = Construct();

            underTest.Requires(x => x.BoolProp)
                .Precludes(x => x.BoolProp);

            Assert.Throws<InvalidOperationException>(() => underTest.Build());
        }

        [Fact]
        public void Build_RequireSamePropertyTwice_Throws()
        {
            var underTest = Construct();

            underTest.Requires(x => x.BoolProp)
                .Requires(x => x.BoolProp);

            Assert.Throws<InvalidOperationException>(() => underTest.Build());
        }

        [Fact]
        public void Build_PrecludeSamePropertyTwice_Throws()
        {
            var underTest = Construct();

            underTest.Precludes(x => x.BoolProp)
                .Precludes(x => x.BoolProp);

            Assert.Throws<InvalidOperationException>(() => underTest.Build());
        }

        [Fact]
        public void Build_ParserNullAndObjectHasTypeConverter_UsesDefault()
        {
            var optionBuilder = new OptionBuilder<TestDummy, int>('n', x => x.IntProp);

            var res = optionBuilder.Build();

            Assert.NotNull(optionBuilder.Parser);
            //CN - It aint null. ^
#pragma warning disable CS8602 // Dereference of a possibly null reference.
            Assert.Equal(10, optionBuilder.Parser("10"));
#pragma warning restore CS8602 // Dereference of a possibly null reference.
        }

        [Fact]
        public void Build_ParserNullAndObjectHasNoTypeConverter_Throws()
        {
            var optionBuilder = new OptionBuilder<TestDummy, NoTypeConverter>('n', x => x.NoConverter);

            Assert.Throws<InvalidOperationException>(() => optionBuilder.Build());

        }

        private OptionBuilder<TestDummy,int> Construct()
        {
            return new OptionBuilder<TestDummy, int>('w', "waka", x => x.IntProp);
        }


        

    }
}
