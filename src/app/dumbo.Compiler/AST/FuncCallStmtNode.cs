using System.Text;
using dumbo.Compiler.CCAnalysis;
using dumbo.Compiler.SymbolTable;

namespace dumbo.Compiler.AST
{
    public class FuncCallStmtNode : StmtNode
    {
        public FuncCallStmtNode(FuncCallExprNode funcCallNode)
        {
            CallNode = funcCallNode;
        }

        public FuncCallExprNode CallNode { get; }

        public override T Accept<T, K>(IVisitor<T, K> visitor, K arg)
        {
            return visitor.Visit(this, arg);
        }

        public override void CCAnalyse(ICCAnalyser analyser)
        {
            var function = analyser.SymbolTable.RetrieveSymbol(CallNode.FuncName);

            // Check for formal and actual parameters
            if (function == null)
            {
                var error = new CCError("The function is not declared", this.Line, this.Column);
                analyser.ErrorReporter.AddError(error);
            }
            else
            {
                CallNode.CheckFormalAndActualParameters(function, analyser);

                // Check for returns nothing
                var functionType = function.Type as SymbolTableFunctionType;
                if (functionType != null)
                {
                    if (functionType.returntypes.Count != 0)
                    {
                        var error = new CCError($"Cannot use the {function.Name} function as statement. It should return 'Nothing'.", Line, Column);
                        analyser.ErrorReporter.AddError(error);
                    }
                }
            }
        }
    }
}