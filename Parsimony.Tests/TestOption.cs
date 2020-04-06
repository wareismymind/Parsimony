using System;
using Xunit;

namespace Parsimony.Tests
{
    public class TestOption
    {
        private const char _shortName = 'f';
        private const string _longName = "foo";
        private readonly OptionName _name = new OptionName(_shortName, _longName);
        private readonly Func<string, bool> _parseFlag = s => false;
        private readonly Action<Opts, bool> _assignFlag = (o, v) => { };

        [Fact]
        public void Ctor_NullName_Throws()
        {
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
            Assert.Throws<ArgumentNullException>(
                "name", () => _ = new Option<Opts, bool>(null, _parseFlag, _assignFlag));
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
        }

        [Fact]
        public void Ctor_NullValueParser_Throws()
        {
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
            Assert.Throws<ArgumentNullException>(
                "valueParser", () => _ = new Option<Opts, bool>(_name, null, _assignFlag));
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
        }

        [Fact]
        public void Ctor_Assignment_Throws()
        {
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
            Assert.Throws<ArgumentNullException>(
                "assignment", () => _ = new Option<Opts, bool>(_name, _parseFlag, null));
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
        }

        [Fact]
        public void Ctor_ValidArgs_Constructs()
        {
            var _ = new Option<Opts, bool>(_name, _parseFlag, _assignFlag);
        }

        [Fact]
        public void Name_Constructed_HasExpectedValue()
        {
            var underTest = new Option<Opts, bool>(_name, _parseFlag, _assignFlag);
            Assert.Equal(_name.ShortName, underTest.Name.ShortName);
            Assert.Equal(_name.LongName, underTest.Name.LongName);
        }
    }
}
