using System;
using System.Collections.Generic;
using System.Text;
using dumbo.Compiler.CCAnalysis;
using dumbo.Compiler.PrettyPrint;
using dumbo.Compiler.SymbolTable;

namespace dumbo.Compiler.AST
{
    public class FuncDeclNode : BaseNode
    {
        public FuncDeclNode(IdentifierNode identifer, StmtBlockNode body)
        {
            Identifer = identifer;
            Body = body;
            ReturnTypes = new List<HappyType>();
            Parameters = new FormalParamListNode();
        }

        public FormalParamListNode Parameters { get; }
        public IdentifierNode Identifer { get; }
        public IList<HappyType> ReturnTypes { get; }
        public StmtBlockNode Body { get; }

        public override void PrettyPrint(IPrettyPrinter prettyPrinter)
        {
            //Declares the Function + its name
            prettyPrinter.EndLine();
            prettyPrinter.EndLine();
            prettyPrinter.Append("Function " + Identifer.Name + "(");
            Parameters.PrettyPrint(prettyPrinter);
            prettyPrinter.Append(") Returns ");

            //Declare the return types of the function decl
            if (ReturnTypes.Count == 0)
                prettyPrinter.Append("Nothing");
            else if (ReturnTypes.Count != 1)
            {
                prettyPrinter.Append(ReturnTypes[0].ToString());

                for (int i = 1; i < ReturnTypes.Count; i++)
                {
                    prettyPrinter.Append(", " + ReturnTypes[i].ToString());
                }
            }
            else
                prettyPrinter.Append(ReturnTypes[0].ToString());

            prettyPrinter.EndLine();

            Body.PrettyPrint(prettyPrinter);

            //Finish the function with end
            prettyPrinter.EndLine("End Function");

        }


        public override void CCAnalyse(ICCAnalyser analyser)
        {
            analyser.SymbolTable.OpenScope();

            AddParametersToSymbolTable(analyser);
            Body.CCAnalyse(analyser);
            CheckGlobalParameters(analyser);
            CheckReturnTypes(analyser);

            analyser.SymbolTable.CloseScope();
        }


        private void AddParametersToSymbolTable(ICCAnalyser analyser)
        {
            foreach (var param in Parameters)
            {
                analyser.AddVariableToSymbolTable(param.Name,param.Type,true,param.Line,param.Column);
            }
        }

        private void CheckGlobalParameters(ICCAnalyser analyser)
        {
            var parameters = ConvertParametersToHappyType();
            int numberOfNothing = 0;

            foreach (var parameter in parameters)
            {
                if (parameter.Equals(HappyType.Nothing))
                    numberOfNothing++;
            }

            if (numberOfNothing == 0 && Parameters.Count > 0)
                return;
            if (numberOfNothing == 1 && Parameters.Count == 1)
                return;
            if(numberOfNothing > 1)
            {
                analyser.ErrorReporter.AddError("Function cannot contain multiple Nothing as return");
                return;
            }
        }


        private void CheckReturnTypes(ICCAnalyser analyser)
        {
            var returnStms = GetReturnStmtNodes();
            var parameters = ConvertParametersToHappyType();

            if (ContaintsNothing(parameters))
                CheckNothingReturnTypeCorectness(returnStms, analyser);
            else
                CheckOneOrMoreReturnTypeCorectness(returnStms, analyser);
        }


        private void CheckNothingReturnTypeCorectness(IList<ReturnStmtNode> returnStms, ICCAnalyser analyser)
        {
            foreach (var retStmt in returnStms)
            {
                if (retStmt.GeteReturnTypes(analyser.SymbolTable).GetNumberOfTypes() != 0)
                    analyser.ErrorReporter.AddError("Function has return type (Nothing+maybe more) but return x,y has type xxxx");
            }
        }


        private void CheckOneOrMoreReturnTypeCorectness(IList<ReturnStmtNode> returnStms, ICCAnalyser analyser)
        {
            var parametersAsHappyType = ConvertParametersToHappyType();

            foreach (var retStmt in returnStms)
            {
                var retStmtHappyTypes = retStmt.GeteReturnTypes(analyser.SymbolTable).GetAsList();

                if (!analyser.IsListsEqual(parametersAsHappyType, retStmtHappyTypes))
                    analyser.ErrorReporter.AddError("Function has return types yyy but return x,y has types xxxx");
            }
        }


        private IList<ReturnStmtNode> GetReturnStmtNodes()
        {
            var returnStms = new List<ReturnStmtNode>();

            foreach (var stmt in Body)
            {
                if (stmt is ReturnStmtNode)
                    returnStms.Add(((ReturnStmtNode)stmt));
            }

            return returnStms;
        }


        private IList<HappyType> ConvertParametersToHappyType()
        {
            var retList = new List<HappyType>();

            foreach (var parameter in Parameters)
            {
                retList.Add(parameter.Type);
            }

            return retList;
        }

        private bool ContaintsNothing(IList<HappyType> list)
        {
            foreach (var type in list)
            {
                if (type.Equals(HappyType.Nothing))
                    return true;
            }
            return false;
        }

    }
}