using System;
using System.Collections.Generic;
using Xunit;

namespace Parsimony.Tests
{
    public class TestParseResult
    {
        [Fact]
        public void Ctor_NullOptions_Throws()
        {
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
            Assert.Throws<ArgumentNullException>(
                "options",
                () => new ParseResult<object>(null, Array.Empty<string>()));
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
        }

        [Fact]
        public void Ctor_NullArguments_Throws()
        {
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
            Assert.Throws<ArgumentNullException>(
                "arguments",
                () => new ParseResult<object>(new object(), null));
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
        }

        [Fact]
        public void Ctor_NonNullOptionsAndArguments_Constructs()
        {
            var _ = new ParseResult<object>(new object(), Array.Empty<string>());
        }

        [Fact]
        public void Options_Constructed_HasExpectedValue()
        {
            var options = new object();
            var underTest = new ParseResult<object>(options, Array.Empty<string>());
            Assert.Same(options, underTest.Options);
        }

        [Fact]
        public void Arguments_Constructed_HasExpectedValue()
        {
            var arguments = Array.Empty<string>();
            var underTest = new ParseResult<object>(new object(), arguments);
            Assert.Same(arguments, underTest.Arguments);
        }
    }
}
