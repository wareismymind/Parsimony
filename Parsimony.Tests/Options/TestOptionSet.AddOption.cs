using Moq;
using Parsimony.Errors;
using Parsimony.Options;
using System;
using System.Linq;
using Xunit;

namespace Parsimony.Tests
{
    public partial class TestOptionSet
    {
        // There are 12 overloads of AddOption and each one has 3-6 parameters. Every overload requires the property
        // expression, and help text arguments, and at least one of shortname or longname. For the sake of brevity the
        // test names will exclude the parameters that are always present. All the implementations use the same method
        // internally for everything but their null checks, so each overload will have its null checks tested then each
        // expected behaviour will be tested using just one appropriate overload.

#nullable disable

        #region AddOption(shortName, property, helpText)

        [Fact]
        public void AddOptionShortName_NullProperty_Throws()
        {
            Assert.Throws<ArgumentNullException>("property", () => new OptionSet<Opts>().AddOption<string>(
                'o', null, "an option"));
        }

        [Fact]
        public void AddOptionShortName_NullHelpText_Throws()
        {
            Assert.Throws<ArgumentNullException>("helpText", () => new OptionSet<Opts>().AddOption(
                'o', o => o.Option, null));
        }

        #endregion AddOption(shortName, property, helpText)

        #region AddOption(longName, property, helpText)

        [Fact]
        public void AddOptionLongName_NullLongName_Throws()
        {
            Assert.Throws<ArgumentNullException>("longName", () => new OptionSet<Opts>().AddOption<string>(
                null as string, o => o.Option, "an option"));
        }

        [Fact]
        public void AddOptionLongName_NullProperty_Throws()
        {
            Assert.Throws<ArgumentNullException>("property", () => new OptionSet<Opts>().AddOption<string>(
                "option", null, "an option"));
        }

        [Fact]
        public void AddOptionLongName_NullHelpText_Throws()
        {
            Assert.Throws<ArgumentNullException>("helpText", () => new OptionSet<Opts>().AddOption(
                "option", o => o.Option, null));
        }

        #endregion AddOption(longName, property, helpText)

        #region AddOption(shortName, longName, property, helpText)

        [Fact]
        public void AddOptionBothNames_NullLongName_Throws()
        {
            Assert.Throws<ArgumentNullException>("longName", () => new OptionSet<Opts>().AddOption<string>(
                'o', null as string, o => o.Option, "an option"));
        }

        [Fact]
        public void AddOptionBothNames_NullProperty_Throws()
        {
            Assert.Throws<ArgumentNullException>("property", () => new OptionSet<Opts>().AddOption<string>(
                'o', "option", null, "an option"));
        }

        [Fact]
        public void AddOptionBothNames_NullHelpText_Throws()
        {
            Assert.Throws<ArgumentNullException>("helpText", () => new OptionSet<Opts>().AddOption(
                'o', "option", o => o.Option, null));
        }

        #endregion AddOption(shortName, longName, property, helpText)


        #region AddOption(shortName, property, parser, helpText)

        [Fact]
        public void AddOptionShortNameParser_NullProperty_Throws()
        {
            Assert.Throws<ArgumentNullException>("property", () => new OptionSet<Opts>().AddOption<string>(
                'o', null, s => s, "an option"));
        }

        [Fact]
        public void AddOptionShortNameParser_NullParser_Throws()
        {
            Assert.Throws<ArgumentNullException>("optionValueParser", () => new OptionSet<Opts>().AddOption(
                'o', o => o.Option, null as Func<string, string>, "an option"));
        }

        [Fact]
        public void AddOptionShortNameParser_NullHelpText_Throws()
        {
            Assert.Throws<ArgumentNullException>("helpText", () => new OptionSet<Opts>().AddOption(
                'o', o => o.Option, s => s, null));
        }

        #endregion AddOption(shortName, property, parser, helpText)

        #region AddOption(longName, property, parser, helpText)
        
        [Fact]
        public void AddOptionLongNameParser_NullLongName_Throws()
        {
            Assert.Throws<ArgumentNullException>("longName", () => new OptionSet<Opts>().AddOption<string>(
                null, o => o.Option, s => s, "an option"));
        }

        [Fact]
        public void AddOptionLongNameParser_NullProperty_Throws()
        {
            Assert.Throws<ArgumentNullException>("property", () => new OptionSet<Opts>().AddOption<string>(
                "option", null, s => s, "an option"));
        }

        [Fact]
        public void AddOptionLongNameParser_NullParser_Throws()
        {
            Assert.Throws<ArgumentNullException>("optionValueParser", () => new OptionSet<Opts>().AddOption(
                "option", o => o.Option, null as Func<string, string>, "an option"));
        }

        [Fact]
        public void AddOptionLongNameParser_NullHelpText_Throws()
        {
            Assert.Throws<ArgumentNullException>("helpText", () => new OptionSet<Opts>().AddOption(
                "option", o => o.Option, s => s, null));
        }

        #endregion AddOption(longName, property, parser, helpText)

        #region AddOption(shortName, longName, property, parser, helpText)

        [Fact]
        public void AddOptionBothNamesParser_NullLongName_Throws()
        {
            Assert.Throws<ArgumentNullException>("longName", () => new OptionSet<Opts>().AddOption<string>(
                'o', null, o => o.Option, s => s, "an option"));
        }

        [Fact]
        public void AddOptionBothNamesParser_NullProperty_Throws()
        {
            Assert.Throws<ArgumentNullException>("property", () => new OptionSet<Opts>().AddOption<string>(
                'o', "option", null, s => s, "an option"));
        }

        [Fact]
        public void AddOptionBothNamesParser_NullParser_Throws()
        {
            Assert.Throws<ArgumentNullException>("optionValueParser", () => new OptionSet<Opts>().AddOption(
                'o', "option", o => o.Option, null as Func<string, string>, "an option"));
        }

        [Fact]
        public void AddOptionBothNamesParser_NullHelpText_Throws()
        {
            Assert.Throws<ArgumentNullException>("helpText", () => new OptionSet<Opts>().AddOption(
                'o', "option", o => o.Option, s => s, null));
        }

        #endregion AddOption(shortName, longName, property, parser, helpText)

        
        #region AddOption(shortName, property, helpText, helpTextValueName)

        [Fact]
        public void AddOptionShortNameValueName_NullProperty_Throws()
        {
            Assert.Throws<ArgumentNullException>("property", () => new OptionSet<Opts>().AddOption<string>(
                'o', null, "an option", "OPT"));
        }

        [Fact]
        public void AddOptionShortNameValueName_NullHelpText_Throws()
        {
            Assert.Throws<ArgumentNullException>("helpText", () => new OptionSet<Opts>().AddOption(
                'o', o => o.Option, null as string, "OPT"));
        }

        [Fact]
        public void AddOptionShortNameValueName_NullValueName_Throws()
        {
            Assert.Throws<ArgumentNullException>("helpTextValueName", () => new OptionSet<Opts>().AddOption(
                'o', o => o.Option, "an option", null));
        }

        #endregion AddOption(shortName, property, helpText, helpTextValueName)

        #region AddOption(longName, property, helpText, helpTextValueName)

        [Fact]
        public void AddOptionLongNameValueName_NullLongName_Throws()
        {
            Assert.Throws<ArgumentNullException>("longName", () => new OptionSet<Opts>().AddOption<string>(
                null as string, o => o.Option, "an option", "OPT"));
        }

        [Fact]
        public void AddOptionLongNameValueName_NullProperty_Throws()
        {
            Assert.Throws<ArgumentNullException>("property", () => new OptionSet<Opts>().AddOption<string>(
                "option", null, "an option", "OPT"));
        }

        [Fact]
        public void AddOptionLongNameValueName_NullHelpText_Throws()
        {
            Assert.Throws<ArgumentNullException>("helpText", () => new OptionSet<Opts>().AddOption(
                "option", o => o.Option, null as string, "OPT"));
        }

        [Fact]
        public void AddOptionLongNameValueName_NullValueName_Throws()
        {
            Assert.Throws<ArgumentNullException>("helpTextValueName", () => new OptionSet<Opts>().AddOption(
                "option", o => o.Option, "an option", null));
        }

        #endregion AddOption(longName, property, helpText, helpTextValueName)

        #region AddOption(shortName, longName, property, helpText, helpTextValueName)

        [Fact]
        public void AddOptionBothNamesValueName_NullLongName_Throws()
        {
            Assert.Throws<ArgumentNullException>("longName", () => new OptionSet<Opts>().AddOption<string>(
                'o', null as string, o => o.Option, "an option", "OPT"));
        }

        [Fact]
        public void AddOptionBothNamesValueName_NullProperty_Throws()
        {
            Assert.Throws<ArgumentNullException>("property", () => new OptionSet<Opts>().AddOption<string>(
                'o', "option", null, "an option", "OPT"));
        }

        [Fact]
        public void AddOptionBothNamesValueName_NullHelpText_Throws()
        {
            Assert.Throws<ArgumentNullException>("helpText", () => new OptionSet<Opts>().AddOption(
                'o', "option", o => o.Option, null as string, "OPT"));
        }

        [Fact]
        public void AddOptionBothNamesValueName_NullValueName_Throws()
        {
            Assert.Throws<ArgumentNullException>("helpTextValueName", () => new OptionSet<Opts>().AddOption(
                'o', "option", o => o.Option, "an option", null));
        }

        #endregion AddOption(shortName, longName, property, helpText, helpTextValueName)


        #region AddOption(shortName, property, parser, helpText, helpTextValueName)

        [Fact]
        public void AddOptionShortNameParserValueName_NullProperty_Throws()
        {
            Assert.Throws<ArgumentNullException>("property", () => new OptionSet<Opts>().AddOption<string>(
                'o', null, s => s, "an option", "OPT"));
        }

        [Fact]
        public void AddOptionShortNameParserValueName_NullParser_Throws()
        {
            Assert.Throws<ArgumentNullException>("optionValueParser", () => new OptionSet<Opts>().AddOption(
                'o', o => o.Option, null as Func<string, string>, "an option", "OPT"));
        }

        [Fact]
        public void AddOptionShortNameParserValueName_NullHelpText_Throws()
        {
            Assert.Throws<ArgumentNullException>("helpText", () => new OptionSet<Opts>().AddOption(
                'o', o => o.Option, s => s, null, "OPT"));
        }

        [Fact]
        public void AddOptionShortNameParserValueName_NullValueName_Throws()
        {
            Assert.Throws<ArgumentNullException>("helpTextValueName", () => new OptionSet<Opts>().AddOption(
                'o', o => o.Option, s => s, "an option", null));
        }

        #endregion AddOption(shortName, property, parser, helpText, helpTextValueName)

        #region AddOption(longName, property, parser, helpText, helpTextValueName)

        [Fact]
        public void AddOptionLongNameParserValueName_NullLongName_Throws()
        {
            Assert.Throws<ArgumentNullException>("longName", () => new OptionSet<Opts>().AddOption<string>(
                null, o => o.Option, s => s, "an option", "OPT"));
        }

        [Fact]
        public void AddOptionLongNameParserValueName_NullProperty_Throws()
        {
            Assert.Throws<ArgumentNullException>("property", () => new OptionSet<Opts>().AddOption<string>(
                "option", null, s => s, "an option", "OPT"));
        }

        [Fact]
        public void AddOptionLongNameParserValueName_NullParser_Throws()
        {
            Assert.Throws<ArgumentNullException>("optionValueParser", () => new OptionSet<Opts>().AddOption(
                "option", o => o.Option, null as Func<string, string>, "an option", "OPT"));
        }

        [Fact]
        public void AddOptionLongNameParserValueName_NullHelpText_Throws()
        {
            Assert.Throws<ArgumentNullException>("helpText", () => new OptionSet<Opts>().AddOption(
                "option", o => o.Option, s => s, null, "OPT"));
        }

        [Fact]
        public void AddOptionLongNameParserValueName_NullValueName_Throws()
        {
            Assert.Throws<ArgumentNullException>("helpTextValueName", () => new OptionSet<Opts>().AddOption(
                "option", o => o.Option, s => s, "an option", null));
        }

        #endregion AddOption(longName, property, parser, helpText, helpTextValueName)

        #region AddOption(shortName, longName, property, parser, helpText, helpTextValueName)

        [Fact]
        public void AddOptionBothNamesParserValueName_NullLongName_Throws()
        {
            Assert.Throws<ArgumentNullException>("longName", () => new OptionSet<Opts>().AddOption(
                'o', null, o => o.Option, s => s, "an option", "OPT"));
        }

        [Fact]
        public void AddOptionBothNamesParserValueName_NullProperty_Throws()
        {
            Assert.Throws<ArgumentNullException>("property", () => new OptionSet<Opts>().AddOption(
                'o', "option", null, s => s, "an option", "OPT"));
        }

        [Fact]
        public void AddOptionBothNamesParserValueName_NullParser_Throws()
        {
            Assert.Throws<ArgumentNullException>("optionValueParser", () => new OptionSet<Opts>().AddOption(
                'o', "option", o => o.Option, null, "an option", "OPT"));
        }

        [Fact]
        public void AddOptionBothNamesParserValueName_NullHelpText_Throws()
        {
            Assert.Throws<ArgumentNullException>("helpText", () => new OptionSet<Opts>().AddOption(
                'o', "option", o => o.Option, s => s, null, "OPT"));
        }

        [Fact]
        public void AddOptionBothNamesParserValueName_NullValueName_Throws()
        {
            Assert.Throws<ArgumentNullException>("helpTextValueName", () => new OptionSet<Opts>().AddOption(
                'o', "option", o => o.Option, s => s, "an option", null));
        }

        #endregion AddOption(shortName, longName, property, parser, helpText, helpTextValueName)

#nullable enable

        [Theory]
        [InlineData(' ')]
        [InlineData('\t')]
        [InlineData('3')]
        [InlineData('$')]
        public void AddOption_InvalidShortName_Throws(char shortName)
        {
            Assert.Throws<ArgumentException>("shortName", () => new OptionSet<Opts>().AddOption(
                shortName, o => o.Option, "an option"));
        }

        [Theory]
        [InlineData("a")]
        [InlineData("contains spaces")]
        [InlineData("special-ch@rs")]
        [InlineData("-leading-dash")]
        [InlineData("trailing-dash-")]
        [InlineData("double--dash")]
        public void AddOption_InvalidLongName_Throws(string longName)
        {
            Assert.Throws<ArgumentException>("longName", () => new OptionSet<Opts>().AddOption(
                longName, o => o.Option, "an option"));
        }

        [Fact]
        public void AddOption_NonPropertyExpression_Throws()
        {
            Assert.Throws<ArgumentException>("property", () => new OptionSet<Opts>().AddOption(
                'o', _ => "not a property", "an option"));
        }

        [Theory]
        [InlineData("")]
        [InlineData(" \t\n\r")]
        public void AddOption_WhitespaceHelpText_Throws(string helpText)
        {
            Assert.Throws<ArgumentException>("helpText", () => new OptionSet<Opts>().AddOption(
                'o', o => o.Option, helpText));
        }

        [Theory]
        [InlineData("")]
        [InlineData("multiple tokens")]
        [InlineData("multiple\ttokens")]
        [InlineData("multiple\rtokens")]
        [InlineData("multiple\ntokens")]
        [InlineData("speci@l")]
        public void AddOptionBothNamesParserValueName_InvalidHelpTextValueName_Throws(string valueName)
        {
            Assert.Throws<ArgumentException>("helpTextValueName", () => new OptionSet<Opts>().AddOption(
                'o', o => o.Option, "an option", valueName));
        }

        [Fact]
        public void AddOption_SetAlreadyContainsShortName_Throws()
        {
            var underTest = new OptionSet<Opts>();
            underTest.AddOption('o', o => o.Option, "an option");
            Assert.Throws<InvalidOperationException>(
                () => underTest.AddOption('o', o => o.Other, "another option"));
        }

        [Fact]
        public void AddOption_MultipleNullShortNames_AreAllowed()
        {
            var underTest = new OptionSet<Opts>();
            underTest.AddOption("option", o => o.Option, "an option");
            underTest.AddOption("other", o => o.Other, "another option");
        }

        [Fact]
        public void AddOption_SetAlreadyContainsLongName_Throws()
        {
            var underTest = new OptionSet<Opts>();
            underTest.AddOption("option", o => o.Other, "an option");
            Assert.Throws<InvalidOperationException>(
                () => underTest.AddOption("option", o => o.Other, "another option"));
        }

        [Fact]
        public void AddOption_MultipleNullLongNames_AreAllowed()
        {
            var underTest = new OptionSet<Opts>();
            underTest.AddOption('o', o => o.Option, "an option");
            underTest.AddOption('O', o => o.Other, "another option");
        }

        [Fact]
        public void AddOption_SetAlreadyContainsProperty_Throws()
        {
            var underTest = new OptionSet<Opts>();
            underTest.AddOption('o', o => o.Option, "an option");
            Assert.Throws<InvalidOperationException>(
                () => underTest.AddOption("option", o => o.Option, "an option again"));
        }

        [Fact]
        public void AddOption_NoConversionNoParser_Throws()
        {
            Assert.Throws<InvalidOperationException>(
                () => new OptionSet<Opts>().AddOption('n', o => o.NoConvert, "an option without a conversion"));
        }
    }
}
