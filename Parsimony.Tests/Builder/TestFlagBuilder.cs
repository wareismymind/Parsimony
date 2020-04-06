using Parsimony.ParserBuilder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Parsimony.Tests
{
    public class TestFlagBuilder
    {

//Disabling nullable for ctor tests
#nullable disable
        [Fact]
        public void ConstructLongName_LongNameNull_Throws()
        {
            Assert.Throws<ArgumentException>(() => new FlagOptionBuilder<TestDummy>(null as string, x => x.BoolProp));
        }

        [Fact]
        public void ConstructLongName_SelectorNull_Throws()
        {
            Assert.Throws<ArgumentException>(() => new FlagOptionBuilder<TestDummy>("waka", null));
        }

        [Fact]
        public void ConstructShortName_SelectorNull_Throws()
        {
            Assert.Throws<ArgumentException>(() => new FlagOptionBuilder<TestDummy>(null as string, null));
        }

#nullable enable

        [Fact]
        public void Construct_SelectorNotMember_Throws()
        {
            Assert.Throws<ArgumentException>(() => new FlagOptionBuilder<TestDummy>("waka", x => x.GetMeADoot()));
        }

    }

    public class TestDummy
    {
        public bool BoolProp { get; set; }
        public string? StringProp { get; set; }
        public int IntProp { get; set; }

        public bool GetMeADoot() => true;
    }
}
