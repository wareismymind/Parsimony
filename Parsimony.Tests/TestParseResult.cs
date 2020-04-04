using System;
using Xunit;

namespace Parsimony.Tests
{
    public class TestParseResult
    {
        private readonly ParseContext<Options> _context =
            new ParseContext<Options>(new Options(true, "foo"), new[] { "bar", "baz" }, new[] { "harpo", "groucho" });

        [Fact]
        public void CtorContext_NullContext_Throws()
        {
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
            Assert.Throws<ArgumentNullException>("context", () => new ParseResult<Options, string>(null));
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
        }

        [Fact]
        public void CtorContext_ValidArgs_Constructs()
        {
            var _ = new ParseResult<Options, string>(_context);
        }

        [Fact]
        public void CtorContextError_NullContext_Throws()
        {
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
            Assert.Throws<ArgumentNullException>(
                "context", () => new ParseResult<Options, string>(null, "it din work"));
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
        }

        [Fact]
        public void CtorContextError_NulError_Throws()
        {
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
            Assert.Throws<ArgumentNullException>("error", () => new ParseResult<Options, string>(_context, null));
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
        }

        [Fact]
        public void CtorContextError_ValidArgs_Constructs()
        {
            var _ = new ParseResult<Options, string>(_context, "it din work");
        }

        [Fact]
        public void Context_ConstructedWithoutError_HasExpectedValue()
        {
            var underTest = new ParseResult<Options, string>(_context);
            Assert.Equal(_context, underTest.Context);
        }
        [Fact]
        public void Context_ConstructedWithError_HasExpectedValue()
        {
            var underTest = new ParseResult<Options, string>(_context, "it din work");
            Assert.Equal(_context, underTest.Context);
        }

        [Fact]
        public void Error_ConstructedWithoutError_IsNull()
        {
            var underTest = new ParseResult<Options, string>(_context);
            Assert.Null(underTest.Error);
        }

        [Fact]
        public void Error_ConstructedWithError_HasExpectedValue()
        {
            var underTest = new ParseResult<Options, string>(_context, "it din work");
            Assert.Equal("it din work", underTest.Error);
        }

        class Options
        {
            public bool Flag { get; private set; }

            public string Value { get; private set; }

            public Options(bool flag, string value)
            {
                Flag = flag;
                Value = value ?? throw new ArgumentNullException(nameof(value));
            }
        }
    }
}
