using System.Collections.Generic;
using System.Diagnostics;
using dumbo.Compiler.AST;
using GOLD;

namespace dumbo.Compiler.SyntaxAnalysis
{
    internal class AbstractSyntaxTreeBuilder
    {
        internal RootNode Build(Reduction root)
        {
            Debug.Assert(root.Parent.Head().Name() == "Start");

            ProgramNode programNode = BuildProgram(root[0]);
            IList<FunctionDeclarationNode> funcDelcs;

            if (root.Count() == 3)
                funcDelcs = BuildFunctionDecls(root[2]);
            else
                funcDelcs = new List<FunctionDeclarationNode>();

            return new RootNode(programNode, funcDelcs);
        }

        private ProgramNode BuildProgram(Token programSymbol)
        {
            Debug.Assert(programSymbol.Parent.Name() == "Program");
            var lhs = (Reduction)programSymbol.Data;

            Debug.Assert(lhs.Count() == 4);

            return null;
        }

        private IList<FunctionDeclarationNode> BuildFunctionDecls(Token funcDeclsSymbol)
        {
            Debug.Assert(funcDeclsSymbol.Parent.Name() == "FuncDecls");

            var lhs = (Reduction)funcDeclsSymbol.Data;

            return null;
        }
    }
}