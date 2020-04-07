namespace Parsimony.Tests
{
    public class TestDummy
    {
        public bool BoolProp { get; set; }
        public string? StringProp { get; set; }
        public int IntProp { get; set; }

        public bool GetMeADoot() => true;

        public int DoTheSet
        {
            set => _toBeSet = value;
        }

        public int ReadOnly => 22;

        public int NotAProperty;


        private int _toBeSet;
    }
}
