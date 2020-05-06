using Moq;
using Parsimony.Errors;
using Parsimony.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Parsimony.Tests
{
    public partial class TestOptionSet
    {
        internal class Opts
        {
            public virtual string Option { get; set; } = "";
            public virtual bool Flag { get; set; } 
            public int Int { get; set; }
            public virtual string Other { get; set; } = "";
            public NoConvert NoConvert { get; set; } = new NoConvert();
        }

        internal class NoConvert { }

        private class NonNewable
        {
            public NonNewable(object _) { }
        }
    }
}
