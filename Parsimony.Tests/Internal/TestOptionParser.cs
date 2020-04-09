using Moq;
using Parsimony.Internal;
using System;
using System.Linq;
using Xunit;

namespace Parsimony.Tests.Internal
{
    public class TestOptionParser
    {
        [Fact]
        public void Ctor_NullOptions_Throws()
        {
#nullable disable
            Assert.Throws<ArgumentNullException>("options", () => new OptionParser<object>(null));
#nullable enable
        }

        [Fact]
        public void Ctor_ValidOptions_Constructs()
        {
            var _ = new OptionParser<object>(Array.Empty<IOption<object>>());
        }

        [Fact]
        public void Parse_NullInput_Throws()
        {
            var underTest = new OptionParser<object>(Array.Empty<IOption<object>>());
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

            var underTest = new OptionParser<object>(new[] { expectedOption.Object });

            var _ = underTest.Parse(input);

            expectedOption.Verify(o => o.Parse(expectedValue), Times.Once);
        }

        [Fact]
        public void Parse_AdjoinedFlagOptions_ExpectedOptionsHaveParseInvoked()
        {
            var optionA = new Mock<IOption<object>>();
            optionA.SetupGet(o => o.ShortName).Returns(OptionName.Parse("a") as OptionName.Short);
            optionA.SetupGet(o => o.IsFlag).Returns(true);

            var optionB = new Mock<IOption<object>>();
            optionB.SetupGet(o => o.ShortName).Returns(OptionName.Parse("b") as OptionName.Short);
            optionB.SetupGet(o => o.IsFlag).Returns(true);

            var optionC = new Mock<IOption<object>>();
            optionC.SetupGet(o => o.ShortName).Returns(OptionName.Parse("c") as OptionName.Short);
            optionC.SetupGet(o => o.IsFlag).Returns(false);

            var underTest = new OptionParser<object>(new[] { optionA.Object, optionB.Object, optionC.Object });

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

            var underTest = new OptionParser<object>(new[] { option.Object });

            (var _, var input) = underTest.Parse(new[] { "-a", "rgument" });

            Assert.Equal(new[] { "rgument" }.AsEnumerable(), input);
        }

        [Fact]
        public void Parse_SomeOptionsFound_TheExpectedActionsAreReturned()
        {
            var optionA = new Mock<IOption<object>>();
            optionA.SetupGet(o => o.ShortName).Returns(OptionName.Parse("a") as OptionName.Short);
            optionA.SetupGet(o => o.IsFlag).Returns(true);
            var assignOptionA = new Mock<Action<object>>().Object;
            optionA.Setup(o => o.Parse(It.IsAny<string>())).Returns(assignOptionA);

            var optionB = new Mock<IOption<object>>();
            optionB.SetupGet(o => o.ShortName).Returns(OptionName.Parse("b") as OptionName.Short);
            optionB.SetupGet(o => o.IsFlag).Returns(false);
            var assignOptionB = new Mock<Action<object>>().Object;
            optionB.Setup(o => o.Parse(It.IsAny<string>())).Returns(assignOptionB);

            var optionC = new Mock<IOption<object>>();
            optionC.SetupGet(o => o.ShortName).Returns(OptionName.Parse("c") as OptionName.Short);
            optionC.SetupGet(o => o.IsFlag).Returns(false);
            var assignOptionC = new Mock<Action<object>>().Object;
            optionC.Setup(o => o.Parse(It.IsAny<string>())).Returns(assignOptionC);

            var underTest = new OptionParser<object>(new[] { optionA.Object, optionB.Object, optionC.Object });

            (var assignments, var _) = underTest.Parse(new[] { "-a", "-c", "defg" });

            Assert.Equal(2, assignments.Count());
            Assert.Contains(assignOptionA, assignments);
            Assert.DoesNotContain(assignOptionB, assignments);
            Assert.Contains(assignOptionC, assignments);
        }

        [Fact]
        public void Parse_SomeOptionsFound_TheExpectedTokenStreamIsReturned()
        {
            var optionD = new Mock<IOption<object>>();
            optionD.SetupGet(o => o.ShortName).Returns(OptionName.Parse("d") as OptionName.Short);
            optionD.SetupGet(o => o.IsFlag).Returns(true);

            var optionH = new Mock<IOption<object>>();
            optionH.SetupGet(o => o.ShortName).Returns(OptionName.Parse("h") as OptionName.Short);
            optionH.SetupGet(o => o.IsFlag).Returns(false);

            var underTest = new OptionParser<object>(new[] { optionD.Object, optionH.Object });

            (var _, var arguments) = underTest.Parse(new[] { "abc", "-d", "efg", "-h", "ijk", "lmno" });

            Assert.Equal(new[] { "abc", "efg", "lmno" }.AsEnumerable(), arguments);
        }
    }
}
