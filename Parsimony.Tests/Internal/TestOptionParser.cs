using Moq;
using Parsimony.Errors;
using Parsimony.Internal;
using Parsimony.Options;
using System;
using System.Linq;
using Xunit;

namespace Parsimony.Tests
{
    public class TestOptionParser
    {
#nullable disable
        [Fact]
        public void Parse_NullOptions_Throws()
        {
            Assert.Throws<ArgumentNullException>(
                "options", () => OptionParser<object>.Parse(null, new ParserConfig(), Array.Empty<string>()));
        }

        [Fact]
        public void Parse_NullConfig_Throws()
        {
            Assert.Throws<ArgumentNullException>(
                "config",
                () => OptionParser<object>.Parse(Array.Empty<IOption<object>>(), null, Array.Empty<string>()));
        }

        [Fact]
        public void Parse_NullInput_Throws()
        {
            Assert.Throws<ArgumentNullException>(
                "input",
                () => OptionParser<object>.Parse(Array.Empty<IOption<object>>(), new ParserConfig(), null));
        }
#nullable enable

        [Theory]
        [InlineData(true, "true", "-e")]                        // standalone short-name flag
        [InlineData(false, "banana", "-ebanana")]               // adjoined value on short-name option
        [InlineData(false, "banana", "-e", "banana")]           // short-name option with value in next token
        [InlineData(true, "true", "--expected")]                // standalone long-name flag
        [InlineData(false, "banana", "--expected=banana")]      // equals-joined value on long-name option
        [InlineData(false, "banana", "--expected", "banana")]   // short-name option with value in next token
        public void Parse_OptionFound_AssignmentIsCreatedWithExpectedValue(
            bool isFlag, string expectedValue, params string[] input)
        {
            var shortName = OptionName.Parse("e") as OptionName.Short;
            var longName = OptionName.Parse("expected") as OptionName.Long;

            var expectedOption = new Mock<IOption<object>>();
            expectedOption.SetupGet(o => o.ShortName).Returns(shortName);
            expectedOption.SetupGet(o => o.LongName).Returns(longName);
            expectedOption.SetupGet(o => o.IsFlag).Returns(isFlag);
            Action<object> expectedAssignment = o => { };
            expectedOption.Setup(o => o.GetAssignment(It.IsAny<string>())).Returns((expectedAssignment, null));

            var result = OptionParser<object>.Parse(new[] { expectedOption.Object }, new ParserConfig(), input);

            expectedOption.Verify(o => o.GetAssignment(expectedValue), Times.Once);
        }

        [Fact]
        public void Parse_AdjoinedFlagOptions_ExpectedAssignmentsAreReturned()
        {
            var optionA = new Mock<IOption<object>>();
            optionA.SetupGet(o => o.ShortName).Returns(OptionName.Parse("a") as OptionName.Short);
            optionA.SetupGet(o => o.IsFlag).Returns(true);
            Action<object> assignmentA = o => { };
            optionA.Setup(o => o.GetAssignment(It.IsAny<string>())).Returns((assignmentA, null));
            var a = optionA.Object;

            var optionB = new Mock<IOption<object>>();
            optionB.SetupGet(o => o.ShortName).Returns(OptionName.Parse("b") as OptionName.Short);
            optionB.SetupGet(o => o.IsFlag).Returns(true);
            Action<object> assignmentB = o => { };
            optionB.Setup(o => o.GetAssignment(It.IsAny<string>())).Returns((assignmentB, null));
            var b = optionB.Object;

            var optionC = new Mock<IOption<object>>();
            optionC.SetupGet(o => o.ShortName).Returns(OptionName.Parse("c") as OptionName.Short);
            optionC.SetupGet(o => o.IsFlag).Returns(false);
            Action<object> assignmentC = o => { };
            optionC.Setup(o => o.GetAssignment(It.IsAny<string>())).Returns((assignmentC, null));
            var c = optionC.Object;

            var options = new[] { a, b, c };

            var result = OptionParser<object>.Parse(options, new ParserConfig(), new[] { "-abcdefg" });

            optionA.Verify(o => o.GetAssignment("true"), Times.Once);
            optionB.Verify(o => o.GetAssignment("true"), Times.Once);
            optionC.Verify(o => o.GetAssignment("defg"), Times.Once);

#pragma warning disable CS8602 // Dereference of a possibly null reference.
            Assert.Equal(3, result.Assignments.Count);
#pragma warning restore CS8602s
            Assert.Equal(assignmentA, result.Assignments[a]);
            Assert.Equal(assignmentB, result.Assignments[b]);
            Assert.Equal(assignmentC, result.Assignments[c]);
        }

        [Fact]
        public void Parse_TokenAfterFlag_BecomesArgument()
        {
            var option = new Mock<IOption<object>>();
            option.SetupGet(o => o.ShortName).Returns(OptionName.Parse("a") as OptionName.Short);
            option.SetupGet(o => o.IsFlag).Returns(true);
            option.Setup(o => o.GetAssignment(It.IsAny<string>())).Returns((o => { }, null));

            var options = new[] { option.Object };

            var result = OptionParser<object>.Parse(options, new ParserConfig(), new[] { "-a", "rgument" });

            Assert.Equal(new[] { "rgument" }.AsEnumerable(), result.Arguments);
        }

        [Fact]
        public void Parse_SomeOptionsFound_TheExpectedAssignmentsAreReturned()
        {
            var optionA = new Mock<IOption<object>>();
            optionA.SetupGet(o => o.ShortName).Returns(OptionName.Parse("a") as OptionName.Short);
            optionA.SetupGet(o => o.IsFlag).Returns(true);
            Action<object> assignmentA = o => { };
            optionA.Setup(o => o.GetAssignment(It.IsAny<string>())).Returns((assignmentA, null));
            var a = optionA.Object;

            var optionB = new Mock<IOption<object>>();
            optionB.SetupGet(o => o.ShortName).Returns(OptionName.Parse("b") as OptionName.Short);
            optionB.SetupGet(o => o.IsFlag).Returns(false);
            Action<object> assignmentB = o => { };
            optionB.Setup(o => o.GetAssignment(It.IsAny<string>())).Returns((assignmentB, null));
            var b = optionB.Object;

            var optionC = new Mock<IOption<object>>();
            optionC.SetupGet(o => o.ShortName).Returns(OptionName.Parse("c") as OptionName.Short);
            optionC.SetupGet(o => o.IsFlag).Returns(false);
            Action<object> assignmentC = o => { };
            optionC.Setup(o => o.GetAssignment(It.IsAny<string>())).Returns((assignmentC, null));
            var c = optionC.Object;

            var options = new[] { a, b, c };

            var result = OptionParser<object>.Parse(options, new ParserConfig(), new[] { "-a", "-c", "defg" });


#pragma warning disable CS8602 // Dereference of a possibly null reference.
            Assert.Equal(2, result.Assignments.Count);
#pragma warning restore CS8602
            Assert.Equal(assignmentA, result.Assignments[a]);
            Assert.Equal(assignmentC, result.Assignments[c]);
        }

        [Fact]
        public void Parse_SomeOptionsFound_TheExpectedTokenStreamIsReturned()
        {
            var optionD = new Mock<IOption<object>>();
            optionD.SetupGet(o => o.ShortName).Returns(OptionName.Parse("d") as OptionName.Short);
            optionD.SetupGet(o => o.IsFlag).Returns(true);
            optionD.Setup(o => o.GetAssignment(It.IsAny<string>())).Returns((o => { }, null));

            var optionH = new Mock<IOption<object>>();
            optionH.SetupGet(o => o.ShortName).Returns(OptionName.Parse("h") as OptionName.Short);
            optionH.SetupGet(o => o.IsFlag).Returns(false);
            optionH.Setup(o => o.GetAssignment(It.IsAny<string>())).Returns((o => { }, null));

            var options = new[] { optionD.Object, optionH.Object };

            var result = OptionParser<object>.Parse(
                options, new ParserConfig(), new[] { "abc", "-d", "efg", "-h", "ijk", "lmno" });

            Assert.Equal(new[] { "abc", "efg", "lmno" }.AsEnumerable(), result.Arguments);
        }

        [Fact]
        public void Parse_UnknownOption_ReturnsExpectedError()
        {
            var options = Array.Empty<IOption<object>>();
            var result = OptionParser<object>.Parse(options, new ParserConfig(), new[] { "-f" });

            var err = Assert.IsType<UnknownOptionError>(result.Error);
            Assert.Equal("f", err.OptionName);
        }

        [Fact]
        public void Parse_MissingValue_ReturnsExpectedError()
        {
            var option = new Mock<IOption<object>>();
            option.SetupGet(o => o.ShortName).Returns(OptionName.Parse("v") as OptionName.Short);
            option.SetupGet(o => o.IsFlag).Returns(false);
            option.Setup(o => o.GetAssignment(It.IsAny<string>())).Returns((o => { }, null));

            var result = OptionParser<object>.Parse(new[] { option.Object }, new ParserConfig(), new[] { "-v" });

            var err = Assert.IsType<MissingOptionValueError>(result.Error);
            Assert.Equal("v", err.OptionName);
        }

        [Fact]
        public void Parse_PosixOrder_StopsParsingOptionsAfterFirstNonOption()
        {
            var optionD = new Mock<IOption<object>>();
            optionD.SetupGet(o => o.ShortName).Returns(OptionName.Parse("d") as OptionName.Short);
            optionD.SetupGet(o => o.IsFlag).Returns(true);
            optionD.Setup(o => o.GetAssignment(It.IsAny<string>())).Returns((o => { }, null));

            var optionH = new Mock<IOption<object>>();
            optionH.SetupGet(o => o.ShortName).Returns(OptionName.Parse("h") as OptionName.Short);
            optionH.SetupGet(o => o.IsFlag).Returns(false);
            optionH.Setup(o => o.GetAssignment(It.IsAny<string>())).Returns((o => { }, null));

            var options = new[] { optionD.Object, optionH.Object };
            var config = new ParserConfig { PosixOptionOrder = true };
            var result = OptionParser<object>.Parse(
                options, config, new[] { "-d", "efg", "-h", "ijk", "--", "lmno", "--" });

            // Option -d is parsed, then "efg" is encountered and since it's a non-option option parsing stops and the
            // token -h is not considered an option even though the option exists.
            optionD.Verify(d => d.GetAssignment(It.IsAny<string>()), Times.Once);
            optionH.Verify(h => h.GetAssignment(It.IsAny<string>()), Times.Never);

            // The first "--" is removed but subsequent ones are left.
            Assert.Equal(new[] { "efg", "-h", "ijk", "lmno", "--" }.AsEnumerable(), result.Arguments);
        }

        [Fact]
        public void Parse_GetAssignmentReturnError_ReturnsError()
        {
            var option = new Mock<IOption<object>>();
            var optionName = OptionName.Parse("o") as OptionName.Short ??
                throw new InvalidOperationException($"Invalid option name for optionName.");
            option.SetupGet(o => o.ShortName).Returns(optionName);
            option.SetupGet(o => o.IsFlag).Returns(true);
            var error = new OptionValueFormatError(optionName, "value", "message");
            option.Setup(o => o.GetAssignment(It.IsAny<string>())).Returns((null, error));

            var options = new[] { option.Object };
            var result = OptionParser<object>.Parse(options, new ParserConfig(), new[] { "-o" });

            Assert.Same(error, result.Error);
        }
    }
}
