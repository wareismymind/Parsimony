using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Parsimony.Tests
{
    public class OptionBuilder
    {
        [Fact]
        public void Construct_LongNameNull_Throws()
        {
            Assert.Throws<ArgumentException>(() => new OptionBuilder<>)
        }
    }
}
