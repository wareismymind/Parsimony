using System;
using System.Linq;
using Xunit;

namespace Parsimony.Tests
{
    public partial class TestParser
    {
        [Fact]
        public void ParseReturnsDefaultValuesForUnsetOptions()
        {
            var underTest = new Parser<Options>(Array.Empty<OptionSpec<Options>>(), new ParserConfig());
            var result = underTest.Parse();
            Assert.False(result.Options.Foo);
            Assert.Null(result.Options.Bar);
            Assert.Equal(0, result.Options.Baz);
        }

        [Fact]
        public void ParseSetsShortNameFlagsWithNoArgument()
        {
            var underTest = new Parser<Options>(_standardOptionSpecs, new ParserConfig());

            var result = underTest.Parse("-f");

            Assert.True(result.Options.Foo);
        }

        [Fact]
        public void ParseSetsShortNameOptionWithAdjoinedValue()
        {
            var underTest = new Parser<Options>(_standardOptionSpecs, new ParserConfig());

            var result = underTest.Parse("-barnana");

            Assert.Equal("arnana", result.Options.Bar);
        }

        [Fact]
        public void ParseSetsShortNameOptionWithSeparateValue()
        {
            var underTest = new Parser<Options>(_standardOptionSpecs, new ParserConfig());

            var result = underTest.Parse("-b", "arnana");

            Assert.Equal("arnana", result.Options.Bar);
        }

        [Fact]
        public void ParseUsesParseFn()
        {
            var optionSpecs = new OptionSpec<Options>[]
            {
                 new OptionSpec<Options, string?>(
                    shortName: 'b',
                    longName: null,
                    required: false,
                    @default: default,
                    parseFn: (s) => string.Join("", s.Reverse()),
                    setFn: (opts, v) => opts.Bar = v)
            };

            var underTest = new Parser<Options>(optionSpecs, new ParserConfig());

            var result = underTest.Parse("-b", "arnana");

            Assert.Equal("ananra", result.Options.Bar);
        }

        [Fact]
        public void ParseWithoutOptionsFirstParsesAllOptions()
        {
            var underTest = new Parser<Options>(_standardOptionSpecs, new ParserConfig
            {
                OptionsFirst = false
            });

            var result = underTest.Parse("-b", "arnana", "groucho", "-f", "chico", "-z", "17");

            Assert.True(result.Options.Foo);
            Assert.Equal("arnana", result.Options.Bar);
            Assert.Equal(17, result.Options.Baz);
        }

        [Fact]
        public void ParseWithOptionsFirstParsesOptionsUpToFirstArgument()
        {
            var underTest = new Parser<Options>(_standardOptionSpecs, new ParserConfig
            {
                OptionsFirst = true
            });

            var result = underTest.Parse("-b", "arnana", "groucho", "-f", "chico", "-z", "17");

            Assert.False(result.Options.Foo);
            Assert.Equal("arnana", result.Options.Bar);
            Assert.NotEqual(17, result.Options.Baz);
        }

        [Fact]
        public void ParseWithoutDoubleDashParsesAllOptions()
        {
            var underTest = new Parser<Options>(_standardOptionSpecs, new ParserConfig
            {
                DoubleDash = false
            });

            var result = underTest.Parse("harpo", "-b", "arnana", "--", "groucho", "-f", "chico", "-z", "17");

            Assert.True(result.Options.Foo);
            Assert.Equal("arnana", result.Options.Bar);
            Assert.Equal(17, result.Options.Baz);
        }

        [Fact]
        public void ParseWithDoubleDashStopsParsingOptionsAfterDoubleDash()
        {
            var underTest = new Parser<Options>(_standardOptionSpecs, new ParserConfig
            {
                DoubleDash = true
            });

            var result = underTest.Parse("harpo", "-b", "arnana", "--", "groucho", "-f", "chico", "-z", "17");

            Assert.False(result.Options.Foo);
            Assert.Equal("arnana", result.Options.Bar);
            Assert.NotEqual(17, result.Options.Baz);
        }
    }
}
