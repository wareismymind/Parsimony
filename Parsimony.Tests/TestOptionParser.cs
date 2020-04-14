using Moq;
using Parsimony.Internal;
using System;
using System.Linq;
using Xunit;

namespace Parsimony.Tests
{
    public class TestOptionParser
    {
        [Fact]
        public void Ctor_NullOptions_Throws()
        {
#nullable disable
            Assert.Throws<ArgumentNullException>("options", () => new OptionParser<object>(null, () => new object()));
#nullable enable
        }

        [Fact]
        public void Ctor_NullFactory_Throws()
        {
#nullable disable
            Assert.Throws<ArgumentNullException>(
                "optionSetFactory", () => new OptionParser<object>(Array.Empty<IOption<object>>(), null));
#nullable enable
        }

        [Fact]
        public void Ctor_ValidOptions_Constructs()
        {
            var _ = new OptionParser<object>(Array.Empty<IOption<object>>(), () => new object());
        }

        [Fact]
        public void Parse_NullInput_Throws()
        {
            var underTest = new OptionParser<object>(Array.Empty<IOption<object>>(), () => new object());
#nullable disable
            Assert.Throws<ArgumentNullException>("input", () => underTest.Parse(null));
#nullable enable
        }

        [Theory]
        [InlineData(true, "true", "-e")]                        // standalone short-name flag
        [InlineData(false, "banana", "-ebanana")]               // adjoined value on short-name option
        [InlineData(false, "banana", "-e", "banana")]           // short-name option with value in next token
        [InlineData(true, "true", "--expected")]                // standalone long-name flag
        [InlineData(false, "banana", "--expected=banana")]      // equals-joined value on long-name option
        [InlineData(false, "banana", "--expected", "banana")]   // short-name option with value in next token
        public void Parse_SomeOptionsFound_ExpectedOptionsHaveParseInvoked(
            bool isFlag, string expectedValue, params string[] input)
        {
            var shortName = OptionName.Parse("e") as OptionName.Short;
            var longName = OptionName.Parse("expected") as OptionName.Long;

            var expectedOption = new Mock<IOption<object>>();
            expectedOption.SetupGet(o => o.ShortName).Returns(shortName);
            expectedOption.SetupGet(o => o.LongName).Returns(longName);
            expectedOption.SetupGet(o => o.IsFlag).Returns(isFlag);
            expectedOption.Setup(o => o.Parse(It.IsAny<string>())).Returns(o => { });

            var underTest = new OptionParser<object>(new[] { expectedOption.Object }, () => new object());

            var _ = underTest.Parse(input);

            expectedOption.Verify(o => o.Parse(expectedValue), Times.Once);
        }

        [Fact]
        public void Parse_AdjoinedFlagOptions_ExpectedOptionsHaveParseInvoked()
        {
            var optionA = new Mock<IOption<object>>();
            optionA.SetupGet(o => o.ShortName).Returns(OptionName.Parse("a") as OptionName.Short);
            optionA.SetupGet(o => o.IsFlag).Returns(true);
            optionA.Setup(o => o.Parse(It.IsAny<string>())).Returns(o => { });

            var optionB = new Mock<IOption<object>>();
            optionB.SetupGet(o => o.ShortName).Returns(OptionName.Parse("b") as OptionName.Short);
            optionB.SetupGet(o => o.IsFlag).Returns(true);
            optionB.Setup(o => o.Parse(It.IsAny<string>())).Returns(o => { });

            var optionC = new Mock<IOption<object>>();
            optionC.SetupGet(o => o.ShortName).Returns(OptionName.Parse("c") as OptionName.Short);
            optionC.SetupGet(o => o.IsFlag).Returns(false);
            optionC.Setup(o => o.Parse(It.IsAny<string>())).Returns(o => { });

            var underTest =
                new OptionParser<object>(new[] { optionA.Object, optionB.Object, optionC.Object }, () => new object());

            var _ = underTest.Parse(new[] { "-abcdefg" });

            optionA.Verify(o => o.Parse("true"), Times.Once);
            optionB.Verify(o => o.Parse("true"), Times.Once);
            optionC.Verify(o => o.Parse("defg"), Times.Once);
        }

        [Fact]
        public void Parse_TokenAfterFlag_BecomesArgument()
        {
            var option = new Mock<IOption<object>>();
            option.SetupGet(o => o.ShortName).Returns(OptionName.Parse("a") as OptionName.Short);
            option.SetupGet(o => o.IsFlag).Returns(true);
            option.Setup(o => o.Parse(It.IsAny<string>())).Returns(o => { });

            var underTest = new OptionParser<object>(new[] { option.Object }, () => new object());

            var result = underTest.Parse(new[] { "-a", "rgument" });

            Assert.Equal(new[] { "rgument" }.AsEnumerable(), result.Arguments);
        }

        [Fact]
        public void Parse_SomeOptionsFound_TheExpectedAssignmentsOccurred()
        {
            var optionA = new Mock<IOption<object>>();
            optionA.SetupGet(o => o.ShortName).Returns(OptionName.Parse("a") as OptionName.Short);
            optionA.SetupGet(o => o.IsFlag).Returns(true);
            var assignOptionA = new Mock<Action<object>>();
            optionA.Setup(o => o.Parse(It.IsAny<string>())).Returns(assignOptionA.Object);

            var optionB = new Mock<IOption<object>>();
            optionB.SetupGet(o => o.ShortName).Returns(OptionName.Parse("b") as OptionName.Short);
            optionB.SetupGet(o => o.IsFlag).Returns(false);
            var assignOptionB = new Mock<Action<object>>();
            optionB.Setup(o => o.Parse(It.IsAny<string>())).Returns(assignOptionB.Object);

            var optionC = new Mock<IOption<object>>();
            optionC.SetupGet(o => o.ShortName).Returns(OptionName.Parse("c") as OptionName.Short);
            optionC.SetupGet(o => o.IsFlag).Returns(false);
            var assignOptionC = new Mock<Action<object>>();
            optionC.Setup(o => o.Parse(It.IsAny<string>())).Returns(assignOptionC.Object);

            var underTest =
                new OptionParser<object>(new[] { optionA.Object, optionB.Object, optionC.Object }, () => new object());

            var result = underTest.Parse(new[] { "-a", "-c", "defg" });

#nullable disable
            assignOptionA.Verify(a => a.Invoke(result.OptionSet), Times.Once);
            assignOptionB.Verify(a => a.Invoke(It.IsAny<object>()), Times.Never);
            assignOptionC.Verify(a => a.Invoke(result.OptionSet), Times.Once);
#nullable enable
        }

        [Fact]
        public void Parse_SomeOptionsFound_TheExpectedTokenStreamIsReturned()
        {
            var optionD = new Mock<IOption<object>>();
            optionD.SetupGet(o => o.ShortName).Returns(OptionName.Parse("d") as OptionName.Short);
            optionD.SetupGet(o => o.IsFlag).Returns(true);
            optionD.Setup(o => o.Parse(It.IsAny<string>())).Returns(o => { });

            var optionH = new Mock<IOption<object>>();
            optionH.SetupGet(o => o.ShortName).Returns(OptionName.Parse("h") as OptionName.Short);
            optionH.SetupGet(o => o.IsFlag).Returns(false);
            optionH.Setup(o => o.Parse(It.IsAny<string>())).Returns(o => { });

            var underTest = new OptionParser<object>(new[] { optionD.Object, optionH.Object }, () => new object());

            var result = underTest.Parse(new[] { "abc", "-d", "efg", "-h", "ijk", "lmno" });

            Assert.Equal(new[] { "abc", "efg", "lmno" }.AsEnumerable(), result.Arguments);
        }

        [Fact]
        public void Parse_UnknownOption_ReturnsExpectedError()
        {
            var underTest = new OptionParser<object>(new IOption<object>[]{ }, () => new object());

            var result = underTest.Parse(new[] { "-f" });

            var err = Assert.IsType<UnknownOptionError>(result.Error);
            Assert.Equal("f", err.OptionName);
        }

        [Fact]
        public void Parse_MissingValue_ReturnsExpectedError()
        {
            var option = new Mock<IOption<object>>();
            option.SetupGet(o => o.ShortName).Returns(OptionName.Parse("v") as OptionName.Short);
            option.SetupGet(o => o.IsFlag).Returns(false);
            option.Setup(o => o.Parse(It.IsAny<string>())).Returns(o => { });
            var underTest = new OptionParser<object>(new[] { option.Object }, () => new object());

            var result = underTest.Parse(new[] { "-v" });

            var err = Assert.IsType<MissingOptionValueError>(result.Error);
            Assert.Equal("v", err.OptionName);
        }
    }
}
