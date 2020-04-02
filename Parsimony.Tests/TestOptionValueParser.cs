using Moq;
using System;
using System.Linq;
using Xunit;

namespace Parsimony.Tests
{
    public class TestOptionValueParser
    {
        // This class is public to allow Moqing an Action<Options, TValue>.
        public class Options
        {
            public bool Foo { get; set; }
            public string? Bar { get; set; }
            public int Baz { get; set; }
        }

        [Fact]
        public void Ctor_NullParse_Throws()
        {
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
            Assert.Throws<ArgumentNullException>(
                "parse",
                () => new OptionValueParser<Options, int>(null, (opts, v) => opts.Baz = v));
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
        }

        [Fact]
        public void Ctor_NullAssign_Throws()
        {

#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
            Assert.Throws<ArgumentNullException>(
                "assign",
                () => new OptionValueParser<Options, int>(int.Parse, null));
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
        }

        [Fact]
        public void Ctor_NonNullParseAndAssign_Constructs()
        {
            var _ = new OptionValueParser<Options, int>(int.Parse, (opts, v) => opts.Baz = v);
        }

        [Fact]
        public void Parse_NullState_Throws()
        {
            var underTest = new OptionValueParser<Options, string>(s => s, (opts, v) => opts.Bar = v);
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
            Assert.Throws<ArgumentNullException>("state", () => underTest.Parse(null));
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
        }

        [Fact]
        public void Parse_EmptyInput_Throws()
        {
            var result = new ParseResult<Options>(new Options(), Array.Empty<string>());
            var state = new ParserState<Options>(result, Array.Empty<string>());
            var underTest = new OptionValueParser<Options, string>(s => s, (opts, v) => opts.Bar = v);
            // TODO: Test custom exception
            Assert.Throws<Exception>(() => underTest.Parse(state));
        }

        [Fact]
        public void Parse_NonEmptyInput_FirstTokenFromInputIsParsed()
        {
            var result = new ParseResult<Options>(new Options(), Array.Empty<string>());
            var input = new[] { "shake", "rattle", "roll" };
            var state = new ParserState<Options>(result, input);
            var parse = new Mock<Func<string, int>>();
            parse.Setup(p => p.Invoke(It.IsAny<string>())).Returns(17);
            var underTest = new OptionValueParser<Options, int>(parse.Object, (opts, v) => { });
            var _ = underTest.Parse(state);
            parse.Verify(p => p.Invoke("shake"), Times.Once);
        }

        [Fact]
        public void Parse_NonEmptyInput_FirstTokenFromInputIsAssignedToOption()
        {
            var options = new Options();
            var result = new ParseResult<Options>(options, Array.Empty<string>());
            var input = new[] { "shake", "rattle", "roll" };
            var state = new ParserState<Options>(result, input);
            var assign = new Mock<Action<Options, int>>();
            var underTest = new OptionValueParser<Options, int>(s => 17, assign.Object);
            var _ = underTest.Parse(state);
            assign.Verify(p => p.Invoke(options, 17), Times.Once);
        }

        [Fact]
        public void Parse_NonEmptyInput_FirstTokenIsConsumed()
        {
            var result = new ParseResult<Options>(new Options(), Array.Empty<string>());
            var input = new[] { "shake", "rattle", "roll" };
            var state = new ParserState<Options>(result, input);
            var underTest = new OptionValueParser<Options, int>(s => 17, (opts, v) => { });
            var newState = underTest.Parse(state);
            Assert.Equal(new[] { "rattle", "roll" }.AsEnumerable(), newState.Input);
        }

        // TODO: Tests for when parse/assign throw
    }
}
