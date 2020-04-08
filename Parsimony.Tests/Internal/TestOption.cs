using Moq;
using Parsimony.Internal;
using System;
using Xunit;

namespace Parsimony.Tests.Internal
{
    public class TestOption
    {
        private readonly OptionName.Short _valueShortName =
            OptionName.Parse("v") as OptionName.Short ?? throw new InvalidOperationException();

        private readonly OptionName.Long _valueLongName =
            OptionName.Parse("val") as OptionName.Long ?? throw new InvalidOperationException();

        private readonly Func<string, string> _parseValue = s => s;

        private readonly Action<Opts, string> _assignValue = (opts, v) => opts.Value = v;

        [Fact]
        public void Ctor_NullShortNameAndLongName_Throws() =>
            Assert.Throws<ArgumentException>(() => new Option<Opts, string>(null, null, _parseValue, _assignValue));

        [Fact]
        public void Ctor_NullValueParser_Throws() =>
#nullable disable
            Assert.Throws<ArgumentNullException>(
                "parseValue", () => new Option<Opts, string>(_valueShortName, _valueLongName, null, _assignValue));
#nullable enable

        [Fact]
        public void Ctor_NullAssignment_Throws() =>
#nullable disable
            Assert.Throws<ArgumentNullException>(
                "assignValue", () => new Option<Opts, string>(_valueShortName, _valueLongName, _parseValue, null));
#nullable enable

        [Fact]
        public void ShortName_NotProvided_IsNull()
        {
            var underTest = new Option<Opts, string>(null, _valueLongName, _parseValue, _assignValue);
            Assert.Null(underTest.ShortName);
        }

        [Fact]
        public void ShortName_WasProvided_HasExpectedValue()
        {
            var underTest = new Option<Opts, string>(_valueShortName, null, _parseValue, _assignValue);
            Assert.Equal(_valueShortName, underTest.ShortName);
        }

        [Fact]
        public void ShortName_BothProvided_HasExpectedValue()
        {
            var underTest = new Option<Opts, string>(_valueShortName, _valueLongName, _parseValue, _assignValue);
            Assert.Equal(_valueShortName, underTest.ShortName);
        }

        [Fact]
        public void LongName_NotProvided_IsNull()
        {
            var underTest = new Option<Opts, string>(_valueShortName, null, _parseValue, _assignValue);
            Assert.Null(underTest.LongName);
        }

        [Fact]
        public void LongName_WasProvided_HasExpectedValue()
        {
            var underTest = new Option<Opts, string>(null, _valueLongName, _parseValue, _assignValue);
            Assert.Equal(_valueLongName, underTest.LongName);
        }

        [Fact]
        public void LongName_BothProvided_HasExpectedValue()
        {
            var underTest = new Option<Opts, string>(_valueShortName, _valueLongName, _parseValue, _assignValue);
            Assert.Equal(_valueLongName, underTest.LongName);
        }

        [Fact]
        public void Parse_NullInput_Throws()
        {
            var underTest = new Option<Opts, string>(_valueShortName, null, _parseValue, _assignValue);
#nullable disable
            Assert.Throws<ArgumentNullException>("input", () => underTest.Parse(null));
#nullable enable
        }

        [Fact]
        public void Parse_ValidInput_ParsesValueAndReturnsAssignmentAction()
        {
            // The parse function should use the Option's valueParse function to parse the input parameter then return
            // an Action<TOptions>. When the returned action is invoked it should pass the TOptions it receives and the
            // value returned from the valueParse function to the Option's assignment function.

            var parsedValue = "parsed value";
            var inputValue = "input value";

            var parse = new Mock<Func<string, string>>();
            parse.Setup(p => p.Invoke(It.IsAny<string>())).Returns(parsedValue);

            var assign = new Mock<Action<Opts, string>>();

            var underTest = new Option<Opts, string>(_valueShortName, null, parse.Object, assign.Object);

            var result = underTest.Parse(inputValue);

            parse.Verify(p => p.Invoke(inputValue), Times.Once);
            assign.Verify(a => a.Invoke(It.IsAny<Opts>(), It.IsAny<string>()), Times.Never);

            var opts = new Opts(false, "");
            result(opts);

            assign.Verify(a => a.Invoke(opts, parsedValue), Times.Once);
        }

        [Fact]
        public void Parse_ParseFnThrows_Throws()
        {
            // TODO: Find a better way to communicate the parse error
            var expected = new Exception("junko");
            Func<string, string> throwyParse = s => throw expected;
            var underTest = new Option<Opts, string>(_valueShortName, null, throwyParse, _assignValue);
            var actual = Assert.Throws<Exception>(() => underTest.Parse("something"));
            Assert.Same(expected, actual);
        }
    }
}
