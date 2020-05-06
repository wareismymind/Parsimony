using Parsimony.Options;
using System;
using Xunit;

namespace Parsimony.Tests
{
    public partial class TestOptionSet
    {
        [Fact]
        public void Ctor_NullDefaultNewableType_Constructs()
        {
            var _ = new OptionSet<Opts>();
            _ = new OptionSet<Opts>(null);
        }

        [Fact]
        public void Ctor_NullDefaultNonNewableType_Throws()
        {
            Assert.Throws<InvalidOperationException>(() => new OptionSet<NonNewable>());
            Assert.Throws<InvalidOperationException>(() => new OptionSet<NonNewable>(null));
        }

        [Fact]
        public void Ctor_NonNullDefaultNewableType_Constructs()
        {
            var _ = new OptionSet<Opts>(() => new Opts());
        }

        [Fact]
        public void Ctor_NonNullDefaultNonNewableType_Constructs()
        {
            var _ = new OptionSet<NonNewable>(() => new NonNewable(new object()));
        }
    }
}
