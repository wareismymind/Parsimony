using System;
using Xunit;

namespace Parsimony.Tests
{
   public class TestOptionParseResult
   {
        class TestParseError : OptionParsingError { public TestParseError() : base("It din work") { } }

        [Fact]
        public void CtorError_NullError_Throws()
        {
#nullable disable
            Assert.Throws<ArgumentNullException>("error", () => new OptionParseResult<object>(null));
#nullable enable
        }

        [Fact]
        public void CtorError_NonNullError_Constructs()
        {
            var _ = new OptionParseResult<object>(new TestParseError());
        }

        [Fact]
        public void CtorError_Constructed_HasExpectedValues()
        {
            var error = new TestParseError();
            var underTest = new OptionParseResult<object>(error);
            Assert.Same(error, underTest.Error);
            Assert.Null(underTest.OptionSet);
            Assert.Null(underTest.Arguments);
        }

        [Fact]
        public void CtorNonError_NullOptionSet_Throws()
        {
#nullable disable
            Assert.Throws<ArgumentNullException>(
                "optionSet", () => new OptionParseResult<object>(null, Array.Empty<string>()));
#nullable enable
        }

        [Fact]
        public void CtorNonError_NullArguments_Throws()
        {
#nullable disable
            Assert.Throws<ArgumentNullException>("arguments", () => new OptionParseResult<object>(new object(), null));
#nullable enable
        }

        [Fact]
        public void CtorNonError_ValidArgs_Constructs()
        {
            var _ = new OptionParseResult<object>(new object(), Array.Empty<string>());
        }

        [Fact]
        public void CtorNonError_Constructed_HasExpectedValues()
        {
            var optionSet = new object();
            var arguments = new[] { "foo", "bar", "baz" };
            var underTest = new OptionParseResult<object>(optionSet, arguments);
            Assert.Null(underTest.Error);
            Assert.Same(optionSet, underTest.OptionSet);
            Assert.Equal(arguments, underTest.Arguments);
        }
    }
}
