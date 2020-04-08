using System;
using System.Collections.Generic;
using System.Linq;

namespace Parsimony.ParserBuilder
{
    internal class OptionParserBuildResult<TOptions>
    {
        internal Option<TOptions> Parser { get; set; }
        internal IEnumerable<Rule> Rules { get; set; } = new List<Rule>();

        public OptionParserBuildResult(Option<TOptions> parser, IEnumerable<Rule> rules)
        {
            Parser = parser ?? throw new ArgumentNullException(nameof(parser));
            Rules = rules?.ToList() ?? throw new ArgumentNullException();
        }
    }
   
}
