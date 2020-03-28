using Moq;
using System;
using Xunit;

namespace Parsimony.Tests
{
    public class TestOptionSpec
    {
        [Fact]
        public void CtorRequiresLongNameOrShortNameOrBoth()
        {
            static OptionSpec<object, int> ctor(char? shortName, string? longName) =>
                new OptionSpec<object, int>(
                    shortName: shortName,
                    longName: longName,
                    required: false,
                    @default: 3,
                    setFn: (opts, v) => { },
                    parseFn: int.Parse);

            Assert.Throws<ArgumentException>(() => ctor(null, null));

            ctor('r', null);
            ctor(null, "retry");
            ctor('r', "retry");
        }

        [Fact]
        public void CtorRequiresParseFn()
        {
            static OptionSpec<object, int> ctor(Func<string, int> parseFn) =>
                new OptionSpec<object, int>(
                    shortName: 'r',
                    longName: "retry",
                    required: false,
                    @default: 3,
                    setFn: (opts, v) => { },
                    parseFn: parseFn);

#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
            Assert.Throws<ArgumentNullException>("parseFn", () => ctor(null));
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.

            ctor(int.Parse);
        }

        [Fact]
        public void CtorRequiresSetFn()
        {
            static OptionSpec<object, int> ctor(Action<object, int> setFn) =>
                new OptionSpec<object, int>(
                    shortName: 'r',
                    longName: "retry",
                    required: false,
                    @default: 3,
                    setFn: setFn,
                    parseFn: int.Parse);

#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
            Assert.Throws<ArgumentNullException>("setFn", () => ctor(null));
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.

            ctor((opts, v) => { });
        }

        [Fact]
        public void OptionTypeReturnsTValue()
        {
            var underTest = new OptionSpec<object, int>(
                shortName: 'r',
                longName: "retry",
                required: false,
                @default: 3,
                setFn: (opts, v) => { },
                parseFn: int.Parse) as OptionSpec<object>;

            Assert.Equal(typeof(int), underTest.OptionType);
        }

        [Fact]
        public void ParseCallsParseFn()
        {
            var expected = new object();

            var parseFn = new Mock<Func<string, object>>();
            parseFn.Setup(x => x.Invoke(It.IsAny<string>())).Returns(expected);

            var underTest = new OptionSpec<object, object>(
                shortName: 'r',
                longName: "retry",
                required: false,
                @default: 3,
                setFn: (opts, v) => { },
                parseFn: parseFn.Object);

            var input = "a specific string";

            var actual = underTest.Parse(input);

            parseFn.Verify(x => x.Invoke(input), Times.Once);
            Assert.Same(expected, actual);
        }

        [Fact]
        public void SetCallsSetFn()
        {
            var options = new object();
            var value = new object();

            var setFn = new Mock<Action<object, object>>();

            var underTest = new OptionSpec<object, object>(
                shortName: 'r',
                longName: "retry",
                required: false,
                @default: 3,
                setFn: setFn.Object,
                parseFn: (str) => new object());

            underTest.Set(options, value);

            setFn.Verify(x => x.Invoke(options, value), Times.Once);
        }
    }
}
