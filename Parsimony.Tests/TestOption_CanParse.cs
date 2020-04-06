using System;
using Xunit;

namespace Parsimony.Tests
{
    public partial class TestOption_CanParse
    {
        [Fact]
        public void CanParse_StandaloneShortNameFlag_ReturnsTrue() => TestFlag("-f");

        [Fact]
        public void CanParse_StandaloneLongNameFlag_ReturnsTrue() => TestFlag("--foo");

        [Fact]
        public void CanParse_AdjoinedShortNameFlag_ReturnsTrue() => TestFlag("-fgh");

        [Fact]
        public void CanParse_SpaceSeparatedShortNameOptionAndValue_ReturnsTrue() => TestValueOption("-v", "gh");

        [Fact]
        public void CanParse_SpaceSeparatedLongNameOptionAndValue_ReturnsTrue() => TestValueOption("--val", "gh");

        [Fact]
        public void CanParse_AdjoinedShortNameOptionAndValue_ReturnsTrue() => TestValueOption("-vgh");

        [Fact]
        public void CanParse_EqualsJoinedLongNameFlag_ReturnsTrue() => TestFlag("--foo=yes");

        [Fact]
        public void CanParse_EqualsJoinedLongNameOptionAndVale_ReturnsTrue() => TestValueOption("--val=no");

        private static void TestFlag(params string[] input) => Assert.True(GetFlag().CanParse(NewContext(input)));

        private static void TestValueOption(params string[] input) =>
            Assert.True(GetValueOption().CanParse(NewContext(input)));

        private static ParseContext<Opts> NewContext(params string[] input) =>
            new ParseContext<Opts>(Array.Empty<Action<Opts>>(), Array.Empty<string>(), input);

        private static Option<Opts, bool> GetFlag() =>
            new Option<Opts, bool>(new OptionName('f', "foo"), bool.Parse, (opts, f) => opts.Flag = f);

        private static Option<Opts, string> GetValueOption() =>
            new Option<Opts, string>(new OptionName('v', "val"), s => s, (opts, v) => opts.Value = v);
    }
}
