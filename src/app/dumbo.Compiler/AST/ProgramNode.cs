﻿using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using dumbo.Compiler.CCAnalysis;
using dumbo.Compiler.PrettyPrint;
using dumbo.Compiler.SymbolTable;

namespace dumbo.Compiler.AST
{
    public class ProgramNode : BaseNode, IHaveBlocks
    {
        public ProgramNode(StmtBlockNode body)
        {
            Body = body;
        }

        public StmtBlockNode Body { get; }

        public override void PrettyPrint(IPrettyPrinter prettyPrinter)
        {
            prettyPrinter.EndLine("Program");
            Body.PrettyPrint(prettyPrinter);
            prettyPrinter.EndLine("End Program");
        }

        public override void CCAnalyse(ICCAnalyser analyser)
        {
            Body.CCAnalyse(analyser);
        }
        
        public IEnumerable<StmtBlockNode> GetBlocks()
        {
            yield return Body;
        }
    }

}