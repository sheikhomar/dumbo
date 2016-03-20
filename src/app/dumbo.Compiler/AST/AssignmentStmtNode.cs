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

        public override void CCAnalyse(ICCAnalyser analyser)
        {
            // Check for the correct count of identifiers and expressions
            if (Identifiers.Count < Expressions.Count)
                analyser.ErrorReporter.AddError("Assignment Error: More Expressions than identifiers");
            else if (Identifiers.Count > Expressions.Count && Expressions.Count != 1)
                analyser.ErrorReporter.AddError("Assignment Error: Too many identifiers compared to expressions");

            for (int i = 0; i < Identifiers.Count; i++)
            {
                var id = Identifiers[i];
                var expr = Expressions[i];

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
                        analyser.ErrorReporter.AddError(new CCError($"Assignment to a function is not allowed.", Line,
                            Column));
                    }
                    else
                    {
                        var typeDescriptor = expr.GetHappyType(analyser.SymbolTable);
                        var exprType = typeDescriptor.GetFirst();
                        if (type.Type != exprType)
                        {
                            analyser.ErrorReporter.AddError(new CCError($"The variable '{id.Name}' cannot be assigned the type {exprType}.", Line,
                            Column));
                        }
                    }
                }
            }

            // Generating the TypeLists
            IList<HappyType> idTypeList = GetHappyTypeList(Identifiers, analyser.SymbolTable);
            IList<HappyType> exprTypeList = GetHappyTypeList(Expressions, analyser.SymbolTable);

            // Checking if the assignment is typecorrect
            if (exprTypeList.Count == 1 && !analyser.IsListEqualToType(idTypeList, exprTypeList[0]))
                analyser.ErrorReporter.AddError("Type Error: The assignment is not type correct.");
            else if (!analyser.IsListsEqual(idTypeList, exprTypeList))
                analyser.ErrorReporter.AddError("Type Error: The assignment is not type correct.");
        }

        private IList<HappyType> GetHappyTypeList(IEnumerable<ExpressionNode> nodeList, ISymbolTable st)
        {
            IList<HappyType> typeList = new List<HappyType>();

            foreach (var node in nodeList)
            {
                typeList.Add(node.GetHappyType(st).GetFirst());
            }

            return typeList;
        }
    }
}