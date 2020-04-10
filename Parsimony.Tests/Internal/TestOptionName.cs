using Parsimony.Internal;
using System;
using Xunit;

namespace Parsimony.Tests.Internal
{
    public class TestOptionName
    {
        [Fact]
        public void Parse_Null_Throws()
        {
#nullable disable
            Assert.Throws<ArgumentNullException>("input", () => OptionName.Parse(null));
#nullable enable
        }

        [Fact]
        public void Parse_ShortName_ReturnsShortName()
        {
            var optionName = OptionName.Parse("f");
            Assert.IsType<OptionName.Short>(optionName);
#nullable disable
            Assert.Equal("f", optionName);
#nullable enable
        }

        [Fact]
        public void Parse_NonAsciiShortName_ReturnsShortName()
        {
            var optionName = OptionName.Parse("ά");
            Assert.IsType<OptionName.Short>(optionName);
#nullable disable
            Assert.Equal("ά", optionName);
#nullable enable
        }

        [Theory]
        [InlineData("foo")]
        [InlineData("foo-bar")]
        public void Parse_LongName_ReturnsLongName(string longName)
        {
            var optionName = OptionName.Parse(longName);
            Assert.IsType<OptionName.Long>(optionName);
#nullable disable
            Assert.Equal(longName, optionName);
#nullable enable
        }

        [Theory]
        [InlineData("")]
        [InlineData("-")]
        [InlineData("--")]
        [InlineData("&")]
        [InlineData("-foo")]
        [InlineData("foo-")]
        [InlineData("foo--bar")]
        [InlineData("abc123")]
        public void Parse_InvalidName_ReturnsNull(string invalidName) => Assert.Null(OptionName.Parse(invalidName));

        [Fact]
        public void Equals_SameName_ReturnsTrue()
        {
            Assert.Equal(OptionName.Parse("foo"), OptionName.Parse("foo"));
        }

        [Fact]
        public void Equality_Works()
        {
            var underTest = OptionName.Parse("foo");
            var unequal = OptionName.Parse("bar");
            var equal = OptionName.Parse("foo");
            var @null = null as OptionName;

#nullable disable
            Assert.False(underTest.Equals(null as object));
            Assert.False(underTest.Equals(unequal as object));
            Assert.True(underTest.Equals(equal as object));

            Assert.False(underTest.Equals(null as OptionName));
            Assert.False(underTest.Equals(unequal));
            Assert.True(underTest.Equals(equal));

            Assert.False(underTest == null);
            Assert.False(underTest == unequal);
            Assert.True(underTest == equal);
            Assert.True(@null == null);

            Assert.True(underTest != null);
            Assert.True(underTest != unequal);
            Assert.False(underTest != equal);
            Assert.False(@null != null);
#nullable enable
        }
    }
}
