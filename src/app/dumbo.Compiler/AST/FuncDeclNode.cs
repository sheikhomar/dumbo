using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using dumbo.Compiler.CCAnalysis;
using dumbo.Compiler.SymbolTable;

namespace dumbo.Compiler.AST
{
    public class FuncDeclNode : BaseNode, IHaveBlocks
    {
        public FuncDeclNode(string name, StmtBlockNode body)
        {
            Name = name;
            Body = body;
            ReturnTypes = new List<HappyType>();
            Parameters = new FormalParamListNode();
        }

        public FormalParamListNode Parameters { get; }
        public IList<HappyType> ReturnTypes { get; }
        public string Name { get; set; }
        public StmtBlockNode Body { get; }

        public override void CCAnalyse(ICCAnalyser analyser)
        {
            analyser.SymbolTable.OpenScope();

            AddParametersToSymbolTable(analyser);

            // NOTE! We don't call Body.CCAnalyse(analyser) because
            // StmtBlockNode opens and closes its own scope.
            foreach (var innerStmt in Body)
                innerStmt.CCAnalyse(analyser);

            CheckGlobalParameters(analyser);
            CheckReturnTypes(analyser);

            EnsureCorrectReturnCalls(analyser);

            analyser.SymbolTable.CloseScope();
        }

        private void EnsureCorrectReturnCalls(ICCAnalyser analyser)
        {
            var returnStmts = Body.FindDescendants<ReturnStmtNode>().ToList();
            if (ReturnTypes.Count > 0 && returnStmts.Count == 0)
            {
                analyser.ErrorReporter.AddError(
                    new CCError($"There are no return statement in the function.", Line, Column));
            }
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
            foreach (var retStmt in returnStms)
            {
                var actualRetTypes = retStmt.GeteReturnTypes(analyser.SymbolTable).Types.ToList();

                if (ReturnTypes.Count == actualRetTypes.Count)
                {
                    for (int i = 0; i < ReturnTypes.Count; i++)
                    {
                        if (ReturnTypes[i] != actualRetTypes[i])
                        {
                            string message = $"Function should return type {ReturnTypes[i]} in {i+1} position, but statement returns type {actualRetTypes[i]}.";
                            analyser.ErrorReporter.AddError(new CCError(message, retStmt.Line, retStmt.Column));
                        }
                    }
                }
                else
                {
                    if (ReturnTypes.Count != 0 || actualRetTypes.Count != 1 || actualRetTypes[0] != HappyType.Nothing)
                    {
                        string message =
                            $"Function should return {ReturnTypes.Count} types, but statement returns {actualRetTypes.Count} types.";
                        analyser.ErrorReporter.AddError(new CCError(message, retStmt.Line, retStmt.Column));
                    }
                }
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
        
        public IEnumerable<StmtBlockNode> GetBlocks()
        {
            yield return Body;
        }

        public override VisitResult Accept(IVisitor visitor, VisitorArgs arg)
        {
            return visitor.Visit(this, arg);
        }
    }
}