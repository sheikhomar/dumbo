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
            write.Parameters.Add(new FormalParamNode("input", new PrimitiveTypeNode(PrimitiveType.Text), srcPos));

            // Define ReadText function
            var readText = new BuiltInFuncDeclNode(BuiltInFunction.ReadText);
            readText.ReturnTypes.Add(new PrimitiveTypeNode(PrimitiveType.Text));

            // Define ReadNumber function
            var readNumber = new BuiltInFuncDeclNode(BuiltInFunction.ReadNumber);
            readNumber.ReturnTypes.Add(new PrimitiveTypeNode(PrimitiveType.Number));

            // Define Floor function
            var floor = new BuiltInFuncDeclNode(BuiltInFunction.Floor);
            floor.Parameters.Add(new FormalParamNode("n1", new PrimitiveTypeNode(PrimitiveType.Number), srcPos));
            floor.ReturnTypes.Add(new PrimitiveTypeNode(PrimitiveType.Number));

            // Define Ceiling function
            var ceiling = new BuiltInFuncDeclNode(BuiltInFunction.Ceiling);
            ceiling.Parameters.Add(new FormalParamNode("n1", new PrimitiveTypeNode(PrimitiveType.Number), srcPos));
            ceiling.ReturnTypes.Add(new PrimitiveTypeNode(PrimitiveType.Number));

            // Define Random function
            var random = new BuiltInFuncDeclNode(BuiltInFunction.Random);
            random.Parameters.Add(new FormalParamNode("n1", new PrimitiveTypeNode(PrimitiveType.Number), srcPos));
            random.Parameters.Add(new FormalParamNode("n2", new PrimitiveTypeNode(PrimitiveType.Number), srcPos));
            random.ReturnTypes.Add(new PrimitiveTypeNode(PrimitiveType.Number));

            // Define IsTextAndTextEqual function
            var isTextAndTextEqual = new BuiltInFuncDeclNode(BuiltInFunction.IsTextAndTextEqual);
            isTextAndTextEqual.Parameters.Add(new FormalParamNode("t1", new PrimitiveTypeNode(PrimitiveType.Text), srcPos));
            isTextAndTextEqual.Parameters.Add(new FormalParamNode("t2", new PrimitiveTypeNode(PrimitiveType.Text), srcPos));
            isTextAndTextEqual.ReturnTypes.Add(new PrimitiveTypeNode(PrimitiveType.Boolean));

            // Define ConvertNumberToText function
            var convertNumberToText = new BuiltInFuncDeclNode(BuiltInFunction.ConvertNumberToText);
            convertNumberToText.Parameters.Add(new FormalParamNode("n", new PrimitiveTypeNode(PrimitiveType.Number), srcPos));
            convertNumberToText.ReturnTypes.Add(new PrimitiveTypeNode(PrimitiveType.Text));

            // Define ConvertBooleanToText function
            var convertBooleanToText = new BuiltInFuncDeclNode(BuiltInFunction.ConvertBooleanToText);
            convertBooleanToText.Parameters.Add(new FormalParamNode("n", new PrimitiveTypeNode(PrimitiveType.Boolean), srcPos));
            convertBooleanToText.ReturnTypes.Add(new PrimitiveTypeNode(PrimitiveType.Text));

            yield return ceiling;
            yield return floor;
            yield return isTextAndTextEqual;
            yield return random;
            yield return readNumber;
            yield return readText;
            yield return write;
            yield return convertNumberToText;
            yield return convertBooleanToText;
        } 
    }
}