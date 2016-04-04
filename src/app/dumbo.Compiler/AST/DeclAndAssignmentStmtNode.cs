using System.Text;
using dumbo.Compiler.CCAnalysis;
using dumbo.Compiler.SymbolTable;
using System.Collections.Generic;

namespace dumbo.Compiler.AST
{
    public class DeclAndAssignmentStmtNode : AssignmentStmtNode
    {
        public DeclAndAssignmentStmtNode(HappyType type, 
            IdentifierListNode identifiers, 
            ExpressionListNode expressions,
            SourcePosition sourcePosition) : base(identifiers, expressions, sourcePosition)
        {
            Type = type;
        }

        public HappyType Type { get; }
        
        public override VisitResult Accept(IVisitor visitor, VisitorArgs arg)
        {
            return visitor.Visit(this, arg);
        }
        
        public override void CCAnalyse(ICCAnalyser analyser)
        {
            // Check for the correct count of identifiers and expressions
            if (Identifiers.Count < Expressions.Count)
            {
                analyser.ErrorReporter.AddError("Assignment Error: More Expressions than identifiers");
                return;
            }

            IList<HappyType> exprTypeList = CheckExpressionForMultipleReturnTypes(analyser);

            if (Identifiers.Count > exprTypeList.Count)
                analyser.ErrorReporter.AddError("Assignment Error: Too many identifiers compared to expression types.");
            else if (Identifiers.Count < exprTypeList.Count)
                analyser.ErrorReporter.AddError("Assignment Error: Too many expression types compared to identifiers.");

            if (Identifiers.Count == exprTypeList.Count)
            {
                for (int i = 0; i < Identifiers.Count; i++)
                {
                    var id = Identifiers[i];
                    var exprType = exprTypeList[i];
                    if (Type == exprType)
                        analyser.AddVariableToSymbolTable(id.Name, Type, false, Line, Column);
                    else
                        analyser.ErrorReporter.AddError(new CCError($"Identifier '{id.Name}' has a different type than the assigned expression", Line, Column));
                }
            }

            ////================================    OLD    ====================================//
            ////Check all Type and literal, remember that a, b = 5 ie last exp
            ////This must be done before scope stuff.

            //// Generating TypeLists
            //var idTypeList = GetHappyTypeList(Identifiers, analyser.SymbolTable);
            //var exprTypeList = GetHappyTypeList(Expressions, analyser.SymbolTable);

            //// Checking for correct id and expr count
            //if (exprTypeList.Count < exprTypeList.Count)
            //    analyser.ErrorReporter.AddError("Decl Ass Error: Too many expressions compared to identifiers");
            //else if (idTypeList.Count > exprTypeList.Count && exprTypeList.Count != 1)
            //    analyser.ErrorReporter.AddError("Decl Ass Error: Incompatible amount of identifiers and expressions.");

            //// Checking for correct types in expr compared to the declaration type
            //if (exprTypeList.Count == 1 && !analyser.IsEqual(exprTypeList[0], Type))
            //    analyser.ErrorReporter.AddError("Type Error: The assignment is not type correct.");
            //else if (!analyser.IsListEqualToType(exprTypeList, Type))
            //    analyser.ErrorReporter.AddError("Type Error: The assignment is not type correct.");

            //// Adding identifiers to symboltable
            //foreach (var id in Identifiers)
            //{
            //    analyser.AddVariableToSymbolTable(id.Name, Type, false, Line, Column);
            //}
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