using System.Text;
using dumbo.Compiler.CCAnalysis;
using dumbo.Compiler.PrettyPrint;
using dumbo.Compiler.SymbolTable;
using System.Collections.Generic;

namespace dumbo.Compiler.AST
{
    public class DeclAndAssignmentStmtNode : AssignmentStmtNode
    {
        public DeclAndAssignmentStmtNode(HappyType type, 
            IdentifierListNode identifiers, 
            ExpressionListNode expressions) : base(identifiers, expressions)
        {
            Type = type;
        }

        public HappyType Type { get; }

        public override void PrettyPrint(IPrettyPrinter prettyPrinter)
        {
            prettyPrinter.Append(Type.ToString() + " ");
            Identifiers.PrettyPrint(prettyPrinter);
            prettyPrinter.Append(" := ");
            Expressions.PrettyPrint(prettyPrinter);
            prettyPrinter.EndLine();
        }

        public override void CCAnalyse(ICCAnalyser analyser)
        {
            //Check all Type and literal, remember that a, b = 5 ie last exp
            //This must be done before scope stuff.

            // Generating TypeLists
            var idTypeList = GetHappyTypeList(Identifiers, analyser.SymbolTable);
            var exprTypeList = GetHappyTypeList(Expressions, analyser.SymbolTable);

            // Checking for correct id and expr count
            if (exprTypeList.Count < exprTypeList.Count)
                analyser.ErrorReporter.AddError("Decl Ass Error: Too many expressions compared to identifiers");
            else if (idTypeList.Count > exprTypeList.Count && exprTypeList.Count != 1)
                analyser.ErrorReporter.AddError("Decl Ass Error: Incompatible amount of identifiers and expressions.");

            // Checking for correct types in expr compared to the declaration type
            if (exprTypeList.Count == 1 && !analyser.IsEqual(exprTypeList[0], Type))
                analyser.ErrorReporter.AddError("Type Error: The assignment is not type correct.");
            else if (!analyser.IsListEqualToType(exprTypeList, Type))
                analyser.ErrorReporter.AddError("Type Error: The assignment is not type correct.");

            // Adding identifiers to symboltable
            foreach (var id in Identifiers)
            {
                analyser.SymbolTable.EnterSymbol(id.Name, new SymbolTablePrimitiveType(Type));
            }
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