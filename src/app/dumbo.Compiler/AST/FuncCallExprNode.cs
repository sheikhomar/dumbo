using System;
using System.Text;
using dumbo.Compiler.CCAnalysis;
using dumbo.Compiler.PrettyPrint;
using dumbo.Compiler.SymbolTable;
using System.Collections.Generic;

namespace dumbo.Compiler.AST
{
    public class FuncCallExprNode : ExpressionNode
    {
        public FuncCallExprNode(string identifier)
        {
            Identifier = identifier;
            Parameters = new ActualParamListNode();
        }

        public string Identifier { get; }
        public ActualParamListNode Parameters { get; }

        public override VisitResult Accept(IVisitor visitor, VisitorArgs arg)
        {
            return visitor.Visit(this, arg);
        }
        
        public override TypeDescriptor GetHappyType(ISymbolTable symbolTable)
        {
            var function = symbolTable.RetrieveSymbol(Identifier);

            if (function == null)
            {
                return new TypeDescriptor(HappyType.Error);
            }
            else
            {
                var functionType = function.Type as SymbolTableFunctionType;
                if (functionType == null)
                {
                    return new TypeDescriptor(HappyType.Error);
                }
                else if (functionType.returntypes.Count > 1)
                {
                    var td = new TypeDescriptor(functionType.returntypes[0]);
                    for (int i = 1; i < functionType.returntypes.Count; i++)
                    {
                        td.Add(functionType.returntypes[i]);
                    }
                    return td;
                }
                else if (functionType.returntypes.Count == 0)
                    return new TypeDescriptor(HappyType.Error);
                else
                    return new TypeDescriptor(functionType.returntypes[0]);
            }
        }

        public override void PrettyPrint(IPrettyPrinter prettyPrinter)
        {
            prettyPrinter.Append(Identifier + "(");
            Parameters.PrettyPrint(prettyPrinter);
            prettyPrinter.Append(")");
        }

        public override void CCAnalyse(ICCAnalyser analyser)
        {
            var function = analyser.SymbolTable.RetrieveSymbol(Identifier);

            if (function == null)
            {
                analyser.ErrorReporter.AddError(new CCError("The function is not declared", this.Line, this.Column));
            }
            else
            {
                CheckFormalAndActualParameters(function, analyser);
            }
        }

        public void CheckFormalAndActualParameters(SymbolTableEntry function, ICCAnalyser analyser)
        {
            var functionType = function.Type as SymbolTableFunctionType;
            if (functionType == null)
                analyser.ErrorReporter.AddError(new CCError("Cannot be called as a funtion because it is a variable", this.Line, this.Column));
            else
            {
                if(functionType.parametertypes.Count < Parameters.Count)
                    analyser.ErrorReporter.AddError(new CCError("There is too many actual parameters compared to the number of formal parameters", Line, Column));
                else
                {
                    var actualParamTypeList = new List<HappyType>();
                    foreach (var actualParam in Parameters)
                        actualParamTypeList.AddRange(actualParam.GetHappyType(analyser.SymbolTable).Types);

                    TypeCheckParameters(functionType.parametertypes, actualParamTypeList, analyser);
                }
            }
        }

        private void TypeCheckParameters(IList<HappyType> formalParameters, IList<HappyType> actualParameters, ICCAnalyser analyser)
        {
            if(formalParameters.Count == actualParameters.Count)
            {
                for (int i = 0; i < formalParameters.Count; i++)
                {
                    var formalParam = formalParameters[i];
                    var actualParam = actualParameters[i];

                    if (!actualParam.Equals(formalParam))
                    {
                        analyser.ErrorReporter.AddError(new CCError($"The actual parameter number {i + 1} doesn't match the formal parameter", Line, Column));
                    }
                }
            }
            else
                analyser.ErrorReporter.AddError(new CCError($"The number of actual and formal parameters isn't compatible. There is {formalParameters.Count} formal parameters and {actualParameters.Count} actual parameters", Line, Column));
        }
    }
}