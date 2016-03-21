using System;
using System.Text;
using dumbo.Compiler.CCAnalysis;
using dumbo.Compiler.PrettyPrint;
using dumbo.Compiler.SymbolTable;

namespace dumbo.Compiler.AST
{
    public class FuncCallExprNode : ExpressionNode
    {
        public FuncCallExprNode(string identifier)
        {
            Identifier = identifier;
            Parameters = new ActualParamListNode();
        }

        public string Identifier { get; }
        public ActualParamListNode Parameters { get; }

        public override TypeDescriptor GetHappyType(ISymbolTable symbolTable)
        {
            var typeDescriptor = new TypeDescriptor(Parameters[0].GetHappyType(symbolTable).Types[0]);

            for (int i = 0; i < Parameters.Count; i++)
            {
                var parameterTypeList = Parameters[i].GetHappyType(symbolTable).Types;

                for (int j = 0; j < parameterTypeList.Count; j++)
                {
                    if(i != 0 && j != 0)
                        typeDescriptor.Add(parameterTypeList[j]);
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

        public override void CCAnalyse(ICCAnalyser analyser)
        {
            var function = analyser.SymbolTable.RetrieveSymbol(Identifier);

            if (function == null)
            {
                var error = new CCError("The function is not declared", this.Line, this.Column);
                analyser.ErrorReporter.AddError(error);
            }
            else
            {
                var functionType = function.Type as SymbolTableFunctionType;
                if (functionType == null)
                {
                    var error = new CCError("Cannot be called as a funtion because it is a variable", this.Line, this.Column);
                    analyser.ErrorReporter.AddError(error);
                }

            }

            
        }
    }
}