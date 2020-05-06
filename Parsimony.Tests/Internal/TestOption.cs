using Moq;
using Parsimony.Internal;
using System;
using Xunit;

namespace Parsimony.Tests.Internal
{
    public class TestOption
    {
        private static readonly OptionName.Short _valueShortName =
            OptionName.Parse("v") as OptionName.Short ??
                throw new InvalidOperationException($"Invalid option name for {nameof(_valueShortName)}.");

        private static readonly OptionName.Long _valueLongName =
            OptionName.Parse("val") as OptionName.Long ??
                throw new InvalidOperationException($"Invalid option name for {nameof(_valueLongName)}.");

        private readonly Func<string, string> _parseValue = s => s;

        private readonly Action<Opts, string> _assignValue = (opts, v) => opts.Value = v;

        [Fact]
        public void Ctor_NullShortNameAndLongName_Throws()
        {
            Assert.Throws<ArgumentException>(() => new Option<Opts, string>(null, null, _parseValue, _assignValue));
        }

#nullable disable
        [Fact]
        public void Ctor_NullValueParser_Throws()
        {
            Assert.Throws<ArgumentNullException>(
                "parseValue", () => new Option<Opts, string>(_valueShortName, _valueLongName, null, _assignValue));
        }

        [Fact]
        public void Ctor_NullAssignment_Throws()
        {
            Assert.Throws<ArgumentNullException>(
                "assignValue", () => new Option<Opts, string>(_valueShortName, _valueLongName, _parseValue, null));
        }
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
        public void GetAssignment_NullInput_Throws()
        {
            var underTest = new Option<Opts, string>(_valueShortName, null, _parseValue, _assignValue);
#nullable disable
            Assert.Throws<ArgumentNullException>("input", () => underTest.GetAssignment(null));
#nullable enable
        }

        [Fact]
        public void GetAssignment_ValidInput_ParsesValueAndReturnsAssignmentAction()
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

            var (assignment, error) = underTest.GetAssignment(inputValue);

            Assert.NotNull(assignment);
            Assert.Null(error);

            parse.Verify(p => p.Invoke(inputValue), Times.Once);
            assign.Verify(a => a.Invoke(It.IsAny<Opts>(), It.IsAny<string>()), Times.Never);

            var opts = new Opts();

#pragma warning disable CS8602 // Dereference of a possibly null reference.
            assignment(opts);
#pragma warning restore CS8602

            assign.Verify(a => a.Invoke(opts, parsedValue), Times.Once);
        }

        [Fact]
        public void GetAssignment_ParseFnThrows_ReturnsFormatError()
        {
            var exception = new Exception("junko");
            string throwyParse(string s) => throw exception;
            var underTest = new Option<Opts, string>(_valueShortName, null, throwyParse, _assignValue);
            var input = "something";
            var (assignment, error) = underTest.GetAssignment(input);
            Assert.Null(assignment);
#pragma warning disable CS8602 // Dereference of a possibly null reference.
            Assert.Equal(_valueShortName, error.OptionName);
            Assert.Equal(input, error.Value);
            Assert.Equal(exception.Message, error.Message);
#pragma warning restore CS8602
        }

        internal class Opts
        {
            public string? Value { get; set; }
        }
    }
}
