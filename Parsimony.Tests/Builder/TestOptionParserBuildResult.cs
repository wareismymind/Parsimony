using Parsimony.Internal;
using Parsimony.ParserBuilder;
using System;
using System.Collections.Generic;
using Xunit;

namespace Parsimony.Tests.Builder
{
    public class TestOptionParserBuildResult
    {
#nullable disable
        [Fact]
        public void Construct_ParserNull_Throws()
        {
            Assert.Throws<ArgumentNullException>(() => new OptionParserBuildResult<TestDummy>(null, new List<Rule>()));
        }

        [Fact]
        public void Construct_RulesNull_Throws()
        {
            Assert.Throws<ArgumentNullException>(() => new OptionParserBuildResult<TestDummy>(new DummyParser(), null));
        }

#nullable enable
    }

    internal class DummyParser : IOption<TestDummy>
    {
        public OptionName.Short? ShortName => throw new NotImplementedException();

        public OptionName.Long? LongName => throw new NotImplementedException();

        public bool IsFlag => throw new NotImplementedException();

        public Action<TestDummy> Parse(string input) => throw new NotImplementedException();
    }

}
