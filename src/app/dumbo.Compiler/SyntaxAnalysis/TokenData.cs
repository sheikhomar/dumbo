using GOLD;

namespace dumbo.Compiler.SyntaxAnalysis
{
    public class TokenData
    {
        public TokenData(Position position, object extraData)
        {
            ExtraData = extraData;
            LineNumber = position.Line;
            Column = position.Column;
            IsToken = extraData is string;
            Spelling = IsToken ? extraData as string : string.Empty;
        }

        public int LineNumber { get; }
        public int Column { get; }
        public bool IsToken { get; }
        public object ExtraData { get; }
        public string Spelling { get; }
    }
}
