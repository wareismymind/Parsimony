using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Parsimony.Tests
{
    public class TestRemainingArgumentsParser
    {
        private class Options
        {
            public bool Foo { get; set; }
            public string? Bar { get; set; }
            public int Baz { get; set; }
        }

        [Fact]
        public void Parse_NullState_Throws()
        {
            var underTest = new RemainingArgumentsParser<Options>();
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
            Assert.Throws<ArgumentNullException>("state", () => underTest.Parse(null));
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
        }

        [Fact]
        public void Parse_EmptyInput_ChangesNothing()
        {
            var options = new Options { Foo = true, Bar = "nana", Baz = 17 };
            var arguments = new[] { "foo", "bar" };
            var result = new ParseResult<Options>(options, arguments);
            var state = new ParserState<Options>(result, Array.Empty<string>());
            var underTest = new RemainingArgumentsParser<Options>();
            var newState = underTest.Parse(state);
            Assert.Equal(state.Result.Options, newState.Result.Options);
            Assert.Equal(state.Result.Arguments, newState.Result.Arguments);
            Assert.Equal(state.Input, newState.Input);
        }

        [Fact]
        public void Parse_NonEmptyInput_InputIsAppendedToArgumentsAndEmptied()
        {
            var options = new Options { Foo = true, Bar = "nana", Baz = 17 };
            var arguments = new[] { "foo", "bar" };
            var result = new ParseResult<Options>(options, arguments);
            var input = new[] { "shake", "rattle", "roll" };
            var state = new ParserState<Options>(result, input);
            var underTest = new RemainingArgumentsParser<Options>();
            var newState = underTest.Parse(state);
            var expectedArguments = new[] { "foo", "bar", "shake", "rattle", "roll" };
            Assert.Equal(state.Result.Options, newState.Result.Options);
            Assert.Equal(expectedArguments.AsEnumerable(), newState.Result.Arguments);
            Assert.Equal(Array.Empty<string>(), newState.Input);
        }
    }
}
