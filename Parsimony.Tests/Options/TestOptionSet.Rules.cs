using Moq;
using Parsimony.Errors;
using Parsimony.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Xunit;

namespace Parsimony.Tests
{
    public partial class TestOptionSet
    {

#nullable disable

        [Fact]
        public void Option_NullPropertySelector_Throws()
        {
            var underTest = new OptionSet<Opts>();
            _ = Assert.Throws<ArgumentNullException>(
                "property", () => underTest.Option(null as Expression<Func<Opts, string>>));
        }

#nullable enable

        [Fact]
        public void Option_NonPropertyExpression_Throws()
        {
            var underTest = new OptionSet<Opts>();
            _ = Assert.Throws<ArgumentException>("property", () => underTest.Option(_ => "not a property"));
        }

        [Fact]
        public void BuildParser_RequireRuleTargetsUnknownShortName_Throws()
        {
            var underTest = new OptionSet<Opts>();
            underTest.AddOption('o', o => o.Option, "an option");
            underTest.Option(o => o.Other).Requires(o => o.Option);
            _ = Assert.Throws<InvalidOperationException>(() => underTest.BuildParser());
        }

        [Fact]
        public void BuildParser_RequireRuleReferencesUnknownShortName_Throws()
        {
            var underTest = new OptionSet<Opts>();
            underTest.AddOption('o', o => o.Option, "an option");
            underTest.Option(o => o.Option).Requires(o => o.Other);
            _ = Assert.Throws<InvalidOperationException>(() => underTest.BuildParser());
        }

        [Fact]
        public void BuildParser_PrecludeRuleTargetsUnknownShortName_Throws()
        {
            var underTest = new OptionSet<Opts>();
            underTest.AddOption('o', o => o.Option, "an option");
            underTest.Option(o => o.Other).Precludes(o => o.Option);
            _ = Assert.Throws<InvalidOperationException>(() => underTest.BuildParser());
        }

        [Fact]
        public void BuildParser_PrecludeRuleReferencesUnknownShortName_Throws()
        {
            var underTest = new OptionSet<Opts>();
            underTest.AddOption('o', o => o.Option, "an option");
            underTest.Option(o => o.Option).Precludes(o => o.Other);
            _ = Assert.Throws<InvalidOperationException>(() => underTest.BuildParser());
        }

        [Fact]
        public void BuildParser_ConflictingRequiresAndPrecludes_Throws()
        {
            var tests = new Action<OptionSet<Opts>>[]
            {
                underTest =>
                {
                    underTest.Option(o => o.Option).Requires(o => o.Other);
                    underTest.Option(o => o.Option).Precludes(o => o.Other);
                },
                underTest =>
                {
                    underTest.Option(o => o.Option).Requires(o => o.Other);
                    underTest.Option(o => o.Other).Precludes(o => o.Option);
                },
                underTest =>
                {
                    underTest.Option(o => o.Other).Requires(o => o.Option);
                    underTest.Option(o => o.Option).Precludes(o => o.Other);
                },
                underTest =>
                {
                    underTest.Option(o => o.Other).Requires(o => o.Option);
                    underTest.Option(o => o.Other).Precludes(o => o.Option);
                },
            };

            foreach (var test in tests)
            {
                var underTest = new OptionSet<Opts>();
                underTest.AddOption('o', o => o.Option, "an option");
                underTest.AddOption('O', o => o.Other, "another option");
                test(underTest);
                _ = Assert.Throws<InvalidOperationException>(() => underTest.BuildParser());
            }
        }

        [Fact]
        public void Parse_MissingRequiredOption_ReturnsMissingRequiredOptionError()
        {
            var underTest = new OptionSet<Opts>();
            underTest.AddOption('o', o => o.Option, "an option");
            underTest.AddOption('O', o => o.Other, "another option");
            underTest.Option(o => o.Option).Requires(o => o.Other);
            var result = underTest.BuildParser().Parse(new[] { "-ooptionvalue" });
            var err = Assert.IsType<MissingRequiredOptionError>(result.Error);
            Assert.Equal("O", err.OptionName);
            Assert.Equal("o", err.RequiredBy);
        }

        [Fact]
        public void Parse_PrecludedOption_ReturnsPrecludedOptionError()
        {
            var underTest = new OptionSet<Opts>();
            underTest.AddOption('o', o => o.Option, "an option");
            underTest.AddOption('O', o => o.Other, "another option");
            underTest.Option(o => o.Option).Precludes(o => o.Other);
            var result = underTest.BuildParser().Parse(new[] { "-ooptionvalue", "-Ootheroptionvalue" });
            var err = Assert.IsType<PrecludedOptionError>(result.Error);
            Assert.Equal("O", err.OptionName);
            Assert.Equal("o", err.PrecludedBy);
        }
    }
}
