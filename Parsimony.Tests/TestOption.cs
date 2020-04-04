using System;
using Xunit;

namespace Parsimony.Tests
{
    public class TestOption
    {
        [Fact]
        public void Ctor_NullName_Throws()
        {
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
            Assert.Throws<ArgumentNullException>("name", () => _ = new Option<object, string>(null));
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
        }

        [Fact]
        public void Ctor_ValidArgs_Constructs()
        {
            var _ = new Option<object, string>(new OptionName('a', "foo"));
        }

        [Fact]
        public void CanParse_ShortNameFlag_ReturnsTrue()
        {
            var context = new ParseContext<Options>(new Options(false, ""), Array.Empty<string>(), new[] { "-f" });
            var underTest = new Option<Options, bool>(new OptionName('f', "foo"));
            Assert.True(underTest.CanParse(context));
        }

        [Fact]
        public void CanParse_LongNameFlag_ReturnsTrue()
        {
            var context = new ParseContext<Options>(new Options(false, ""), Array.Empty<string>(), new[] { "--foo" });
            var underTest = new Option<Options, bool>(new OptionName('f', "foo"));
            Assert.True(underTest.CanParse(context));
        }

        [Fact]
        public void CanParse_AdjoinedShortNameFlag_ReturnsTrue()
        {
            var context = new ParseContext<Options>(new Options(false, ""), Array.Empty<string>(), new[] { "-fgh" });
            var underTest = new Option<Options, bool>(new OptionName('f', "foo"));
            Assert.True(underTest.CanParse(context));
        }

        [Fact]
        public void CanParse_EqualsJoinedLongNameFlag_ReturnsTrue()
        {
            var context = new ParseContext<Options>(new Options(false, ""), Array.Empty<string>(), new[] { "--foo=yes" });
            var underTest = new Option<Options, bool>(new OptionName('f', "foo"));
            Assert.True(underTest.CanParse(context));
        }

        [Fact]
        public void CanParse_SpaceSeparatedShortNameOption_ReturnsTrue()
        {
            var context = new ParseContext<Options>(new Options(false, ""), Array.Empty<string>(), new[] { "-f", "gh" });
            var underTest = new Option<Options, string>(new OptionName('f', "foo"));
            Assert.True(underTest.CanParse(context));
        }

        [Fact]
        public void CanParse_SpaceSeparatedLongNameOption_ReturnsTrue()
        {
            var context = new ParseContext<Options>(new Options(false, ""), Array.Empty<string>(), new[] { "--foo", "gh" });
            var underTest = new Option<Options, string>(new OptionName('f', "foo"));
            Assert.True(underTest.CanParse(context));
        }

        class Options
        {
            public bool Flag { get; private set; }

            public string Value { get; private set; }

            public Options(bool flag, string value)
            {
                Flag = flag;
                Value = value ?? throw new ArgumentNullException(nameof(value));
            }
        }
    }
}
