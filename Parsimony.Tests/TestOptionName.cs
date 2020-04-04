using System;
using Xunit;

namespace Parsimony.Tests
{
    public class TestOptionName
    {
        [Theory]
        [InlineData('-')]
        [InlineData('3')]
        [InlineData('@')]
        [InlineData('\u06ec')]
        public void CtorChar_Invalid_Throws(char shortName)
        {
            Assert.Throws<ArgumentOutOfRangeException>("shortName", () => new OptionName(shortName));
        }

        [Theory]
        [InlineData('A')]
        [InlineData('b')]
        [InlineData('ά')]
        public void CtorChar_Valid_Constructs(char shortName)
        {
            var _ = new OptionName(shortName);
        }

        [Fact]
        public void CtorString_Null_Throws()
        {
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
            Assert.Throws<ArgumentNullException>("longName", () => new OptionName(null));
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
        }

        [Theory]
        [InlineData("")]
        [InlineData("a")]
        [InlineData("-foo")]
        [InlineData("bar-")]
        [InlineData("b--az")]
        [InlineData("n0tjus5tle77ers")]
        public void CtorString_Invalid_Throws(string longName)
        {
            Assert.Throws<ArgumentOutOfRangeException>("longName", () => new OptionName(longName));
        }

        [Theory]
        [InlineData("UPPER")]
        [InlineData("lower")]
        [InlineData("mIxEd")]
        [InlineData("nonlάtin")]
        [InlineData("with-dashes")]
        public void CtorString_Valid_Constructs(string longName)
        {
            var _ = new OptionName(longName);
        }

        [Theory]
        [InlineData('-')]
        [InlineData('3')]
        [InlineData('@')]
        [InlineData('\u06ec')]
        public void CtorCharString_InvalidShortName_Throws(char shortName)
        {
            Assert.Throws<ArgumentOutOfRangeException>("shortName", () => new OptionName(shortName, "foo"));
        }

        [Fact]
        public void CtorCharString_Null_Throws()
        {
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
            Assert.Throws<ArgumentNullException>("longName", () => new OptionName('a', null));
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
        }

        [Theory]
        [InlineData("")]
        [InlineData("a")]
        [InlineData("-foo")]
        [InlineData("bar-")]
        [InlineData("b--az")]
        [InlineData("n0tjus5tle77ers")]
        public void CtorCharString_InvalidLongName_Throws(string longName)
        {
            Assert.Throws<ArgumentOutOfRangeException>("longName", () => new OptionName('a', longName));
        }

        [Fact]
        public void CtorCharString_Valid_Constructs()
        {
            var _ = new OptionName('a', "foo");
        }

        [Fact]
        public void ShortName_NotSet_IsNull()
        {
            var underTest = new OptionName("foo");
            Assert.Null(underTest.ShortName);
        }

        [Fact]
        public void ShortName_ShortNameOnly_HasExpectedValue()
        {
            var underTest = new OptionName('a');
            Assert.Equal('a', underTest.ShortName);
        }

        [Fact]
        public void ShortName_BothNames_HasExpectedValue()
        {
            var underTest = new OptionName('a', "foo");
            Assert.Equal('a', underTest.ShortName);
        }

        [Fact]
        public void LongName_NotSet_IsNull()
        {
            var underTest = new OptionName('a');
            Assert.Null(underTest.LongName);
        }

        [Fact]
        public void LongName_LongNameOnly_HasExpectedValue()
        {
            var underTest = new OptionName("foo");
            Assert.Equal("foo", underTest.LongName);
        }

        [Fact]
        public void LongName_BothNames_HasExpectedValue()
        {
            var underTest = new OptionName('a', "foo");
            Assert.Equal("foo", underTest.LongName);
        }

        [Theory]
        [InlineData('A')]
        [InlineData('b')]
        [InlineData('ά')]
        public void FullName_ShortNameOnly_EqualsShortName(char shortName)
        {
            var underTest = new OptionName(shortName);
            Assert.Equal($"-{shortName}", underTest.DescriptiveName);
        }

        [Theory]
        [InlineData("mIxEd")]
        [InlineData("nonlάtin")]
        [InlineData("with-dashes")]
        public void FullName_LongNameOnly_EqualsLongName(string longName)
        {
            var underTest = new OptionName(longName);
            Assert.Equal($"--{longName}", underTest.DescriptiveName);
        }

        [Theory]
        [InlineData('A', "mIxEd")]
        [InlineData('b', "nonlάtin")]
        [InlineData('ά', "with-dashes")]
        public void FullName_ShortNameOnly_ShortNamePipeLongName(char shortName, string longName)
        {
            var underTest = new OptionName(shortName, longName);
            Assert.Equal($"-{shortName}|--{longName}", underTest.DescriptiveName);
        }
    }
}
