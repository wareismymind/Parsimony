namespace Parsimony
{
    /// <summary>
    /// Configuration options for a parser.
    /// </summary>
    public class ParserConfig
    {
        /// <summary>
        /// Indicates whether a parser should treat all tokens following a double-dash
        /// (&quot;--&quot;) token.
        /// as positional arguments.
        /// </summary>
        public bool DoubleDash { get; set; }

        /// <summary>
        /// Indicates whether a parser should treat all tokens following the first non-option token
        /// as positional arguments.
        /// </summary>
        public bool OptionsFirst { get; set; }
    }
}
