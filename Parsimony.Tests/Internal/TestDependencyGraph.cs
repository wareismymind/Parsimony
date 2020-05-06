using Parsimony.Internal;
using System;
using System.Linq;
using Xunit;

namespace Parsimony.Tests.Internal
{
    public class TestDependencyGraph
    {
        [Fact]
        public void Add_NewDependency_ReturnsTrue()
        {
            var underTest = new DependencyGraph<string>();
            Assert.True(underTest.Add(new Dependency<string>("aaa", "aaaaa")));
        }

        [Fact]
        public void Add_ExistingDependency_ReturnsReturnsFalse()
        {
            var underTest = new DependencyGraph<string>();
            var _ = underTest.Add(new Dependency<string>("aaa", "aaaaa"));
            Assert.False(underTest.Add(new Dependency<string>("aaa", "aaaaa")));
        }

        [Fact]
        public void Enumerable_New_IsEmpty()
        {
            var underTest = new DependencyGraph<string>();
            Assert.Equal(Array.Empty<Dependency<string>>(), underTest);
        }

        [Fact]
        public void Enumerable_AddedDependencies_IncludesAddedDependencies()
        {
            var depA = new Dependency<string>("aaa", "aaaaa");
            var depB = new Dependency<string>("bbb", "bbbbb");
            var underTest = new DependencyGraph<string>();
            underTest.Add(depA);
            underTest.Add(depB);

            Assert.Contains(depA, underTest);
            Assert.Contains(depB, underTest);
        }

        [Fact]
        public void Enumerable_AddedDuplicateDependencies_DoesNotIncludeDuplicates()
        {
            var depA = new Dependency<string>("aaa", "aaaaa");
            var depB = new Dependency<string>("bbb", "bbbbb");
            var underTest = new DependencyGraph<string>();
            underTest.Add(depA);
            underTest.Add(depB);
            underTest.Add(depB);
            underTest.Add(depB);

            Assert.Equal(2, underTest.Count());
            Assert.Contains(depA, underTest);
            Assert.Contains(depB, underTest);
        }

        [Fact]
        public void Enumerable_ImpliedDependenciesExist_IncludesImpliedDependencies()
        {
            var adds = new[] {
                new Dependency<string>("aaa", "bbb"),
                new Dependency<string>("bbb", "ccc"),
            };

            var implied = new[] { new Dependency<string>("aaa", "ccc") };

            Test(adds, implied);
        }

        [Fact]
        public void Enumerable_AncestralImpliedDependenciesExist_IncludesImpliedDependencies()
        {
            var adds = new[]
            {
                new Dependency<string>("aaa", "bbb"),
                new Dependency<string>("bbb", "ccc"),
                new Dependency<string>("eee", "fff"),
                new Dependency<string>("ddd", "eee"),
                new Dependency<string>("ccc", "ddd"),
            };

            var implied = new[] {
                new Dependency<string>("aaa", "ccc"),
                new Dependency<string>("aaa", "ddd"),
                new Dependency<string>("aaa", "eee"),
                new Dependency<string>("aaa", "fff"),

                new Dependency<string>("bbb", "ddd"),
                new Dependency<string>("bbb", "eee"),
                new Dependency<string>("bbb", "fff"),

                new Dependency<string>("ccc", "eee"),
                new Dependency<string>("ccc", "fff"),

                new Dependency<string>("ddd", "fff"),
            };

            Test(adds, implied);
        }

        [Fact]
        public void Enumerable_CyclicDependency_IncludesCycle()
        {
            var adds = new[]
            {
                new Dependency<string>("aaa", "bbb"),
                new Dependency<string>("bbb", "ccc"),
                new Dependency<string>("ccc", "bbb"),
            };

            var implied = new[] {
                new Dependency<string>("aaa", "ccc"),
            };

            Test(adds, implied);
        }

        private void Test<T>(Dependency<T>[] adds, Dependency<T>[] implied) where T : class, IEquatable<T>
        {
            var underTest = new DependencyGraph<T>();
            foreach (var add in adds)
            {
                underTest.Add(add);
            }

            var graph = underTest.ToList();
            Assert.Equal(adds.Length + implied.Length, graph.Count);
            foreach (var add in adds)
            {
                Assert.Contains(add, graph);
            }
            foreach (var implication in implied)
            {
                Assert.Contains(implication, graph);
            }
        }
    }
}
