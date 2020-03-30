using Xunit;

namespace Parsimony.Tests
{
    public partial class TestParser
    {
        [Fact]
        public void ParseCollectsArguments()
        {
            var underTest = new Parser<Options>(_standardOptionSpecs, new ParserConfig());

            var result = underTest.Parse("harpo", "-b", "arnana", "groucho", "-f", "chico");

            Assert.Equal(new[] { "harpo", "groucho", "chico" }, result.Arguments);
        }

        [Fact]
        public void ParseWithoutOptionsFirstOnlyCollectsNonOptions()
        {
            var underTest = new Parser<Options>(_standardOptionSpecs, new ParserConfig
            {
                OptionsFirst = false
            });

            var result = underTest.Parse("-b", "arnana", "harpo", "-f", "groucho", "-z", "17");

            Assert.Equal(new[] { "harpo", "groucho" }, result.Arguments);
        }

        [Fact]
        public void ParseWithOptionsFirstTreatsAllTokensAfterTheFirstNonOptionAsArguments()
        {
            var underTest = new Parser<Options>(_standardOptionSpecs, new ParserConfig
            {
                OptionsFirst = true
            });

            var result = underTest.Parse("-b", "arnana", "harpo", "-f", "groucho", "-z", "17");

            Assert.Equal(new[] { "harpo", "-f", "groucho", "-z", "17" }, result.Arguments);
        }

        [Fact]
        public void ParseWithoutDoubleDashCollectsNonOptionsAndDoubleDash()
        {
            var underTest = new Parser<Options>(_standardOptionSpecs, new ParserConfig
            {
                DoubleDash = false
            });

            var result = underTest.Parse("harpo", "-b", "arnana", "--", "groucho", "-f", "chico", "-z", "17");

            Assert.Equal(new[] { "harpo", "--", "groucho", "chico" }, result.Arguments);
        }

        [Fact]
        public void ParseWithDoubleDashTreatsAllTokensAfterDoubleDashAsArguments()
        {
            var underTest = new Parser<Options>(_standardOptionSpecs, new ParserConfig
            {
                DoubleDash = true
            });

            var result = underTest.Parse("harpo", "-b", "arnana", "--", "groucho", "-f", "chico", "-z", "17");

            Assert.Equal(new[] { "harpo", "groucho", "-f", "chico", "-z", "17" }, result.Arguments);
        }

        [Fact]
        public void ParseWithDoubleDashConsumesSubsequentDoubleDashes()
        {
            var underTest = new Parser<Options>(_standardOptionSpecs, new ParserConfig
            {
                DoubleDash = true
            });

            var result = underTest.Parse("harpo", "-b", "arnana", "--", "groucho", "-f", "--", "chico", "-z", "17");

            Assert.Equal(new[] { "harpo", "groucho", "-f", "--", "chico", "-z", "17" }, result.Arguments);
        }

        [Fact]
        public void ParseWithDoubleDashAndOptionsFirstRespectsRedundantDoubleDash()
        {
            var underTest = new Parser<Options>(_standardOptionSpecs, new ParserConfig
            {
                DoubleDash = true,
                OptionsFirst = true,
            });

            var result = underTest.Parse("-b", "arnana", "groucho", "--", "-f", "chico", "-z", "17");

            Assert.Equal(new[] { "groucho", "-f", "chico", "-z", "17" }, result.Arguments);
        }
    }
}
