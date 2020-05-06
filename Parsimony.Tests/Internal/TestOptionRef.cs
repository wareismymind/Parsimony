﻿using Parsimony.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Parsimony.Tests.Internal
{
    public class TestOptionRef
    {
#nullable disable
        [Fact]
        public void Parse_NullInput_Throws()
        {
            Assert.Throws<ArgumentNullException>("input", () => OptionRef.Parse(null));
        }

        [Fact]
        public void Parse_EmptyInput_Throws()
        {
            Assert.Throws<ArgumentOutOfRangeException>("input", () => OptionRef.Parse(Array.Empty<string>()));
        }
#nullable enable

        //
        // Happy path
        //

        [Fact]
        public void Parse_StandaloneShortNameNoSubsequentTokens_ShortNameNone()
        {
            var (optionRef, input) = Parse("-f");
            Assert.NotNull(optionRef);
#pragma warning disable CS8602 // Dereference of a possibly null reference.
            Assert.Equal("f", optionRef.OptionName);
#pragma warning restore CS8602
            Assert.Null(optionRef.NextToken);

            Assert.Empty(input);
        }

        [Fact]
        public void Parse_StandaloneShortNameSubsequentToken_ShortNameSpace()
        {
            var (optionRef, input) = Parse("-f", "unimation");
            Assert.NotNull(optionRef);
#pragma warning disable CS8602 // Dereference of a possibly null reference.
            Assert.Equal("f", optionRef.OptionName);
#pragma warning restore CS8602
            Assert.Equal("unimation", optionRef.NextToken);
            Assert.Equal(OptionRef.JoinType.Space, optionRef.Join);

            Assert.Empty(input);
        }

        [Fact]
        public void Parse_ShortNameAdjoinedToken_ShortNameAdjoined()
        {
            var (optionRef, input) = Parse("-funimation");
            Assert.NotNull(optionRef);
#pragma warning disable CS8602 // Dereference of a possibly null reference.
            Assert.Equal("f", optionRef.OptionName);
#pragma warning restore CS8602
            Assert.Equal("unimation", optionRef.NextToken);
            Assert.Equal(OptionRef.JoinType.Adjoined, optionRef.Join);

            Assert.Empty(input);
        }

        [Fact]
        public void Parse_ShortNameAdjoinedTokenWithEquals_ShortNameAdjoined()
        {
            // The equals sign is part of the next token
            var (optionRef, input) = Parse("-f=unimation");
            Assert.NotNull(optionRef);
#pragma warning disable CS8602 // Dereference of a possibly null reference.
            Assert.Equal("f", optionRef.OptionName);
#pragma warning restore CS8602
            Assert.Equal("=unimation", optionRef.NextToken);
            Assert.Equal(OptionRef.JoinType.Adjoined, optionRef.Join);

            Assert.Empty(input);
        }

        [Fact]
        public void Parse_StandaloneLongNameNoSubsequentTokens_LongNameNone()
        {
            var (optionRef, input) = Parse("--foo");
            Assert.NotNull(optionRef);
#pragma warning disable CS8602 // Dereference of a possibly null reference.
            Assert.Equal("foo", optionRef.OptionName);
#pragma warning restore CS8602
            Assert.Null(optionRef.NextToken);

            Assert.Empty(input);
        }

        [Fact]
        public void Parse_StandaloneLongNameSubsequentTokens_LongNameSpace()
        {
            var (optionRef, input) = Parse("--foo", "nimation");
            Assert.NotNull(optionRef);
#pragma warning disable CS8602 // Dereference of a possibly null reference.
            Assert.Equal("foo", optionRef.OptionName);
#pragma warning restore CS8602
            Assert.Equal("nimation", optionRef.NextToken);
            Assert.Equal(OptionRef.JoinType.Space, optionRef.Join);

            Assert.Empty(input);
        }

        [Fact]
        public void Parse_LongNameEqualsAdjoined_LongNameEquals()
        {
            var (optionRef, input) = Parse("--foo=nimation");
            Assert.NotNull(optionRef);
#pragma warning disable CS8602 // Dereference of a possibly null reference.
            Assert.Equal("foo", optionRef.OptionName);
#pragma warning restore CS8602
            Assert.Equal("nimation", optionRef.NextToken);
            Assert.Equal(OptionRef.JoinType.Equals, optionRef.Join);

            Assert.Empty(input);
        }

        //
        // Failure paths
        //

        [Fact]
        public void Parse_SingleDash_ReturnsNull()
        {
            (var optionRef, var input) = Parse("-");
            Assert.Null(optionRef);
            Assert.Equal(new[] { "-" }.AsEnumerable(), input);
        }

        [Fact]
        public void Parse_DoubleDash_ReturnsNull()
        {
            (var optionRef, var input) = Parse("--");
            Assert.Null(optionRef);
            Assert.Equal(new[] { "--" }.AsEnumerable(), input);
        }

        [Fact]
        public void Parse_InvalidShortName_ReturnsNull()
        {
            (var optionRef, var input) = Parse("-3");
            Assert.Null(optionRef);
            Assert.Equal(new[] { "-3" }.AsEnumerable(), input);
        }

        [Theory]
        [InlineData("--inv4l1d-ch4r5")]
        [InlineData("--end-with-dash-")]
        [InlineData("--double--dash-inside")]
        public void Parse_InvalidLongName_ReturnsNull(string longName)
        {
            (var optionRef, var input) = Parse(longName);
            Assert.Null(optionRef);
            Assert.Equal(new[] { longName }.AsEnumerable(), input);
        }

        //
        // Non-ASCII
        //

        [Fact]
        public void Parse_NonAsciiShortName_ShortName()
        {
            var (optionRef, _) = Parse("-ά");
            Assert.NotNull(optionRef);
#pragma warning disable CS8602 // Dereference of a possibly null reference.
            Assert.Equal("ά", optionRef.OptionName);
#pragma warning restore CS8602
        }

        [Fact]
        public void Parse_NonAsciiLongName_LongName()
        {
            var (optionRef, _) = Parse("--cάke-and-icecreάm");
            Assert.NotNull(optionRef);
#pragma warning disable CS8602 // Dereference of a possibly null reference.
            Assert.Equal("cάke-and-icecreάm", optionRef.OptionName);
#pragma warning restore CS8602
        }

        //
        // Utilities
        //

        private (OptionRef?, IReadOnlyList<string>) Parse(params string[] input)
        {
            (var optionRef, var newInput) = OptionRef.Parse(input);
            return (optionRef, newInput.ToList());
        }
    }
}
