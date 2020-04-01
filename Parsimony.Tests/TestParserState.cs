using System;
using Xunit;

namespace Parsimony.Tests
{
    public class TestParserState
    {
        [Fact]
        public void Ctor_NullResult_Throws()
        {
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
            Assert.Throws<ArgumentNullException>(
                "result",
                () => new ParserState<object>(null, Array.Empty<string>()));
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
        }

        [Fact]
        public void Ctor_NullInput_Throws()
        {
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
            Assert.Throws<ArgumentNullException>(
                "input",
                () => new ParserState<object>(GetParseResult(), null));
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
        }

        [Fact]
        public void Ctor_NonNullResultAndInput_Constructs()
        {
            var _ = new ParserState<object>(GetParseResult(), Array.Empty<string>());
        }

        [Fact]
        public void Result_Constructed_HasExpectedValue()
        {
            var result = GetParseResult();
            var underTest = new ParserState<object>(result, Array.Empty<string>());
            Assert.Same(result, underTest.Result);
        }

        [Fact]
        public void Input_Constructed_HasExpectedValue()
        {
            var input = Array.Empty<string>();
            var underTest = new ParserState<object>(GetParseResult(), input);
            Assert.Same(input, underTest.Input);
        }

        private ParseResult<object> GetParseResult() =>
            new ParseResult<object>(new object(), Array.Empty<string>());
    }
}
