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
            Assert.Throws<ArgumentNullException>(() => new OptionBuilder<TestDummy, bool>(null as string, null));
        }

        [Fact]
        public void ConstructBothNames_SelectorNull_Throws()
        {
            Assert.Throws<ArgumentNullException>(() => new OptionBuilder<TestDummy, bool>('w', "waka", null));
        }

        [Fact]
        public void ConstructExpressionOnly_SelectorNull_Throws()
        {
            Assert.Throws<ArgumentNullException>(() => new OptionBuilder<TestDummy, bool>(null));
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
            new OptionBuilder<TestDummy, bool>('w',"waka", x => x.BoolProp);
        }


        

    }
}
