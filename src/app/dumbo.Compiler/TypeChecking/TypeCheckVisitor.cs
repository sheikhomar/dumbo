using System;
using System.Collections.Generic;
using System.Linq;
using dumbo.Compiler.AST;

namespace dumbo.Compiler.TypeChecking
{
    public class TypeCheckVisitor : IVisitor<TypeCheckVisitResult, VisitorArgs>
    {
        private IEventReporter Reporter { get; }
        private BinaryOperationTypeChecker BinaryOperation { get; }

        public TypeCheckVisitor(IEventReporter reporter)
        {
            Reporter = reporter;
            BinaryOperation = new BinaryOperationTypeChecker();
        }

        public TypeCheckVisitResult Visit(ActualParamListNode node, VisitorArgs arg)
        {
            foreach (var item in node)
                item.Accept(this, arg);

            return null;
        }

        public TypeCheckVisitResult Visit(ArrayDeclStmtNode node, VisitorArgs arg)
        {
            return null;
        }

        public TypeCheckVisitResult Visit(AssignmentStmtNode node, VisitorArgs arg)
        {
            if (node.Identifiers.Count == 1)
            {
                var id = node.Identifiers.First();
                var expr = node.Value;

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
            else
            {
                var expr = node.Value;
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
                        if (!idType.Equals(exprType))
                        {
                            Reporter.Error($"The variable '{id.Name}' cannot be assigned the type {exprType}.",
                                expr.SourcePosition);
                        }
                    }
                }
            }

            return null;
        }

        public TypeCheckVisitResult Visit(BinaryOperationNode node, VisitorArgs arg)
        {
            var lr = GetVisitResult(node.LeftOperand, arg);
            var rr = GetVisitResult(node.RightOperand, arg);

            if (lr.Types.Count() != 1)
            {
                Reporter.Error("Left operand must return a single value.", node.LeftOperand.SourcePosition);
                return ErrorType();
            }

            if (rr.Types.Count() != 1)
            {
                Reporter.Error("Right operand must return a single value.", node.RightOperand.SourcePosition);
                return ErrorType();
            }

            var leftType = lr.Types.First();
            var rightType = rr.Types.First();
            var resultType = BinaryOperation.GetInferredType(node.Operator, leftType, rightType);

            if (!resultType.Item1)
            {
                Reporter.Error($"The types {leftType} and {rightType} are not compatible.", node.SourcePosition);
            }

            var exprType = new PrimitiveTypeNode(resultType.Item2);

            node.InferredType = new TypeDescriptor(exprType);

            return new TypeCheckVisitResult(exprType);
        }

        public TypeCheckVisitResult Visit(BreakStmtNode node, VisitorArgs arg)
        {
            return null;
        }

        public TypeCheckVisitResult Visit(BuiltInFuncDeclNode node, VisitorArgs arg)
        {
            return null;
        }

        public TypeCheckVisitResult Visit(DeclAndAssignmentStmtNode node, VisitorArgs arg)
        {
            var expr = node.Value;
            var exprResult = GetVisitResult(expr, arg);

            var exprTypes = exprResult.Types.ToList();
            var idList = node.Identifiers.ToList();
            if (exprTypes.Count == idList.Count)
            {
                for (int i = 0; i < exprTypes.Count; i++)
                {
                    var id = idList[i];
                    if (!exprTypes[i].Equals(node.Type))
                    {
                        Reporter.Error(
                            $"Variable '{id.Name}' has a different type than the expression on the left hand side.",
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
                Reporter.Error(
                    $"The expression returns {exprTypes.Count} values, but assignment requires {idList.Count} values.",
                    node.Value.SourcePosition);
            }
            return null;
        }

        public TypeCheckVisitResult Visit(PrimitiveDeclStmtNode node, VisitorArgs arg)
        {
            return null;
        }

        public TypeCheckVisitResult Visit(ElseIfStmtListNode node, VisitorArgs arg)
        {
            foreach (var item in node)
                item.Accept(this, arg);

            return null;
        }

        public TypeCheckVisitResult Visit(ElseIfStmtNode node, VisitorArgs arg)
        {
            EnsureCorrectType(node.Predicate, PrimitiveType.Boolean, arg);

            node.Body.Accept(this, arg);

            return null;
        }

        public TypeCheckVisitResult Visit(ExpressionListNode node, VisitorArgs arg)
        {
            var list = new List<TypeNode>();

            foreach (var item in node)
            {
                var result = GetVisitResult(item, arg);
                list.AddRange(result.Types);
            }

            return new TypeCheckVisitResult(list);
        }

        public TypeCheckVisitResult Visit(FormalParamListNode node, VisitorArgs arg)
        {
            return null;
        }

        public TypeCheckVisitResult Visit(FormalParamNode node, VisitorArgs arg)
        {
            return null;
        }

        public TypeCheckVisitResult Visit(FuncCallExprNode node, VisitorArgs arg)
        {
            if (node.DeclarationNode == null)
                return ErrorType();
            
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
                    var actualParamRes = GetVisitResult(node.Parameters[i], arg);
                    if (actualParamRes.Types.Count() != 1)
                    {
                        Reporter.Error($"The actual parameter {i + 1} does not have correct value.", node.Parameters[i].SourcePosition);
                        continue;
                    }

                    var actual = actualParamRes.Types.First();
                    var formal = node.DeclarationNode.Parameters[i].Type;
                    if (!actual.Equals(formal))
                    {
                        Reporter.Error($"The actual parameter {i + 1} does not match the formal parameter.", node.Parameters[i].SourcePosition);
                    }
                }
            }

            node.InferredType = new TypeDescriptor(node.DeclarationNode.ReturnTypes);

            return new TypeCheckVisitResult(node.DeclarationNode.ReturnTypes);
        }

        public TypeCheckVisitResult Visit(FuncCallStmtNode node, VisitorArgs arg)
        {
            var result = node.CallNode.Accept(this, arg) as TypeCheckVisitResult;
            if (result.Types.Any())
            {
                string funcName = node.CallNode.FuncName;
                Reporter.Error($"Cannot use function '{funcName}' as a statement. The function should return 'Nothing'.", node.CallNode.SourcePosition);
            }

            return null;
        }

        public TypeCheckVisitResult Visit(FuncDeclListNode node, VisitorArgs arg)
        {
            foreach (var item in node)
                item.Accept(this, arg);

            return null;
        }

        public TypeCheckVisitResult Visit(FuncDeclNode node, VisitorArgs arg)
        {
            node.Body.Accept(this, arg);

            var returnStmtNodes = node.Body.FindDescendants<ReturnStmtNode>();

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
                        if (!retStmtTypes[i].Equals(node.ReturnTypes[i]))
                        {
                            Reporter.Error($"Return value {i + 1} is not compatible with the declared return type.",
                                retStmt.SourcePosition);
                        }
                    }
                }
            }

            return null;
        }

        public TypeCheckVisitResult Visit(IdentifierListNode node, VisitorArgs arg)
        {
            var list = new List<TypeNode>();
            // TODO Improve on code dublication. Similar code can be found in 
            // public TypeCheckVisitResult Visit(ExpressionListNode node, VisitorArgs arg)
            foreach (var item in node)
            {
                var result = GetVisitResult(item, arg);
                list.AddRange(result.Types);
            }

            

            return new TypeCheckVisitResult(list);
        }

        public TypeCheckVisitResult Visit(IdentifierNode node, VisitorArgs arg)
        {
            if (node.DeclarationNode == null)
                return ErrorType();

            node.InferredType = new TypeDescriptor(node.DeclarationNode.Type);

            return new TypeCheckVisitResult(node.DeclarationNode.Type);
        }

        public TypeCheckVisitResult Visit(IfElseStmtNode node, VisitorArgs arg)
        {
            EnsureCorrectType(node.Predicate, PrimitiveType.Boolean, arg);

            node.Body.Accept(this, arg);
            node.ElseIfStatements.Accept(this, arg);
            node.Else.Accept(this, arg);

            return null;
        }

        public TypeCheckVisitResult Visit(IfStmtNode node, VisitorArgs arg)
        {
            EnsureCorrectType(node.Predicate, PrimitiveType.Boolean, arg);

            node.Body.Accept(this, arg);
            node.ElseIfStatements.Accept(this, arg);

            return null;
        }

        public TypeCheckVisitResult Visit(LiteralValueNode node, VisitorArgs arg)
        {
            node.InferredType = new TypeDescriptor(node.Type);
            return new TypeCheckVisitResult(node.Type);
        }

        public TypeCheckVisitResult Visit(PrimitiveTypeNode node, VisitorArgs arg)
        {
            return null;
        }

        public TypeCheckVisitResult Visit(ProgramNode node, VisitorArgs arg)
        {
            node.Body.Accept(this, arg);

            var returnStmts = node.Body.FindDescendants<ReturnStmtNode>();
            foreach (var returnStmt in returnStmts)
            {
                Reporter.Error("Program cannot return values.", returnStmt.SourcePosition);
            }

            return null;
        }

        public TypeCheckVisitResult Visit(RepeatStmtNode node, VisitorArgs arg)
        {
            EnsureCorrectType(node.Number, PrimitiveType.Number, arg);

            node.Body.Accept(this, arg);

            return null;
        }

        public TypeCheckVisitResult Visit(RepeatWhileStmtNode node, VisitorArgs arg)
        {
            EnsureCorrectType(node.Predicate, PrimitiveType.Boolean, arg);

            node.Body.Accept(this, arg);

            return null;
        }

        public TypeCheckVisitResult Visit(ReturnStmtNode node, VisitorArgs arg)
        {
            node.Expressions.Accept(this, arg);

            return null;
        }

        public TypeCheckVisitResult Visit(RootNode node, VisitorArgs arg)
        {
            node.FuncDecls.Accept(this, arg);
            node.Program.Accept(this, arg);

            return null;
        }

        public TypeCheckVisitResult Visit(StmtBlockNode node, VisitorArgs arg)
        {
            foreach (var item in node)
                item.Accept(this, arg);

            return null;
        }

        public TypeCheckVisitResult Visit(UnaryOperationNode node, VisitorArgs arg)
        {
            var exprRes = GetVisitResult(node.Expression, arg);
            if (exprRes.Types.Count() != 1)
            {
                Reporter.Error("Expression must return a single value.", node.Expression.SourcePosition);
                return ErrorType();
            }

            var exprType = exprRes.Types.First() as PrimitiveTypeNode;
            if (exprType == null)
            {
                Reporter.Error("Unary operators can only be used with primitive types.", node.Expression.SourcePosition);
                return ErrorType();
            }
            
            switch (node.Operator)
            {
                case UnaryOperatorType.Not:
                    if (exprType.Type != PrimitiveType.Boolean)
                    {
                        Reporter.Error("The 'Not' operator is only applicable for Boolean expressions.",
                            node.Expression.SourcePosition);
                        return ErrorType();
                    }
                    break;

                case UnaryOperatorType.Minus:
                    if (exprType.Type != PrimitiveType.Number)
                    {
                        Reporter.Error("The '-' operator is only applicable for numberic expressions.",
                            node.Expression.SourcePosition);
                        return ErrorType();
                    }
                    break;
                case UnaryOperatorType.Plus:
                    if (exprType.Type != PrimitiveType.Number)
                    {
                        Reporter.Error("The '+' operator is only applicable for numberic expressions.",
                            node.Expression.SourcePosition);
                        return ErrorType();
                    }
                    break;
                default:
                    throw new InvalidOperationException($"The unary operator '{node.Operator}' is unknown.");
            }

            node.InferredType = new TypeDescriptor(exprType);

            return new TypeCheckVisitResult(exprType);
        }

        private TypeCheckVisitResult GetVisitResult(BaseNode node, VisitorArgs args)
        {
            var result = node.Accept(this, args);
            if (result == null)
                throw new InvalidOperationException($"Visiting {node.GetType().Name} object should always return a TypeCheckVisitResult object.");
            return result;
        }

        private void EnsureCorrectType(ExpressionNode expr, PrimitiveType type, VisitorArgs arg)
        {
            var exprRes = GetVisitResult(expr, arg);
            var typeNode = exprRes.Types.FirstOrDefault() as PrimitiveTypeNode;
            if (exprRes.Types.Count() != 1 || (typeNode != null && typeNode.Type != type))
            {
                Reporter.Error($"Expression must be of type '{type}'.", expr.SourcePosition);
            }
        }

        private TypeCheckVisitResult ErrorType()
        {
            return new TypeCheckVisitResult(isError: true);
        }
    }
}