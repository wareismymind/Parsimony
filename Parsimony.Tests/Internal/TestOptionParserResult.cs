using Moq;
using Parsimony.Errors;
using Parsimony.Internal;
using System;
using System.Collections.Generic;
using Xunit;

namespace Parsimony.Tests.Options
{
   public class TestOptionParserResult
   {
        private static readonly OptionName _optionName =
            OptionName.Parse("option") ??
                throw new InvalidOperationException($"Invalid option name for {nameof(_optionName)}.");

        class TestParseError : OptionParsingError { public TestParseError() : base(_optionName) { } }

        [Fact]
        public void CtorError_NullError_Throws()
        {
#nullable disable
            Assert.Throws<ArgumentNullException>("error", () => new OptionParserResult<object>(null));
#nullable enable
        }

        [Fact]
        public void CtorError_NonNullError_Constructs()
        {
            var _ = new OptionParserResult<object>(new TestParseError());
        }

        [Fact]
        public void CtorError_Constructed_HasExpectedValues()
        {
            var error = new TestParseError();
            var underTest = new OptionParserResult<object>(error);
            Assert.Same(error, underTest.Error);
            Assert.Null(underTest.Assignments);
            Assert.Null(underTest.Arguments);
        }

        [Fact]
        public void CtorNonError_NullOptionSet_Throws()
        {
#nullable disable
            Assert.Throws<ArgumentNullException>(
                "assignments", () => new OptionParserResult<object>(null, Array.Empty<string>()));
#nullable enable
        }

        [Fact]
        public void CtorNonError_NullArguments_Throws()
        {
#nullable disable
            Assert.Throws<ArgumentNullException>("arguments", () =>
                new OptionParserResult<object>(new Dictionary<IOption<object>, Action<object>>(), null));
#nullable enable
        }

        [Fact]
        public void CtorNonError_ValidArgs_Constructs()
        {
            var _ = new OptionParserResult<object>(
                new Dictionary<IOption<object>, Action<object>>(), Array.Empty<string>());
        }

        [Fact]
        public void CtorNonError_Constructed_HasExpectedValues()
        {
            var optionSet = new Dictionary<IOption<object>, Action<object>>
            {
                { new Mock<IOption<object>>().Object, x => { } },
                { new Mock<IOption<object>>().Object, x => { } },
            };
            var arguments = new[] { "foo", "bar", "baz" };
            var underTest = new OptionParserResult<object>(optionSet, arguments);
            Assert.Null(underTest.Error);
            Assert.Equal(optionSet, underTest.Assignments);
            Assert.Equal(arguments, underTest.Arguments);
        }
    }
}
