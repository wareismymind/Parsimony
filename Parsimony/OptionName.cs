using System;
using System.Linq;

namespace Parsimony
{
    /// <summary>
    /// A name for an option.
    /// </summary>
    internal class OptionName
    {
        /// <summary>
        /// The option's short name.
        /// </summary>
        /// <remarks>
        /// At least one of <see cref="ShortName"/> and <see cref="LongName"/> will not be <c>null</c>.
        /// </remarks>
        public char? ShortName { get; }

        /// <summary>
        /// The option's long name.
        /// </summary>
        /// <remarks>
        /// At least one of <see cref="ShortName"/> and <see cref="LongName"/> will not be <c>null</c>.
        /// </remarks>
        public string? LongName { get; }

        /// <summary>
        /// The option's
        /// </summary>
        public string DescriptiveName { get;  }

        /// <summary>
        /// Creates a new <see cref="OptionName"/>.
        /// </summary>
        /// <param name="shortName">The option's short name.</param>
        /// <remarks>
        /// <see cref="LongName"/> will be <c>null</c>.
        /// </remarks>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="shortName"/> is not a unicode letter.
        /// </exception>
        public OptionName(char shortName)
        {
            ShortName = ValidateShortName(shortName);
            DescriptiveName = $"-{shortName}";
        }

        /// <summary>
        /// Creates a new <see cref="OptionName"/>.
        /// </summary>
        /// <param name="longName">The option's long name.</param>
        /// <remarks>
        /// <para><see cref="ShortName"/> will be <c>null</c>.</para>
        /// <para>
        /// A valid long name is a string made up of one or more sequences of unicode letters separated by single
        /// hyphens that is at least 2 characters long.
        /// </para>
        /// </remarks>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="longName"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="longName"/> is not a valid long name.
        /// </exception>
        public OptionName(string longName)
        {
            LongName = ValidateLongName(longName);
            DescriptiveName = $"--{longName}";
        }

        /// <summary>
        /// Creates a new <see cref="OptionName"/>.
        /// </summary>
        /// <param name="shortName">The option's short name.</param>
        /// <param name="longName">The option's long name.</param>
        public OptionName(char shortName, string longName)
        {
            ShortName = ValidateShortName(shortName);
            LongName = ValidateLongName(longName);
            DescriptiveName = $"-{shortName}|--{longName}";
        }

        char ValidateShortName(char shortName)
        {
            if (!char.IsLetter(shortName))
                throw new ArgumentOutOfRangeException(nameof(shortName), "Must be a unicode letter");
            return shortName;
        }

        string ValidateLongName(string longName)
        {
            if (longName == null) throw new ArgumentNullException(nameof(longName));

            if (longName.Length < 2)
                throw new ArgumentOutOfRangeException(nameof(longName), "Must be at least 2 character long");

            if (!longName.All(c => char.IsLetter(c) || c == '-'))
                throw new ArgumentOutOfRangeException(
                    nameof(longName), "Must contain only unicode letters and hyphens");

            if (!char.IsLetter(longName[0]) || !char.IsLetter(longName.Last()))
                throw new ArgumentOutOfRangeException(nameof(longName), "Must begin and end with unicode letters");

            if (longName.IndexOf("--") > -1)
                throw new ArgumentOutOfRangeException(nameof(longName), "Must contain only single hyphens");

            return longName;
        }
    }
}
