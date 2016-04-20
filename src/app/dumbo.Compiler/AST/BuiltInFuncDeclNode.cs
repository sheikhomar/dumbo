using System.Collections.Generic;

namespace dumbo.Compiler.AST
{
    public class BuiltInFuncDeclNode : FuncDeclNode
    {
        public BuiltInFunction Type { get; }

        public BuiltInFuncDeclNode(BuiltInFunction type) :
            base(type.ToString(), new StmtBlockNode(), new SourcePosition(0, 0, 0, 0))
        {
            Type = type;
        }

        public override bool IsBuiltIn => true;

        public override T Accept<T, K>(IVisitor<T, K> visitor, K arg)
        {
            return visitor.Visit(this, arg);
        }

        public static IEnumerable<BuiltInFuncDeclNode> GetBuiltInFunctions()
        {
            var srcPos = new SourcePosition(0, 0, 0, 0);

            // Define Write function
            var write = new BuiltInFuncDeclNode(BuiltInFunction.Write);
            write.Parameters.Add(new FormalParamNode("input", HappyType.Text, srcPos));

            // Define ReadText function
            var readText = new BuiltInFuncDeclNode(BuiltInFunction.ReadText);
            readText.ReturnTypes.Add(HappyType.Text);

            // Define ReadNumber function
            var readNumber = new BuiltInFuncDeclNode(BuiltInFunction.ReadNumber);
            readNumber.ReturnTypes.Add(HappyType.Number);

            // Define Floor function
            var floor = new BuiltInFuncDeclNode(BuiltInFunction.Floor);
            floor.Parameters.Add(new FormalParamNode("n1", HappyType.Number, srcPos));
            floor.ReturnTypes.Add(HappyType.Number);

            // Define Ceiling function
            var ceiling = new BuiltInFuncDeclNode(BuiltInFunction.Ceiling);
            ceiling.Parameters.Add(new FormalParamNode("n1", HappyType.Number, srcPos));
            ceiling.ReturnTypes.Add(HappyType.Number);

            // Define Random function
            var random = new BuiltInFuncDeclNode(BuiltInFunction.Random);
            random.Parameters.Add(new FormalParamNode("n1", HappyType.Number, srcPos));
            random.Parameters.Add(new FormalParamNode("n2", HappyType.Number, srcPos));
            random.ReturnTypes.Add(HappyType.Number);
            
            // Define IsEqual function
            var isEqual = new BuiltInFuncDeclNode(BuiltInFunction.IsEqual);
            isEqual.Parameters.Add(new FormalParamNode("t1", HappyType.Text, srcPos));
            isEqual.Parameters.Add(new FormalParamNode("t2", HappyType.Text, srcPos));
            isEqual.ReturnTypes.Add(HappyType.Boolean);

            yield return ceiling;
            yield return floor;
            yield return isEqual;
            yield return random;
            yield return readNumber;
            yield return readText;
            yield return write;
        } 
    }
}