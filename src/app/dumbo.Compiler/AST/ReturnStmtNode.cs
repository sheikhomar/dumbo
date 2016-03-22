using System.Text;
using dumbo.Compiler.CCAnalysis;
using dumbo.Compiler.PrettyPrint;
using dumbo.Compiler.SymbolTable;

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
            //First type
            var td = new TypeDescriptor(Expressions[0].GetHappyType(symbolTable).GetFirst());

            //Then the rest
            for (int i = 1; i < Expressions.Count; i++)
            {
                var typeDec = Expressions[i].GetHappyType(symbolTable); //What to do if functions can be return type

                //This should not be allowed
                td.Add(typeDec.GetFirst());
            }
            return td;
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

        public override void CCAnalyse(ICCAnalyser analyser)
        {
            if (Expressions.Count == 0)
                return; //We have Nothing as return type
        }
    }
}