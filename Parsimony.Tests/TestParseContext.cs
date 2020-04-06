using System;
using System.Linq;
using Xunit;

namespace Parsimony.Tests
{
    public class TestParseContext
    {
        private readonly Action<Opts>[] _assignments = Array.Empty<Action<Opts>>();
        private readonly string[] _empty = Array.Empty<string>();

        [Fact]
        public void Ctor_NullOptions_Throws()
        {
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
            Assert.Throws<ArgumentNullException>("assignments", () => new ParseContext<Opts>(null, _empty, _empty));
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
        }

        [Fact]
        public void Ctor_NullArguments_Throws()
        {
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
            Assert.Throws<ArgumentNullException>(
                "arguments", () => new ParseContext<Opts>(_assignments, null, _empty));
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
        }

        [Fact]
        public void Ctor_NullInput_Throws()
        {
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
            Assert.Throws<ArgumentNullException>("input", () => new ParseContext<Opts>(_assignments, _empty, null));
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
        }

        [Fact]
        public void Ctor_ValidArgs_Constructs()
        {
            var _ = new ParseContext<Opts>(_assignments, _empty, _empty);
        }

        [Fact]
        public void Assignments_Constructed_HasExpectedValue()
        {
            var expected = new Action<Opts>[] { o => o.Flag = true, o => o.Value = "blarg" };
            var underTest = new ParseContext<Opts>(expected, _empty, _empty);
            Assert.Equal(expected, underTest.Assignments);
        }

        [Fact]
        public void Arguments_Constructed_HasExpectedValue()
        {
            var expected = new[] { "harpo", "groucho", "chico" };
            var underTest = new ParseContext<Opts>(_assignments, expected, _empty);
            Assert.Equal(expected.AsEnumerable(), underTest.Arguments);
        }

        [Fact]
        public void Input_Constructed_HasExpectedValue()
        {
            var expected = new[] { "harpo", "groucho", "chico", "zeppo" };
            var underTest = new ParseContext<Opts>(_assignments, _empty, expected);
            Assert.Equal(expected.AsEnumerable(), underTest.Input);
        }
    }
}
