using Parsimony.Internal;
using System;
using Xunit;

namespace Parsimony.Tests.Internal
{
    public class TestDependency
    {

#nullable disable
        [Fact]
        public void Ctor_NullDependent_Throws()
        {
            Assert.Throws<ArgumentNullException>("dependent", () => new Dependency<string>(null, "b"));
        }

        [Fact]
        public void Ctor_NullDependee_Throws()
        {
            Assert.Throws<ArgumentNullException>("dependee", () => new Dependency<string>("a", null));
        }
#nullable enable

        [Fact]
        public void Ctor_DependeeEqualsDependent_Throws()
        {
            Assert.Throws<ArgumentException>("dependee", () => new Dependency<string>("same" ,"same"));
        }

        [Fact]
        public void Dependency_Constructed_HasExpectedValues()
        {
            var underTest = new Dependency<string>("a", "b");
            Assert.Equal("a", underTest.Dependent);
            Assert.Equal("b", underTest.Dependee);
        }

        [Fact]
        public void Equals_SameValues_ReturnsTrue()
        {
            var a = new Dependency<string>("a", "b");
            var b = new Dependency<string>("a", "b");

            Assert.Equal(a, b);
            Assert.True(a == b);
            Assert.False(a != b);
            Assert.True(a.Equals(b));
        }

        [Fact]
        public void Equals_DifferentValues_ReturnsFalse()
        {
            var a = new Dependency<string>("a", "b");
            var b = new Dependency<string>("a", "c");

            Assert.NotEqual(a, b);
            Assert.False(a == b);
            Assert.True(a != b);
            Assert.False(a.Equals(b));
        }
    }
}
