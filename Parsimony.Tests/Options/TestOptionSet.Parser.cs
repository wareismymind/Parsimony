using Moq;
using Parsimony.Errors;
using Parsimony.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Parsimony.Tests
{
    public partial class TestOptionSet
    {

#nullable disable

        [Fact]
        public void ParseInput_NullInput_Throws()
        {
            var underTest = new OptionSet<Opts>();
            Assert.Throws<ArgumentNullException>("input", () => underTest.BuildParser().Parse(null));
        }

        [Fact]
        public void ParseInputConfig_NullInput_Throws()
        {
            var underTest = new OptionSet<Opts>();
            Assert.Throws<ArgumentNullException>(
                "input", () => underTest.BuildParser().Parse(null, new ParserConfig()));
        }

        [Fact]
        public void ParseInputConfig_NullConfig_Throws()
        {
            var underTest = new OptionSet<Opts>();
            Assert.Throws<ArgumentNullException>(
                "config", () => underTest.BuildParser().Parse(Array.Empty<string>(), null));
        }

#nullable enable

        [Fact]
        public void Parse_DefaultOptionsFactoryWasSupplied_FactoryIsUsedForInitialOptionsValue()
        {
            var expectedOpts = new Opts();
            var defaultOptionsFactory = new Mock<Func<Opts>>();
            defaultOptionsFactory.Setup(d => d.Invoke()).Returns(() => expectedOpts);
            var underTest = new OptionSet<Opts>(defaultOptionsFactory.Object);
            var result = underTest.BuildParser().Parse(Array.Empty<string>());
            defaultOptionsFactory.Verify(d => d.Invoke(), Times.Once);
            Assert.Same(expectedOpts, result.Options);
        }

        [Fact]
        public void Parse_OptionNotSupplied_ParseAndAssignmentAreNotCalled()
        {
            var parseFn = new Mock<Func<string, string>>();
            parseFn.Setup(p => p.Invoke(It.IsAny<string>())).Returns<string>(s => s);
            var opts = new Mock<Opts>();
            var underTest = new OptionSet<Opts>(() => opts.Object);
            underTest.AddOption('o', "option", o => o.Option, parseFn.Object, "an option", "OPT");
            var result = underTest.BuildParser().Parse(Array.Empty<string>());
            parseFn.Verify(p => p.Invoke(It.IsAny<string>()), Times.Never);
            opts.VerifySet(o => o.Option = It.IsAny<string>(), Times.Never);
            Assert.Null(result.Error);
        }

        [Theory]
        [InlineData("value", "-ovalue")]
        [InlineData("value", "-o", "value")]
        [InlineData("value", "--option", "value")]
        [InlineData("value", "--option=value")]
        public void Parse_OptionSupplied_ParseAndAssignmentAreCalled(string expectedValue, params string[] args)
        {
            const string sentinel = "sentinel";
            var parseFn = new Mock<Func<string, string>>();
            parseFn.Setup(p => p.Invoke(It.IsAny<string>())).Returns<string>(s => sentinel);
            var opts = new Mock<Opts>();
            var underTest = new OptionSet<Opts>(() => opts.Object);
            underTest.AddOption('o', "option", o => o.Option, parseFn.Object, "an option", "OPT");
            var result = underTest.BuildParser().Parse(args);
            parseFn.Verify(p => p.Invoke(expectedValue), Times.Once);
            opts.VerifySet(o => o.Option = sentinel, Times.Once);
            Assert.Null(result.Error);
        }

        [Theory]
        [InlineData("true", "-f")]
        [InlineData("true", "--flag")]
        public void Parse_FlagIsSupplied_ParseAndAssignmentAreCalled(string expectedValue, params string[] args)
        {
            var parseFn = new Mock<Func<string, bool>>();
            parseFn.Setup(p => p.Invoke(It.IsAny<string>())).Returns<string>(s => s == "true");
            var opts = new Mock<Opts>();
            var underTest = new OptionSet<Opts>(() => opts.Object);
            underTest.AddOption('f', "flag", o => o.Flag, parseFn.Object, "an flag", "FLAG");
            var result = underTest.BuildParser().Parse(args);
            parseFn.Verify(p => p.Invoke(expectedValue), Times.Once);
            opts.VerifySet(o => o.Flag = expectedValue == "true", Times.Once);
        }

        [Fact]
        public void Parse_AdjoinedFlagAndOption_SetsFlagAndOption()
        {
            var flagParseFn = new Mock<Func<string, bool>>();
            flagParseFn.Setup(p => p.Invoke(It.IsAny<string>())).Returns<string>(s => s == "true");
            var optParseFn = new Mock<Func<string, string>>();
            optParseFn.Setup(p => p.Invoke(It.IsAny<string>())).Returns<string>(s => s);
            var opts = new Mock<Opts>();
            var underTest = new OptionSet<Opts>(() => opts.Object);
            underTest.AddOption('f', "flag", o => o.Flag, flagParseFn.Object, "an flag", "FLAG");
            underTest.AddOption('o', "option", o => o.Option, optParseFn.Object, "an option", "OPT");
            var result = underTest.BuildParser().Parse(new[] { "-fovalue" });
            flagParseFn.Verify(p => p.Invoke("true"), Times.Once);
            optParseFn.Verify(p => p.Invoke("value"), Times.Once);
            opts.VerifySet(o => o.Flag = true, Times.Once);
            opts.VerifySet(o => o.Option = "value" , Times.Once);
        }

        [Theory]
        [InlineData("true", "--flag")]
        [InlineData("true", "--flag=true")]
        [InlineData("false", "--flag=false")]
        public void Parse_ConvertableTypeBool_ConvertsValue(string expectedValue, params string[] args)
        {
            var parseFn = new Mock<Func<string, bool>>();
            parseFn.Setup(p => p.Invoke(It.IsAny<string>())).Returns<string>(s => s == "true");
            var opts = new Mock<Opts>();
            var underTest = new OptionSet<Opts>(() => opts.Object);
            underTest.AddOption('f', "flag", o => o.Flag, parseFn.Object, "an flag", "FLAG");
            var result = underTest.BuildParser().Parse(args);
            parseFn.Verify(p => p.Invoke(expectedValue), Times.Once);
            opts.VerifySet(o => o.Flag = expectedValue == "true", Times.Once);
        }

        [Fact]
        public void Parse_ConvertableTypeInt_ConvertsValue()
        {
            var underTest = new OptionSet<Opts>();
            underTest.AddOption('n', o => o.Int, "an integer");
            var result = underTest.BuildParser().Parse(new[] { "-n3" });
#pragma warning disable CS8602 // Dereference of a possibly null reference.
            Assert.Equal(3, result.Options.Int);
#pragma warning restore CS8602 // Dereference of a possibly null reference.
        }

        [Fact]
        public void Parse_NonOptionTokens_BecomeArguments()
        {
            var underTest = new OptionSet<Opts>();
            underTest.AddOption('o', o => o.Option, "an option");
            underTest.AddOption('f', o => o.Flag, "a flag");
            var result = underTest.BuildParser().Parse(new[] { "foo", "-o", "value", "bar", "-f", "baz" });
            Assert.Equal(new[] { "foo", "bar", "baz" }.AsEnumerable(), result.Arguments);
        }

        [Fact]
        public void Parse_DoubleDash_SubsequentTokensAreNotOptions()
        {
            var opts = new Mock<Opts>();
            var underTest = new OptionSet<Opts>(() => opts.Object);
            underTest.AddOption('o', o => o.Option, "an option");
            underTest.AddOption('f', o => o.Flag, "a flag");
            var result = underTest.BuildParser().Parse(new[] { "foo", "--", "-o", "value", "bar", "-f", "baz" });
            opts.VerifySet(o => o.Option = It.IsAny<string>(), Times.Never);
            opts.VerifySet(o => o.Flag = It.IsAny<bool>(), Times.Never);
            Assert.Equal(new[] { "foo", "-o", "value", "bar", "-f", "baz" }.AsEnumerable(), result.Arguments);
        }

        [Fact]
        public void Parse_PosixOptionOrder_PosixOrderIsRespected()
        {
            var opts = new Mock<Opts>();
            var underTest = new OptionSet<Opts>(() => opts.Object);
            underTest.AddOption('o', o => o.Option, "an option");
            underTest.AddOption('f', o => o.Flag, "a flag");
            var result = underTest.BuildParser().Parse(
                new[] { "-o", "value", "foo", "-f", "bar" }, new ParserConfig { PosixOptionOrder = true });
            opts.VerifySet(o => o.Option = "value", Times.Once);
            opts.VerifySet(o => o.Flag = It.IsAny<bool>(), Times.Never);
            Assert.Equal(new[] { "foo", "-f", "bar" }.AsEnumerable(), result.Arguments);
        }

        [Fact]
        public void Parse_UnknownOption_ReturnsUnknownOptionError()
        {
            var underTest = new OptionSet<Opts>();
            var result = underTest.BuildParser().Parse(new[] { "-u" });
            Assert.Null(result.Options);
            Assert.Null(result.Arguments);
            var err = Assert.IsType<UnknownOptionError>(result.Error);
            Assert.Equal("u", err.OptionName);
        }

        [Fact]
        public void Parse_InvalidOptionValue_ReturnsOptionValueFormatError()
        {
            var underTest = new OptionSet<Opts>();
            underTest.AddOption('o', o => o.Option, s => throw new Exception(), "an option");
            var result = underTest.BuildParser().Parse(new[] { "-ovalue" });
            Assert.Null(result.Options);
            Assert.Null(result.Arguments);
            var err = Assert.IsType<OptionValueFormatError>(result.Error);
            Assert.Equal("o", err.OptionName);
            Assert.Equal("value", err.Value);
        }

        [Fact]
        public void Parse_MissingOptionValue_ReturnsMissingOptionValueError()
        {
            var underTest = new OptionSet<Opts>();
            underTest.AddOption('o', o => o.Option, s => throw new Exception(), "an option");
            var result = underTest.BuildParser().Parse(new[] { "-o" });
            Assert.Null(result.Options);
            Assert.Null(result.Arguments);
            var err = Assert.IsType<MissingOptionValueError>(result.Error);
            Assert.Equal("o", err.OptionName);
        }
    }
}
