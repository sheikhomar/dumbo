using System;
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
                    var exprType = exprRes.Types.First();
                    Reporter.Error($"The variable '{id.Name}' cannot be assigned the type {exprType}.", expr.SourcePosition);
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
            foreach (var item in node)
                item.Accept(this, arg);

            return EmptyResult;
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

            return new TypeCheckVisitResult(node.DeclarationNode.ReturnTypes);
        }

        public VisitResult Visit(FuncCallStmtNode node, VisitorArgs arg)
        {
            node.CallNode.Accept(this, arg);

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
            return EmptyResult;
        }

        public VisitResult Visit(IdentifierListNode node, VisitorArgs arg)
        {
            foreach (var item in node)
                item.Accept(this, arg);

            return EmptyResult;
        }

        public VisitResult Visit(IdentifierNode node, VisitorArgs arg)
        {

            HappyType type = node.DeclarationNode?.Type ?? HappyType.Error;
            return new TypeCheckVisitResult(type);
        }

        public VisitResult Visit(IfElseStmtNode node, VisitorArgs arg)
        {
            EnsureCorrectType(node.Predicate, HappyType.Number, arg);

            node.Body.Accept(this, arg);
            node.ElseIfStatements.Accept(this, arg);
            node.Else.Accept(this, arg);

            return EmptyResult;
        }

        public VisitResult Visit(IfStmtNode node, VisitorArgs arg)
        {
            EnsureCorrectType(node.Predicate, HappyType.Number, arg);

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

            return EmptyResult;
        }

        public VisitResult Visit(RepeatStmtNode node, VisitorArgs arg)
        {
            EnsureCorrectType(node.Number, HappyType.Number, arg);

            return EmptyResult;
        }

        public VisitResult Visit(RepeatWhileStmtNode node, VisitorArgs arg)
        {
            EnsureCorrectType(node.Predicate, HappyType.Boolean, arg);

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