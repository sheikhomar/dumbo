using dumbo.Compiler.PrettyPrint;
using System.Collections.Generic;
using System.Text;
using dumbo.Compiler.SymbolTable;
using dumbo.Compiler.CCAnalysis;

namespace dumbo.Compiler.AST
{
    public class AssignmentStmtNode : StmtNode
    {
        public AssignmentStmtNode(IdentifierListNode identifiers, ExpressionListNode expressions, int line, int column)
        {
            Identifiers = identifiers;
            Expressions = expressions;
            Line = line;
            Column = column;
        }

        public IdentifierListNode Identifiers { get; }
        public ExpressionListNode Expressions { get; }

        public override void PrettyPrint(IPrettyPrinter prettyPrinter)
        {
            Identifiers.PrettyPrint(prettyPrinter);

            prettyPrinter.Append(" := ");

            Expressions.PrettyPrint(prettyPrinter);

            prettyPrinter.EndLine();
        }
        
        public override VisitResult Accept(IVisitor visitor, VisitorArgs arg)
        {
            return visitor.Visit(this, arg);
        }

        public override void CCAnalyse(ICCAnalyser analyser)
        {
            IList<HappyType> exprTypeList = new List<HappyType>();

            // Check for the correct count of identifiers and expressions
            if (Identifiers.Count < Expressions.Count)
            {
                analyser.ErrorReporter.AddError("Assignment Error: More Expressions than identifiers");
                return;
            }
            
            exprTypeList = CheckExpressionForMultipleReturnTypes(analyser);
                                    
            if (Identifiers.Count > exprTypeList.Count)
                analyser.ErrorReporter.AddError("Assignment Error: Too many identifiers compared to expression types.");
            else if(Identifiers.Count < exprTypeList.Count)
                analyser.ErrorReporter.AddError("Assignment Error: Too many expression types compared to identifiers.");


            if (Identifiers.Count == exprTypeList.Count)
            {
                for (int i = 0; i < Identifiers.Count; i++)
                {
                    var id = Identifiers[i];
                    var exprType = exprTypeList[i];
                    var entry = analyser.SymbolTable.RetrieveSymbol(id.Name);
                    if (entry == null)
                    {
                        analyser.ErrorReporter.AddError(new CCError($"Identifier '{id.Name}' is undeclared.", Line, Column));
                    }
                    else
                    {
                        var type = entry.Type as SymbolTablePrimitiveType;
                        if (type == null)
                        {
                            analyser.ErrorReporter.AddError(new CCError($"Assignment to a function is not allowed.", Line, Column));
                        }
                        else
                        {
                            if (type.Type != exprType)
                            {
                                analyser.ErrorReporter.AddError(new CCError($"The variable '{id.Name}' cannot be assigned the type {exprType}.", Line, Column));
                            }
                        }
                    }
                }
            }
        }

        protected IList<HappyType> CheckExpressionForMultipleReturnTypes(ICCAnalyser analyser)
        {
            var expressionTypes = new List<HappyType>();

            foreach (var expr in Expressions)
            {
                expressionTypes.AddRange(expr.GetHappyType(analyser.SymbolTable).Types);
                expr.CCAnalyse(analyser);
            }
            return expressionTypes;
        }
    }
}