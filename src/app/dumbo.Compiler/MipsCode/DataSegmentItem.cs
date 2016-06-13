namespace dumbo.Compiler.MipsCode
{
    public class DataSegmentItem
    {
        public string Name { get;  }
        public string Type { get;  }
        public string Value { get; }

        public DataSegmentItem(string name, string type, string value)
        {
            Name = name;
            Type = type;
            Value = value;
        }
    }
}