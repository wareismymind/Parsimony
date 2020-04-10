using System;

namespace Parsimony.Tests
{
    // This class is public to support mocking.
    public class Opts
    {
        public bool Flag { get; set; }

        public string Value { get; set; }

        public Opts(bool flag, string value)
        {
            Flag = flag;
            Value = value ?? throw new ArgumentNullException(nameof(value));
        }
    }
}
