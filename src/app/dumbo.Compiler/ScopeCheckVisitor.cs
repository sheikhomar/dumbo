using dumbo.Compiler.AST;
using dumbo.Compiler.SymbolTable;

namespace dumbo.Compiler
{
    public class ScopeCheckVisitor : IVisitor<object, VisitorArgs>
    {
        public ScopeCheckVisitor(IEventReporter reporter)
        {
            SymbolTable = new SymbolTable.SymbolTable();
            Reporter = reporter;
        }

        public IEventReporter Reporter { get; }
        public SymbolTable.SymbolTable SymbolTable { get; }

        public object Visit(ActualParamListNode node, VisitorArgs arg)
        {
            foreach (var item in node)
                item.Accept(this, arg);

            return null;
        }

        public object Visit(AssignmentStmtNode node, VisitorArgs arg)
        {
            node.Identifiers.Accept(this, arg);
            node.Expressions.Accept(this, arg);

            return null;
        }

        public object Visit(BinaryOperationNode node, VisitorArgs arg)
        {
            node.LeftOperand.Accept(this, arg);
            node.RightOperand.Accept(this, arg);

            return null;
        }

        public object Visit(BreakStmtNode node, VisitorArgs arg)
        {
            return null;
        }

        public object Visit(BuiltInFuncDeclNode node, VisitorArgs arg)
        {
            return null;
        }

        public object Visit(DeclAndAssignmentStmtNode node, VisitorArgs arg)
        {
            foreach (var id in node.Identifiers)
                AddVariableToSymbolTable(id, node);

            node.Expressions.Accept(this, arg);
            node.Identifiers.Accept(this, arg);

            return null;
        }

        public object Visit(DeclStmtNode node, VisitorArgs arg)
        {
            foreach (var id in node.Identifiers)
                AddVariableToSymbolTable(id, node);

            node.Identifiers.Accept(this, arg);

            return null;
        }

        public object Visit(ElseIfStmtListNode node, VisitorArgs arg)
        {
            foreach (var item in node)
                item.Accept(this, arg);

            return null;
        }

        public object Visit(ElseIfStmtNode node, VisitorArgs arg)
        {
            node.Predicate.Accept(this, arg);

            SymbolTable.OpenScope();
            node.Body.Accept(this, arg);
            SymbolTable.CloseScope();

            return null;
        }

        public object Visit(ExpressionListNode node, VisitorArgs arg)
        {
            foreach (var item in node)
                item.Accept(this, arg);

            return null;
        }

        public object Visit(FormalParamListNode node, VisitorArgs arg)
        {
            foreach (var item in node)
                item.Accept(this, arg);

            return null;
        }

        public object Visit(FormalParamNode node, VisitorArgs arg)
        {
            var entry = SymbolTable.RetrieveSymbol(node.Name);
            if (entry == null)
            {
                SymbolTable.EnterSymbol(node.Name, new SymbolTablePrimitiveType(node), true);
            }
            else if (SymbolTable.DeclaredLocally(node.Name))
            {
                Reporter.Error($"Parameter '{node.Name}' is already declared.", node.SourcePosition);
            }
            else
            {
                Reporter.Error($"Identifier '{node.Name}' cannot be used as parameter since it is already used as function name.", node.SourcePosition);
            }

            return null;
        }

        public object Visit(FuncCallExprNode node, VisitorArgs arg)
        {
            node.Parameters.Accept(this, arg);
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

            return null;
        }

        public object Visit(FuncCallStmtNode node, VisitorArgs arg)
        {
            node.CallNode.Accept(this, arg);

            return null;
        }

        public object Visit(FuncDeclListNode node, VisitorArgs arg)
        {
            foreach (var item in node)
            {
                var entry = SymbolTable.RetrieveSymbol(item.Name);
                if (entry != null)
                {
                    Reporter.Error($"Function '{item.Name}' is already declared.", node.SourcePosition);
                }
                else
                {
                    SymbolTable.EnterSymbol(item.Name, new SymbolTableFunctionType(item), true);
                }
            }

            foreach (var item in node)
                item.Accept(this, arg);

            return null;
        }

        public object Visit(FuncDeclNode node, VisitorArgs arg)
        {
            SymbolTable.OpenScope();

            node.Parameters.Accept(this, arg);
            node.Body.Accept(this, arg);

            SymbolTable.CloseScope();

            return null;
        }

        public object Visit(IdentifierListNode node, VisitorArgs arg)
        {
            foreach (var item in node)
                item.Accept(this, arg);

            return null;
        }

        public object Visit(IdentifierNode node, VisitorArgs arg)
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

            return null;
        }

        public object Visit(IfElseStmtNode node, VisitorArgs arg)
        {
            node.Predicate.Accept(this, arg);

            SymbolTable.OpenScope();
            node.Body.Accept(this, arg);
            SymbolTable.CloseScope();

            node.ElseIfStatements.Accept(this, arg);

            SymbolTable.OpenScope();
            node.Else.Accept(this, arg);
            SymbolTable.CloseScope();

            return null;
        }

        public object Visit(IfStmtNode node, VisitorArgs arg)
        {
            node.Predicate.Accept(this, arg);

            SymbolTable.OpenScope();
            node.Body.Accept(this, arg);
            SymbolTable.CloseScope();

            node.ElseIfStatements.Accept(this, arg);

            return null;
        }

        public object Visit(LiteralValueNode node, VisitorArgs arg)
        {
            return null;
        }

        public object Visit(PrimitiveTypeNode node, VisitorArgs arg)
        {
            return null;
        }

        public object Visit(ProgramNode node, VisitorArgs arg)
        {
            SymbolTable.OpenScope();
            node.Body.Accept(this, arg);
            SymbolTable.CloseScope();

            return null;
        }

        public object Visit(RepeatStmtNode node, VisitorArgs arg)
        {
            node.Number.Accept(this, arg);

            SymbolTable.OpenScope();
            node.Body.Accept(this, arg);
            SymbolTable.CloseScope();

            return null;
        }

        public object Visit(RepeatWhileStmtNode node, VisitorArgs arg)
        {
            node.Predicate.Accept(this, arg);

            SymbolTable.OpenScope();
            node.Body.Accept(this, arg);
            SymbolTable.CloseScope();

            return null;
        }

        public object Visit(ReturnStmtNode node, VisitorArgs arg)
        {
            node.Expressions.Accept(this, arg);

            return null;
        }

        public object Visit(RootNode node, VisitorArgs arg)
        {
            node.FuncDecls.Accept(this, arg);
            node.Program.Accept(this, arg);

            return null;
        }

        public object Visit(StmtBlockNode node, VisitorArgs arg)
        {
            foreach (var stmt in node)
                stmt.Accept(this, arg);

            return null;
        }

        public object Visit(UnaryOperationNode node, VisitorArgs arg)
        {
            node.Expression.Accept(this, arg);

            return null;
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
