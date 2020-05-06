using Parsimony.Internal;
using System;
using Xunit;

namespace Parsimony.Tests.Internal
{
    public class TestOptionName
    {
#nullable disable
        [Fact]
        public void Parse_Null_Throws()
        {
            Assert.Throws<ArgumentNullException>("input", () => OptionName.Parse(null));
        }
#nullable enable

        [Fact]
        public void Parse_ShortName_ReturnsShortName()
        {
            var optionName = OptionName.Parse("f");
            Assert.IsType<OptionName.Short>(optionName);
#pragma warning disable CS8604 // Possible null reference argument.
            Assert.Equal("f", optionName);
#pragma warning restore CS8604
        }

        [Fact]
        public void Parse_NonAsciiShortName_ReturnsShortName()
        {
            var optionName = OptionName.Parse("ά");
            Assert.IsType<OptionName.Short>(optionName);
#pragma warning disable CS8604 // Possible null reference argument.
            Assert.Equal("ά", optionName);
#pragma warning restore CS8604
        }

        [Theory]
        [InlineData("foo")]
        [InlineData("foo-bar")]
        public void Parse_LongName_ReturnsLongName(string longName)
        {
            var optionName = OptionName.Parse(longName);
            Assert.IsType<OptionName.Long>(optionName);
#pragma warning disable CS8604 // Possible null reference argument.
            Assert.Equal(longName, optionName);
#pragma warning restore CS8604
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

#pragma warning disable CS8602 // Dereference of a possibly null reference.
            Assert.False(underTest.Equals(null as object));
#pragma warning restore CS8602
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
        }
    }
}
