namespace Parsimony.Options
{
    /// <summary>
    /// Configurable options for option parsing.
    /// </summary>
    public class ParserConfig
    {
        /// <summary>
        /// When true, option parsing will stop at the first non-option token.
        /// </summary>
        /// <remarks>
        /// This can be used to support treating arguments as subcommands and the options that follow them as options
        /// for the subcommand instead of its parent.
        /// </remarks>
        public bool PosixOptionOrder { get; set; }
    }
}
