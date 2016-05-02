using dumbo.Compiler.AST;

namespace dumbo.Compiler
{
    public class ReturnCheckVisitor : IVisitor<ReturnCheckResult, VisitorArgs>
    {
        public ReturnCheckVisitor(EventReporter reporter)
        {
            Reporter = reporter;
        }

        public EventReporter Reporter { get; }

        public ReturnCheckResult Visit(ActualParamListNode node, VisitorArgs arg)
        {
            return null;
        }

        public ReturnCheckResult Visit(ArrayDeclStmtNode node, VisitorArgs arg)
        {
            return null;
        }

        public ReturnCheckResult Visit(ArrayIdentifierNode node, VisitorArgs arg)
        {
            return null;
        }

        public ReturnCheckResult Visit(ArrayTypeNode node, VisitorArgs arg)
        {
            return null;
        }

        public ReturnCheckResult Visit(ArrayValueNode node, VisitorArgs arg)
        {
            return null;
        }

        public ReturnCheckResult Visit(AssignmentStmtNode node, VisitorArgs arg)
        {
            return null;
        }

        public ReturnCheckResult Visit(BinaryOperationNode node, VisitorArgs arg)
        {
            return null;
        }

        public ReturnCheckResult Visit(BreakStmtNode node, VisitorArgs arg)
        {
            return null;
        }

        public ReturnCheckResult Visit(ContinueStmtNode node, VisitorArgs arg)
        {
            return null;
        }

        public ReturnCheckResult Visit(ConstDeclListNode node, VisitorArgs arg)
        {
            return null;
        }

        public ReturnCheckResult Visit(ConstDeclNode node, VisitorArgs arg)
        {
            return null;
        }

        public ReturnCheckResult Visit(BuiltInFuncDeclNode node, VisitorArgs arg)
        {
            return null;
        }

        public ReturnCheckResult Visit(DeclAndAssignmentStmtNode node, VisitorArgs arg)
        {
            return null;
        }

        public ReturnCheckResult Visit(PrimitiveDeclStmtNode node, VisitorArgs arg)
        {
            return null;
        }

        public ReturnCheckResult Visit(ElseIfStmtListNode node, VisitorArgs arg)
        {
            int counter = node.Count;

            foreach (var item in node)
            {
                if (item.Accept(this, arg) == null)
                    counter--;
                else if (item.Accept(this, arg).ContainsReturn)
                    counter--;
            }

            return new ReturnCheckResult(counter == 0);
        }

        public ReturnCheckResult Visit(ElseIfStmtNode node, VisitorArgs arg)
        {
            return node.Body.Accept(this, arg);
        }

        public ReturnCheckResult Visit(ExpressionListNode node, VisitorArgs arg)
        {
            return null;
        }

        public ReturnCheckResult Visit(FormalParamListNode node, VisitorArgs arg)
        {
            return null;
        }

        public ReturnCheckResult Visit(FormalParamNode node, VisitorArgs arg)
        {
            return null;
        }

        public ReturnCheckResult Visit(FuncCallExprNode node, VisitorArgs arg)
        {
            node.DeclarationNode.Accept(this, arg);
            return null;
        }

        public ReturnCheckResult Visit(FuncCallStmtNode node, VisitorArgs arg)
        {
            node.CallNode.Accept(this, arg);
            return null;
        }

        public ReturnCheckResult Visit(FuncDeclListNode node, VisitorArgs arg)
        {
            foreach (var item in node)
            {
                item.Accept(this, arg);
            }
            return null;
        }

        public ReturnCheckResult Visit(FuncDeclNode node, VisitorArgs arg)
        {
            if (node.ReturnTypes.Count == 0)
                return null;

            foreach (var stmt in node.Body)
            {
                if (stmt is ReturnStmtNode)
                    return null;
            }

            var containsReturn = node.Body.Accept(this, arg).ContainsReturn;
            if (!containsReturn)
                Reporter.Error($"Not all code paths in function {node.Name} contain a return.", node.SourcePosition);
            return null;
        }

        public ReturnCheckResult Visit(IdentifierListNode node, VisitorArgs arg)
        {
            return null;
        }

        public ReturnCheckResult Visit(IdentifierNode node, VisitorArgs arg)
        {
            return null;
        }

        public ReturnCheckResult Visit(IfElseStmtNode node, VisitorArgs arg)
        {
            var ifResult = node.Body.Accept(this, arg).ContainsReturn;
            var elseIfResult = node.ElseIfStatements.Accept(this, arg).ContainsReturn;
            var elseResult = node.Else.Accept(this, arg).ContainsReturn;

            return new ReturnCheckResult(ifResult && elseIfResult && elseResult);
        }

        public ReturnCheckResult Visit(IfStmtNode node, VisitorArgs arg)
        {
            return new ReturnCheckResult(false);
        }

        public ReturnCheckResult Visit(LiteralValueNode node, VisitorArgs arg)
        {
            return null;
        }

        public ReturnCheckResult Visit(PrimitiveTypeNode node, VisitorArgs arg)
        {
            return null;
        }

        public ReturnCheckResult Visit(ProgramNode node, VisitorArgs arg)
        {
            return null;
        }

        public ReturnCheckResult Visit(RepeatStmtNode node, VisitorArgs arg)
        {
            node.Body.Accept(this, arg);
            return null;
        }

        public ReturnCheckResult Visit(RepeatWhileStmtNode node, VisitorArgs arg)
        {
            node.Body.Accept(this, arg);
            return null;
        }

        public ReturnCheckResult Visit(ReturnStmtNode node, VisitorArgs arg)
        {
            return null;
        }

        public ReturnCheckResult Visit(RootNode node, VisitorArgs arg)
        {
            node.FuncDecls.Accept(this, arg);
            return null;
        }

        public ReturnCheckResult Visit(StmtBlockNode node, VisitorArgs arg)
        {
            int counter = node.Count;
            if (counter == 0)
                return new ReturnCheckResult(false);
            foreach (var item in node)
            {
                var result = item.Accept(this, arg);
                if (result == null)
                    counter--;
                else if (result.ContainsReturn)
                    counter--;
            }

            return new ReturnCheckResult(counter == 0);
        }

        public ReturnCheckResult Visit(UnaryOperationNode node, VisitorArgs arg)
        {
            return null;
        }
    }
}
