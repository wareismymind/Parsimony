using Moq;
using System;
using Xunit;

namespace Parsimony.Tests
{
    public class TestOption_Parse
    {
        [Fact]
        public void Parse_NullContext_Throws()
        {
            var parse = new Mock<Func<string, bool>>();
            var assign = new Mock<Action<Opts, bool>>();
            var underTest = new Option<Opts, bool>(new OptionName('f', "foo"), parse.Object, assign.Object);

#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
            Assert.Throws<ArgumentNullException>("context", () => underTest.Parse(null));
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
        }

        [Fact]
        public void Parse_StandaloneShortNameFlag_ParsesAndAssigns()
        {
            var result = ParseTest<Opts, bool>("true", true, "-f");
            Assert.Empty(result.Input);
        }

        private static ParseContext<Opts> ParseTest<TOption, TValue>(
            string value,
            TValue parsedValue,
            params string[] input)
        {
            // This test expects that when an Option successfully parses the given context's input it will use the
            // Func<string, TValue> to parse the option's value and append an assignment to the context that passes the
            // value returned by the parse Func to the Action<TOptions, TValue>.

            var parse = new Mock<Func<string, TValue>>();
            parse.Setup(p => p.Invoke(It.IsAny<string>())).Returns(parsedValue);

            var assign = new Mock<Action<Opts, TValue>>();

            var context = new ParseContext<Opts>(Array.Empty<Action<Opts>>(), Array.Empty<string>(), input);

            var underTest = new Option<Opts, TValue>(new OptionName('f', "foo"), parse.Object, assign.Object);

            var result = underTest.Parse(context);

            Assert.Null(result.Error);

            parse.Verify(p => p.Invoke(value), Times.Once);

            Assert.Equal(1, result.Context.Assignments.Count);
            assign.Verify(a => a.Invoke(It.IsAny<Opts>(), It.IsAny<TValue>()), Times.Never);
            var opts = new Opts(false, "");
            var assignment = result.Context.Assignments[0];
            assignment.Invoke(opts);
            assign.Verify(a => a.Invoke(opts, parsedValue), Times.Once);
            return result.Context;
        }

        //[Fact]
        //public void Parse_StandaloneLongNameFlag_ReturnsTrue()
        //{
        //    var context = new ParseContext<Options>(_assignments, _emptyStrings, new[] { "--foo" });
        //    var underTest = new Option<Options, bool>(new OptionName('f', "foo"));
        //    Assert.True(underTest.CanParse(context));
        //}

        //[Fact]
        //public void Parse_AdjoinedShortNameFlag_ReturnsTrue()
        //{
        //    var context = new ParseContext<Options>(_assignments, _emptyStrings, new[] { "-fgh" });
        //    var underTest = new Option<Options, bool>(new OptionName('f', "foo"));
        //    Assert.True(underTest.CanParse(context));
        //}

        //[Fact]
        //public void Parse_SpaceSeparatedShortNameOptionAndValue_ReturnsTrue()
        //{
        //    var context = new ParseContext<Options>(_assignments, _emptyStrings, new[] { "-f", "gh" });
        //    var underTest = new Option<Options, string>(new OptionName('f', "foo"));
        //    Assert.True(underTest.CanParse(context));
        //}

        //[Fact]
        //public void Parse_SpaceSeparatedLongNameOptionAndValue_ReturnsTrue()
        //{
        //    var context = new ParseContext<Options>(_assignments, _emptyStrings, new[] { "--foo", "gh" });
        //    var underTest = new Option<Options, string>(new OptionName('f', "foo"));
        //    Assert.True(underTest.CanParse(context));
        //}

        //[Fact]
        //public void Parse_AdjoinedShortNameOptionAndValue_ReturnsTrue()
        //{
        //    var context = new ParseContext<Options>(_assignments, _emptyStrings, new[] { "-fgh" });
        //    var underTest = new Option<Options, string>(new OptionName('f', "foo"));
        //    Assert.True(underTest.CanParse(context));
        //}

        //[Fact]
        //public void Parse_EqualsJoinedLongNameFlag_ReturnsTrue()
        //{
        //    var context = new ParseContext<Options>(_assignments, _emptyStrings, new[] { "--foo=yes" });
        //    var underTest = new Option<Options, bool>(new OptionName('f', "foo"));
        //    Assert.True(underTest.CanParse(context));
        //}
    }
}
