using System;
using System.Collections.Generic;
using Xunit;

namespace Parsimony.Tests
{
    public class TestParseResult
    {
        [Fact]
        public void CtorRequiresResult()
        {
            static ParseResult<object> ctor(object result) =>
               new ParseResult<object>(result, new[] { "foo", "bar", "baz" });

#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
            Assert.Throws<ArgumentNullException>("options", () => ctor(null));
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.

            ctor(new object());
        }

        [Fact]
        public void CtorRequiresArguments()
        {
            static ParseResult<object> ctor(IEnumerable<string> arguments) =>
               new ParseResult<object>(new object(), arguments);

#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
            Assert.Throws<ArgumentNullException>("arguments", () => ctor(null));
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.

            ctor(Array.Empty<string>());
        }
    }
}
