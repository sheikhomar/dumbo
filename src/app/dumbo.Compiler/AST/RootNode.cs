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

        public RootNode(ProgramNode program)
        {
            Program = program;
            FuncDecls = new FuncDeclListNode();
        }

        public override void PrettyPrint(IPrettyPrinter prettyPrinter)
        {
            Program.PrettyPrint(prettyPrinter);
            FuncDecls.PrettyPrint(prettyPrinter);
        }


        public override void CCAnalyse(ICCAnalyser analyser)
        {
            //Function in table do here
            var FuncDeclList = FuncDecls.GetAllAs<FuncDeclNode>();
            AddFunctionsToTable(FuncDeclList, analyser);

            //Program visit
            Program.CCAnalyse(analyser);

            //Function body visit
            foreach (var function in FuncDeclList)
            {
                function.CCAnalyse(analyser);
            }

        }

        #region  CCAnalyseHelper
        private void AddFunctionsToTable(IList<FuncDeclNode> funcList, ICCAnalyser analyser)
        {
            foreach (var funcDecl in funcList)
            {
                analyser.SymbolTable.EnterSymbol(funcDecl.Identifer.Name, new SymbolTableFunctionType(parameterTypesMaker(funcDecl.Parameters), funcDecl.ReturnTypes), true);
            }
        }

        private IList<HappyType> parameterTypesMaker(FormalParamListNode parameters)
        {
            var paralist = parameters.GetAllAs<FormalParamNode>();
            var retList = new List<HappyType>();

            foreach (var parameter in paralist)
            {
                retList.Add(parameter.Type);
            }

            return retList;
        }
        #endregion
    }
}