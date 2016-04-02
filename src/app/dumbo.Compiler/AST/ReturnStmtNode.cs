using System.Text;
using dumbo.Compiler.CCAnalysis;
using dumbo.Compiler.PrettyPrint;
using dumbo.Compiler.SymbolTable;
using System.Collections.Generic;

namespace dumbo.Compiler.AST
{
    public class ReturnStmtNode : StmtNode
    {
        public ReturnStmtNode(ExpressionListNode expressions)
        {
            Expressions = expressions ?? new ExpressionListNode();
        }

        public ExpressionListNode Expressions { get; }

        public TypeDescriptor GeteReturnTypes(ISymbolTable symbolTable)
        {
            if (Expressions.Count == 0)
                return new TypeDescriptor(HappyType.Nothing);

            //First type
            var happyTypeList = new List<HappyType>(Expressions[0].GetHappyType(symbolTable).Types);

            //Then the rest
            for (int i = 1; i < Expressions.Count; i++)
            {
                var typeDec = Expressions[i].GetHappyType(symbolTable);
                happyTypeList.AddRange(typeDec.Types);

            }
            return new TypeDescriptor(happyTypeList);
        }

        public override void CCAnalyse(ICCAnalyser analyser)
        {
            Expressions.CCAnalyse(analyser);
        }

        public override void PrettyPrint(IPrettyPrinter prettyPrinter)
        {
            prettyPrinter.Append("Return ");
            if (Expressions.Count == 0)
                prettyPrinter.Append("Nothing");
            else
                Expressions.PrettyPrint(prettyPrinter);
            prettyPrinter.EndLine();
        }

        public override VisitResult Accept(IVisitor visitor, VisitorArgs arg)
        {
            return visitor.Visit(this, arg);
        }
    }
}