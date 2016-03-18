using System;
using System.Text;
using dumbo.Compiler.PrettyPrint;
using dumbo.Compiler.SymbolTable;

namespace dumbo.Compiler.AST
{
    public class FuncCallNode : ExpressionNode
    {
        public FuncCallNode(string identifier)
        {
            Identifier = identifier;
            Parameters = new ActualParamListNode();
        }

        public string Identifier { get; }
        public ActualParamListNode Parameters { get; }

        public override TypeDescriptor GetHappyType(ISymbolTable symbolTable)
        {
            var typeDescriptor = new TypeDescriptor();

            foreach (var parameter in Parameters)
            {
                var parameterTypeList = parameter.GetHappyType(symbolTable).Types;

                foreach (var element in parameterTypeList)
                {
                    typeDescriptor.Add(element);
                }
            }

            return typeDescriptor;
        }

        public override void PrettyPrint(IPrettyPrinter prettyPrinter)
        {
            prettyPrinter.Append(Identifier + "(");
            Parameters.PrettyPrint(prettyPrinter);
            prettyPrinter.Append(")");
        }
    }
}