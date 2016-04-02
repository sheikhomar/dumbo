using System.Collections.Generic;
using System.Text;
using dumbo.Compiler.CCAnalysis;
using dumbo.Compiler.PrettyPrint;
using dumbo.Compiler.SymbolTable;

namespace dumbo.Compiler.AST
{
    public class RootNode : BaseNode
    {
        public ProgramNode Program { get; set; }
        public FuncDeclListNode FuncDecls { get; }

        public RootNode(ProgramNode program, FuncDeclListNode funcDecls = null)
        {
            Program = program;
            FuncDecls = funcDecls ?? new FuncDeclListNode();
        }

        public override void PrettyPrint(IPrettyPrinter prettyPrinter)
        {
            Program.PrettyPrint(prettyPrinter);
            FuncDecls.PrettyPrint(prettyPrinter);
        }


        public override void CCAnalyse(ICCAnalyser analyser)
        {
            //Function in table do here
            AddFunctionsToTable(FuncDecls, analyser);

            // Add all constants

            //Program visit
            Program.CCAnalyse(analyser);

            //Function body visit
            foreach (var function in FuncDecls)
            {
                function.CCAnalyse(analyser);
            }

        }

        #region  CCAnalyseHelper
        private void AddFunctionsToTable(FuncDeclListNode funcList, ICCAnalyser analyser)
        {
            foreach (var funcDecl in funcList)
            {
                analyser.AddFunctionToSymbolTable(funcDecl.Identifer.Name, parameterTypesMaker(funcDecl.Parameters), funcDecl.ReturnTypes, Line, Column);
            }
        }

        private IList<HappyType> parameterTypesMaker(FormalParamListNode parameters)
        {
            var retList = new List<HappyType>();

            foreach (var parameter in parameters)
            {
                retList.Add(parameter.Type);
            }

            return retList;
        }
        #endregion

        public override VisitResult Accept(IVisitor visitor, VisitorArgs arg)
        {
            return visitor.Visit(this, arg);
        }
    }
}