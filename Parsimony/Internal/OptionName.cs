using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Parsimony.Internal
{
    /// <summary>
    /// A name of an option.
    /// </summary>
    internal abstract class OptionName : IEquatable<OptionName>
    {
        /// <summary>
        /// The regex for a valid short name (any unicode letter).
        /// </summary>
        public const string ShortNamePattern = @"\p{L}";

        /// <summary>
        /// The regex for a valid long name (a hyphen-separated list or words made up of unicode letters).
        /// </summary>
        public const string LongNamePattern = @"\p{L}+(-\p{L}+)*";

        private static readonly Regex _shortNameRegex = new Regex($"^{ShortNamePattern}$");
        private static readonly Regex _longNameRegex = new Regex($"^{LongNamePattern}$");

        /// <summary>
        /// A short option name.
        /// </summary>
        public class Short : OptionName { }

        /// <summary>
        /// A long option name.
        /// </summary>
        public class Long : OptionName { }

#nullable disable
        // This is always set by Parse
        private string _name;
#nullable enable

        private OptionName() { }

        /// <summary>
        /// Returns a suitable implementation of <see cref="OptionName"/> for the given input.
        /// </summary>
        /// <param name="input">The name to parse.</param>
        /// <returns>
        /// A <see cref="Short"/> or a <see cref="Long"/> when <paramref name="input"/> matches one of the respective
        /// patterns, otherwise <c>null</c>.
        /// </returns>
        public static OptionName? Parse(string input)
        {
            if (input == null) throw new ArgumentNullException(nameof(input));

            if (_shortNameRegex.IsMatch(input))
                return new Short { _name = input };

            if (_longNameRegex.IsMatch(input))
                return new Long { _name = input };

            return null;
        }

        public override bool Equals(object? obj) => Equals(obj as OptionName);

#pragma warning disable CS8602 // Dereference of a possibly null reference.

        // The compiler doesn't understand the "as object" null checks

        public bool Equals(OptionName? other) =>
            other as object != null && _name == other._name;

        public static bool operator ==(OptionName? a, OptionName? b) =>
            a as object == null ? b as object == null : a.Equals(b);

#pragma warning restore CS8602 // Dereference of a possibly null reference.

        public static bool operator !=(OptionName? a, OptionName? b) => !(a == b);

        public override int GetHashCode() => -1125283371 + EqualityComparer<string>.Default.GetHashCode(_name);

        public override string ToString() => _name;

        public static implicit operator string(OptionName optionName) => optionName.ToString();
    }
}
