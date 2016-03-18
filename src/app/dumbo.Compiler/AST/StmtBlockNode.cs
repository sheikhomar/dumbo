﻿using System.Text;
using dumbo.Compiler.CCAnalysis;
using dumbo.Compiler.PrettyPrint;
using dumbo.Compiler.SymbolTable;

namespace dumbo.Compiler.AST
{
    public class StmtBlockNode  : BaseListNode<StmtNode>
    {
        public override void PrettyPrint(IPrettyPrinter prettyPrinter)
        {
            var newList = GetAllAs<StmtNode>();

            if (newList.Count == 0)
                return;

            prettyPrinter.IndentIncrement(); //Inc. indentation
            foreach (var node in newList)
            {
                node.PrettyPrint(prettyPrinter);
            }
            prettyPrinter.IndentDecrement(); //Dec. indentation
        }


        public override void CCAnalyse(ICCAnalyser analyser)
        {
            analyser.SymbolTable.OpenScope();
            base.CCAnalyse(analyser);
            analyser.SymbolTable.CloseScope();
        }
    }
}