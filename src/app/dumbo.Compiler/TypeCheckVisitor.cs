using System;
using System.Collections.Generic;
using System.Linq;
using dumbo.Compiler.AST;

namespace dumbo.Compiler
{
    public class TypeCheckVisitor : IVisitor<VisitResult, VisitorArgs>
    {
        private IEventReporter Reporter { get; }
        private VisitResult EmptyResult { get; }
        private BinaryOperationTypeChecker BinaryOperation { get; }

        public TypeCheckVisitor(IEventReporter reporter)
        {
            Reporter = reporter;
            EmptyResult = new VisitResult();
            BinaryOperation = new BinaryOperationTypeChecker();
        }

        public VisitResult Visit(ActualParamListNode node, VisitorArgs arg)
        {
            foreach (var item in node)
                item.Accept(this, arg);

            return EmptyResult;
        }

        public VisitResult Visit(AssignmentStmtNode node, VisitorArgs arg)
        {
            if (node.Identifiers.Count == 1 && node.Expressions.Count == 1)
            {
                var id = node.Identifiers.First();
                var expr = node.Expressions.First();

                var idRes = GetVisitResult(id, arg);
                var exprRes = GetVisitResult(expr, arg);

                if (!idRes.Equals(exprRes))
                {
                    if (exprRes.Types.Any())
                    {
                        var exprType = exprRes.Types.First();
                        Reporter.Error($"The variable '{id.Name}' cannot be assigned the type {exprType}.", expr.SourcePosition);
                    }
                    else
                    {
                        Reporter.Error($"Expression does not return any value.", expr.SourcePosition);
                    }
                }
            }
            else if (node.Expressions.Count == 1)
            {
                var expr = node.Expressions.First();
                var exprResult = GetVisitResult(expr, arg);
                var exprTypes = exprResult.Types.ToList();
                var idList = node.Identifiers.ToList();

                if (idList.Count == exprTypes.Count)
                {
                    for (int i = 0; i < exprTypes.Count; i++)
                    {
                        var exprType = exprTypes[i];
                        var id = idList[i];
                        var idResult = GetVisitResult(idList[i], arg);
                        var idType = idResult.Types.First();
                        if (idType != exprType)
                        {
                            Reporter.Error($"The variable '{id.Name}' cannot be assigned the type {exprType}.",
                                expr.SourcePosition);
                        }
                    }
                }
                else
                {

                    Reporter.Error($"The expression returns {exprTypes.Count} values, but assignment requires {idList.Count} values.",
                        node.Expressions.SourcePosition);
                }
            }
            else if (node.Expressions.Count > 1)
            {
                Reporter.Error("Multi variable assignment is not allowed.", node.Expressions.SourcePosition);
            }
            else if (node.Identifiers.Count > 1)
            {
                Reporter.Error("Multi variable assignment is not allowed.", node.Identifiers.SourcePosition);
            }

            return EmptyResult;
        }

        public VisitResult Visit(BinaryOperationNode node, VisitorArgs arg)
        {
            var lr = GetVisitResult(node.LeftOperand, arg);
            var rr = GetVisitResult(node.RightOperand, arg);

            if (lr.Types.Count() > 1)
            {
                Reporter.Error("Left operand returns too many values.", node.LeftOperand.SourcePosition);
                return new TypeCheckVisitResult(HappyType.Error);
            }

            if (rr.Types.Count() > 1)
            {
                Reporter.Error("Right operand returns too many values.", node.RightOperand.SourcePosition);
                return new TypeCheckVisitResult(HappyType.Error);
            }

            var leftType = lr.Types.First();
            var rightType = rr.Types.First();
            var resultType = BinaryOperation.GetInferredType(node.Operator, leftType, rightType);

            if (resultType == HappyType.Error)
            {
                Reporter.Error($"The types {leftType} and {rightType} are not compatable.", node.SourcePosition);
            }

            return new TypeCheckVisitResult(resultType);
        }

        public VisitResult Visit(BreakStmtNode node, VisitorArgs arg)
        {
            return EmptyResult;
        }

        public VisitResult Visit(DeclAndAssignmentStmtNode node, VisitorArgs arg)
        {
            if (node.Expressions.Count > 1)
            {
                Reporter.Error($"Multi assignment is not allowed.", node.Expressions.SourcePosition);
            }
            else if (node.Expressions.Count == 1)
            {
                var expr = node.Expressions.First();
                var exprResult = GetVisitResult(expr, arg);
                var exprTypes = exprResult.Types.ToList();
                var idList = node.Identifiers.ToList();
                if (exprTypes.Count == idList.Count)
                {
                    for (int i = 0; i < exprTypes.Count; i++)
                    {
                        var id = idList[i];
                        if (exprTypes[i] != node.Type)
                        {
                            Reporter.Error($"Variable '{id.Name}' has a different type than the expression on the left hand side.",
                                id.SourcePosition);
                        }
                    }
                }
                else if (exprTypes.Count == 0)
                {
                    Reporter.Error($"Expression does not return any value.", expr.SourcePosition);
                }
                else
                {
                    Reporter.Error($"The expression returns {exprTypes.Count} values, but assignment requires {idList.Count} values.",
                        node.Expressions.SourcePosition);
                }

            }
            return EmptyResult;
        }

        public VisitResult Visit(DeclStmtNode node, VisitorArgs arg)
        {
            return EmptyResult;
        }

        public VisitResult Visit(ElseIfStmtListNode node, VisitorArgs arg)
        {
            foreach (var item in node)
                item.Accept(this, arg);

            return EmptyResult;
        }

        public VisitResult Visit(ElseIfStmtNode node, VisitorArgs arg)
        {
            EnsureCorrectType(node.Predicate, HappyType.Boolean, arg);

            node.Body.Accept(this, arg);

            return EmptyResult;
        }

        public VisitResult Visit(ExpressionListNode node, VisitorArgs arg)
        {
            var list = new List<HappyType>();

            foreach (var item in node)
            {
                var result = GetVisitResult(item, arg);
                list.AddRange(result.Types);
            }

            return new TypeCheckVisitResult(list);
        }

        public VisitResult Visit(FormalParamListNode node, VisitorArgs arg)
        {
            return EmptyResult;
        }

        public VisitResult Visit(FormalParamNode node, VisitorArgs arg)
        {
            return EmptyResult;
        }

        public VisitResult Visit(FuncCallExprNode node, VisitorArgs arg)
        {
            if (node.DeclarationNode == null)
                return new TypeCheckVisitResult(HappyType.Error);

            node.Parameters.Accept(this, arg);

            if (node.DeclarationNode.Parameters.Count != node.Parameters.Count)
            {
                Reporter.Error("The number of actual parameters does not correspond to number of formal parameters.",
                    node.SourcePosition);
            }
            else
            {
                for (int i = 0; i < node.Parameters.Count; i++)
                {
                    var actual = GetVisitResult(node.Parameters[i], arg).Types.First();
                    var formal = node.DeclarationNode.Parameters[i].Type;
                    if (actual != formal)
                    {
                        Reporter.Error($"The actual parameter {i + 1} does not match the formal parameter.", node.Parameters[i].SourcePosition);
                    }
                }
            }

            return new TypeCheckVisitResult(node.DeclarationNode.ReturnTypes);
        }

        public VisitResult Visit(FuncCallStmtNode node, VisitorArgs arg)
        {
            var result = node.CallNode.Accept(this, arg) as TypeCheckVisitResult;
            if (result.Types.Any())
            {
                string funcName = node.CallNode.FuncName;
                Reporter.Error($"Cannot use function '{funcName}' as a statement. The function should return 'Nothing'.", node.CallNode.SourcePosition);
            }

            return EmptyResult;
        }

        public VisitResult Visit(FuncDeclListNode node, VisitorArgs arg)
        {
            foreach (var item in node)
                item.Accept(this, arg);

            return EmptyResult;
        }

        public VisitResult Visit(FuncDeclNode node, VisitorArgs arg)
        {
            node.Body.Accept(this, arg);

            var returnStmtNodes = node.Body.FindDescendants<ReturnStmtNode>();

            if (!returnStmtNodes.Any() && node.ReturnTypes.Count > 0)
            {
                Reporter.Error($"There are no 'Return' statements in the function '{node.Name}'.", node.SourcePosition);
            }

            foreach (var retStmt in returnStmtNodes)
            {
                var retStmtTypes = GetVisitResult(retStmt.Expressions, arg).Types.ToList();
                if (retStmtTypes.Count != node.ReturnTypes.Count)
                {
                    Reporter.Error("Number of return values does not correspond to number of declared return types.",
                        retStmt.SourcePosition);
                }
                else
                {
                    for (int i = 0; i < node.ReturnTypes.Count; i++)
                    {
                        if (retStmtTypes[i] != node.ReturnTypes[i])
                        {
                            Reporter.Error($"Return value {i + 1} is not compatible with the declared return type.",
                                retStmt.SourcePosition);
                        }
                    }
                }
            }

            return EmptyResult;
        }

        public VisitResult Visit(IdentifierListNode node, VisitorArgs arg)
        {
            var list = new List<HappyType>();
            // TODO Improve on code dublication. Similar code can be found in 
            // public VisitResult Visit(ExpressionListNode node, VisitorArgs arg)
            foreach (var item in node)
            {
                var result = GetVisitResult(item, arg);
                list.AddRange(result.Types);
            }

            return new TypeCheckVisitResult(list);
        }

        public VisitResult Visit(IdentifierNode node, VisitorArgs arg)
        {
            HappyType type = node.DeclarationNode?.Type ?? HappyType.Error;
            return new TypeCheckVisitResult(type);
        }

        public VisitResult Visit(IfElseStmtNode node, VisitorArgs arg)
        {
            EnsureCorrectType(node.Predicate, HappyType.Boolean, arg);

            node.Body.Accept(this, arg);
            node.ElseIfStatements.Accept(this, arg);
            node.Else.Accept(this, arg);

            return EmptyResult;
        }

        public VisitResult Visit(IfStmtNode node, VisitorArgs arg)
        {
            EnsureCorrectType(node.Predicate, HappyType.Boolean, arg);

            node.Body.Accept(this, arg);
            node.ElseIfStatements.Accept(this, arg);

            return EmptyResult;
        }

        public VisitResult Visit(LiteralValueNode node, VisitorArgs arg)
        {
            return new TypeCheckVisitResult(node.Type);
        }

        public VisitResult Visit(ProgramNode node, VisitorArgs arg)
        {
            node.Body.Accept(this, arg);

            var returnStmts = node.Body.FindDescendants<ReturnStmtNode>();
            foreach (var returnStmt in returnStmts)
            {
                Reporter.Error("Program cannot return values.", returnStmt.SourcePosition);
            }

            return EmptyResult;
        }

        public VisitResult Visit(RepeatStmtNode node, VisitorArgs arg)
        {
            EnsureCorrectType(node.Number, HappyType.Number, arg);

            node.Body.Accept(this, arg);

            return EmptyResult;
        }

        public VisitResult Visit(RepeatWhileStmtNode node, VisitorArgs arg)
        {
            EnsureCorrectType(node.Predicate, HappyType.Boolean, arg);

            node.Body.Accept(this, arg);

            return EmptyResult;
        }

        public VisitResult Visit(ReturnStmtNode node, VisitorArgs arg)
        {
            node.Expressions.Accept(this, arg);

            return EmptyResult;
        }

        public VisitResult Visit(RootNode node, VisitorArgs arg)
        {
            node.FuncDecls.Accept(this, arg);
            node.Program.Accept(this, arg);

            return EmptyResult;
        }

        public VisitResult Visit(StmtBlockNode node, VisitorArgs arg)
        {
            foreach (var item in node)
                item.Accept(this, arg);

            return EmptyResult;
        }

        public VisitResult Visit(UnaryOperationNode node, VisitorArgs arg)
        {
            var exprRes = GetVisitResult(node.Expression, arg);
            if (exprRes.Types.Count() != 1)
            {
                Reporter.Error("Expression must return a single value.", node.Expression.SourcePosition);
                return new TypeCheckVisitResult(HappyType.Error);
            }
            var exprType = exprRes.Types.First();
            switch (node.Operator)
            {
                case UnaryOperatorType.Not:
                    if (exprType != HappyType.Boolean)
                    {
                        Reporter.Error("The 'Not' operator is only applicable for Boolean expressions.", node.Expression.SourcePosition);
                        return new TypeCheckVisitResult(HappyType.Error);
                    }
                    break;
                case UnaryOperatorType.Minus:
                    if (exprType != HappyType.Number)
                    {
                        Reporter.Error("The '-' operator is only applicable for numberic expressions.", node.Expression.SourcePosition);
                        return new TypeCheckVisitResult(HappyType.Error);
                    }
                    break;
                case UnaryOperatorType.Plus:
                    if (exprType != HappyType.Number)
                    {
                        Reporter.Error("The '+' operator is only applicable for numberic expressions.", node.Expression.SourcePosition);
                        return new TypeCheckVisitResult(HappyType.Error);
                    }
                    break;
                default:
                    throw new InvalidOperationException($"The unary operator '{node.Operator}' is unknown.");
            }

            return new TypeCheckVisitResult(exprType);
        }

        private TypeCheckVisitResult GetVisitResult(BaseNode node, VisitorArgs args)
        {
            var result = node.Accept(this, args) as TypeCheckVisitResult;
            if (result == null)
                throw new InvalidOperationException($"Visiting {node.GetType().Name} object should always return a TypeCheckVisitResult object.");
            return result;
        }

        private void EnsureCorrectType(ExpressionNode expr, HappyType type, VisitorArgs arg)
        {
            var exprRes = GetVisitResult(expr, arg);
            if (exprRes.Types.Count() != 1 || exprRes.Types.First() != type)
            {
                Reporter.Error($"Expression must be of type {type}.", expr.SourcePosition);
            }
        }
    }
}