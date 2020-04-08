namespace Parsimony.ParserBuilder
{
    //TODO:CN -- Can probably just make into a bunch of separate types and pattern match 
    // on them for the impls
    public class Rule
    {
        public string PropertyName { get; }
        public string Target { get; }
        public RuleKind Kind { get; }

        public Rule(RuleKind kind, string propertyName, string target)
        {
            //CN: Check for stuff
            Kind = kind;
            PropertyName = propertyName;
            Target = target;
        }
    }
   
}
