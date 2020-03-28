using System;
using System.Collections.Generic;
using Xunit;

namespace Parsimony.Tests
{
    public partial class TestParser
    {
        private class Options
        {
            public bool Foo { get; set; }

            public string? Bar { get; set; }

            public int Baz { get; set; }
        }

        private OptionSpec<Options>[] _standardOptionSpecs = new OptionSpec<Options>[]
        {
            new OptionSpec<Options, bool>(
            shortName: 'f',
            longName: null,
            required: false,
            @default: default,
            parseFn: bool.Parse, // Needs to be non-null, but not used for shortname flags
            setFn: (opts, v) => opts.Foo = v),

            new OptionSpec<Options, string?>(
            shortName: 'b',
            longName: null,
            required: false,
            @default: default,
            parseFn: (s) => s,
            setFn: (opts, v) => opts.Bar = v),

            new OptionSpec<Options, int>(
            shortName: 'z',
            longName: null,
            required: false,
            @default: default,
            parseFn: int.Parse,
            setFn: (opts, v) => opts.Baz = v)
        };

        [Fact]
        public void CtorRequiresOptionSpecs()
        {
            static Parser<Options> ctor(IEnumerable<OptionSpec<Options>> specs) =>
                    new Parser<Options>(specs, new ParserConfig());

#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
            var ex = Assert.Throws<ArgumentNullException>("optionSpecs", () => ctor(null));
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.

            ctor(Array.Empty<OptionSpec<Options>>());
        }

        [Fact]
        public void CtorRequiresConfiguration()
        {
            static Parser<Options> ctor(ParserConfig config) =>
                    new Parser<Options>(Array.Empty<OptionSpec<Options>>(), config);

#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
            var ex = Assert.Throws<ArgumentNullException>("configuration", () => ctor(null));
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.

            ctor(new ParserConfig());
        }

        [Fact]
        public void ParseRequiresInput()
        {
            var underTest = new Parser<Options>(Array.Empty<OptionSpec<Options>>(), new ParserConfig());

#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
            Assert.Throws<ArgumentNullException>("input", () => underTest.Parse(null));
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.

            underTest.Parse(Array.Empty<string>());
        }
    }
}
