using dumbo.Compiler.AST;
using dumbo.Compiler.SymbolTable;

namespace dumbo.Compiler
{
    public class ScopeCheckVisitor : IVisitor
    {
        private readonly VisitResult _emptyResult;

        public ScopeCheckVisitor(IEventReporter reporter)
        {
            _emptyResult = new VisitResult();
            SymbolTable = new SymbolTable.SymbolTable();
            Reporter = reporter;
        }

        public IEventReporter Reporter { get; }
        public SymbolTable.SymbolTable SymbolTable { get; }

        public VisitResult Visit(ActualParamListNode node, VisitorArgs arg)
        {
            foreach (var item in node)
                item.Accept(this, arg);

            return _emptyResult;
        }

        public VisitResult Visit(AssignmentStmtNode node, VisitorArgs arg)
        {
            node.Identifiers.Accept(this, arg);
            node.Expressions.Accept(this, arg);

            return _emptyResult;
        }

        public VisitResult Visit(BinaryOperationNode node, VisitorArgs arg)
        {
            node.LeftOperand.Accept(this, arg);
            node.RightOperand.Accept(this, arg);
            
            return _emptyResult;
        }

        public VisitResult Visit(BreakStmtNode node, VisitorArgs arg)
        {
            return _emptyResult;
        }

        public VisitResult Visit(DeclAndAssignmentStmtNode node, VisitorArgs arg)
        {
            foreach (var id in node.Identifiers)
                AddVariableToSymbolTable(id, node);

            return _emptyResult;
        }

        public VisitResult Visit(DeclStmtNode node, VisitorArgs arg)
        {
            foreach (var id in node.Identifiers)
                AddVariableToSymbolTable(id, node);

            return _emptyResult;
        }

        public VisitResult Visit(ElseIfStmtListNode node, VisitorArgs arg)
        {
            foreach (var item in node)
                item.Accept(this, arg);

            return _emptyResult;
        }

        public VisitResult Visit(ElseIfStmtNode node, VisitorArgs arg)
        {
            node.Predicate.Accept(this, arg);

            SymbolTable.OpenScope();
            node.Body.Accept(this, arg);
            SymbolTable.CloseScope();

            return _emptyResult;
        }

        public VisitResult Visit(ExpressionListNode node, VisitorArgs arg)
        {
            foreach (var item in node)
                item.Accept(this, arg);

            return _emptyResult;
        }
        
        public VisitResult Visit(FormalParamListNode node, VisitorArgs arg)
        {
            foreach (var item in node)
                item.Accept(this, arg);

            return _emptyResult;
        }

        public VisitResult Visit(FormalParamNode node, VisitorArgs arg)
        {
            if (SymbolTable.DeclaredLocally(node.Name))
            {
                Reporter.Error($"Parameter '{node.Name}' is already declared.", node.SourcePosition);
            }
            else
            {
                SymbolTable.EnterSymbol(node.Name, new SymbolTablePrimitiveType(node), true);
            }

            return _emptyResult;
        }

        public VisitResult Visit(FuncCallExprNode node, VisitorArgs arg)
        {
            var function = SymbolTable.RetrieveSymbol(node.FuncName);
            if (function == null)
            {
                Reporter.Error($"The function '{node.FuncName}' is not declared.", node.SourcePosition);
            }
            else
            {
                var functionType = function.Type as SymbolTableFunctionType;
                if (functionType == null)
                {
                    Reporter.Error($"Variable '{node.FuncName}' cannot be called as a function.", node.SourcePosition);
                }
                else
                {
                    node.DeclarationNode = functionType.DeclarationNode;
                }
            }

            return _emptyResult;
        }

        public VisitResult Visit(FuncCallStmtNode node, VisitorArgs arg)
        {
            node.CallNode.Accept(this, arg);

            return _emptyResult;
        }

        public VisitResult Visit(FuncDeclListNode node, VisitorArgs arg)
        {
            foreach (var item in node)
                item.Accept(this, arg);

            return _emptyResult;
        }

        public VisitResult Visit(FuncDeclNode node, VisitorArgs arg)
        {
            var entry = SymbolTable.RetrieveSymbol(node.Name);
            if (entry != null)
            {
                Reporter.Error($"Function '{node.Name}' is already declared.", node.SourcePosition);
            }
            else
            {
                SymbolTable.EnterSymbol(node.Name, new SymbolTableFunctionType(node), true);
            }

            SymbolTable.OpenScope();

            node.Parameters.Accept(this, arg);
            node.Body.Accept(this, arg);

            SymbolTable.CloseScope();
            
            return _emptyResult;
        }

        public VisitResult Visit(IdentifierListNode node, VisitorArgs arg)
        {
            foreach (var item in node)
                item.Accept(this, arg);

            return _emptyResult;
        }

        public VisitResult Visit(IdentifierNode node, VisitorArgs arg)
        {
            var symbolEntry = SymbolTable.RetrieveSymbol(node.Name);
            if (symbolEntry == null)
            {
                Reporter.Error($"Variable '{node.Name}' is not declared.", node.SourcePosition);
            }
            else
            {
                var primitiveType = symbolEntry.Type as SymbolTablePrimitiveType;
                if (primitiveType == null)
                {
                    Reporter.Error($"The function '{node.Name}' cannot be used as a variable.", node.SourcePosition);
                }
                else
                {
                    node.DeclarationNode = primitiveType.DeclarationNode;
                }
            }

            return _emptyResult;
        }

        public VisitResult Visit(IfElseStmtNode node, VisitorArgs arg)
        {
            node.Predicate.Accept(this, arg);

            SymbolTable.OpenScope();
            node.Body.Accept(this, arg);
            SymbolTable.CloseScope();

            node.ElseIfStatements.Accept(this, arg);

            SymbolTable.OpenScope();
            node.Else.Accept(this, arg);
            SymbolTable.CloseScope();

            return _emptyResult;
        }

        public VisitResult Visit(IfStmtNode node, VisitorArgs arg)
        {
            node.Predicate.Accept(this, arg);

            SymbolTable.OpenScope();
            node.Body.Accept(this, arg);
            SymbolTable.CloseScope();

            node.ElseIfStatements.Accept(this, arg);

            return _emptyResult;
        }

        public VisitResult Visit(LiteralValueNode node, VisitorArgs arg)
        {
            return _emptyResult;
        }

        public VisitResult Visit(ProgramNode node, VisitorArgs arg)
        {
            SymbolTable.OpenScope();
            node.Body.Accept(this, arg);
            SymbolTable.CloseScope();

            return _emptyResult;
        }

        public VisitResult Visit(RepeatStmtNode node, VisitorArgs arg)
        {
            node.Number.Accept(this, arg);

            SymbolTable.OpenScope();
            node.Body.Accept(this, arg);
            SymbolTable.CloseScope();

            return _emptyResult;
        }

        public VisitResult Visit(RepeatWhileStmtNode node, VisitorArgs arg)
        {
            node.Predicate.Accept(this, arg);

            SymbolTable.OpenScope();
            node.Body.Accept(this, arg);
            SymbolTable.CloseScope();

            return _emptyResult;
        }

        public VisitResult Visit(ReturnStmtNode node, VisitorArgs arg)
        {
            node.Expressions.Accept(this, arg);

            return _emptyResult;
        }

        public VisitResult Visit(RootNode node, VisitorArgs arg)
        {
            node.FuncDecls.Accept(this, arg);
            node.Program.Accept(this, arg);

            return _emptyResult;
        }

        public VisitResult Visit(StmtBlockNode node, VisitorArgs arg)
        {
            foreach (var stmt in node)
                stmt.Accept(this, arg);

            return _emptyResult;
        }

        public VisitResult Visit(UnaryOperationNode node, VisitorArgs arg)
        {
            node.Expression.Accept(this, arg);

            return _emptyResult;
        }

        public VisitResult Visit(ValueNode node, VisitorArgs arg)
        {
            return _emptyResult;
        }
        
        private void AddVariableToSymbolTable(IdentifierNode idNode, IVariableDeclNode declNode)
        {
            var name = idNode.Name;

            var entry = SymbolTable.RetrieveSymbol(name);
            if (entry == null)
                SymbolTable.EnterSymbol(name, new SymbolTablePrimitiveType(declNode));
            else if (entry.IsUnhideable == false)
            {
                if (entry.Depth == SymbolTable.Depth)
                    Reporter.Error($"The variable '{name}' is already declared in this scope.", idNode.SourcePosition);
                else
                    SymbolTable.EnterSymbol(name, new SymbolTablePrimitiveType(declNode));
            }
            else if (entry.IsUnhideable == true)
                Reporter.Error($"The variable '{name}' is already declared in this scope.", idNode.SourcePosition);
        }
    }
}