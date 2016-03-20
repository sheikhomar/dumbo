using System;
using System.Linq;

namespace dumbo.Compiler.AST.Builder
{
    public class AstBuilder
    {
        #region Constants

        public const string TokenId = "Id";
        public const string TokenNumberLiteral = "NumberLiteral";
        public const string TokenTextLiteral = "TextLiteral";
        public const string TokenBooleanLiteral = "BooleanLiteral";
        public const string TokenNumber = "Number";
        public const string TokenText = "Text";
        public const string TokenBoolean = "Boolean";
        public const string TokenWhile = "While";
        public const string TokenProgram = "Program";

        public const string RuleStart = "Start";
        public const string RuleProgram = "Program";
        public const string RuleFuncDecls = "FuncDecls";
        public const string RuleFuncDecl = "FuncDecl";
        public const string RuleStmts = "Stmts";
        public const string RuleStmt = "Stmt";
        public const string RuleAssignStmt = "AssignStmt";
        public const string RuleIfStmt = "IfStmt";
        public const string RuleRepeatStmt = "RepeatStmt";
        public const string RuleDecl = "Decl";
        public const string RuleReturnStmt = "ReturnStmt";
        public const string RuleFuncCall = "FuncCall";
        public const string RuleBreakStmt = "BreakStmt";
        public const string RuleActualParams = "ActualParams";
        public const string RuleValue = "Value";
        public const string RuleLiteral = "Literal";
        public const string RuleExpr = "Expr";
        public const string RuleUnary = "Unary";
        public const string RuleExprList = "ExprList";
        public const string RuleMultiExpr = "MultiExpr";
        public const string RuleMultiActualParams = "MultiActualParams";
        public const string RuleId = "Id";
        public const string RuleMultiId = "MultiId";
        public const string RuleElseIfStmts = "ElseIfStmts";
        public const string RuleElseIfStmt = "ElseIfStmt";
        public const string RuleElseStmt = "ElseStmt";
        public const string RuleFormalParams = "FormalParams";
        public const string RuleMultiFormalParams = "MultiFormalParams";
        public const string RuleFormalParam = "FormalParam";
        public const string RuleReturnTypes = "ReturnTypes";
        public const string RuleMultiReturnTypes = "MultiReturnTypes";
        public const string RuleReturnType = "ReturnType";

        #endregion

        public RootNode BuildRootNode(IConcreteTreeNode node)
        {
            EnsureValidNode(node, RuleStart);

            IConcreteTreeNode progTreeNode = node.TryFindFirstChild(RuleProgram);
            IConcreteTreeNode funcTreeNode = node.TryFindFirstChild(RuleFuncDecls);

            ProgramNode programNode = BuildProgramNode(progTreeNode);
            FuncDeclListNode funcDeclsNode = null;
            if (funcTreeNode != null)
                funcDeclsNode = BuildFuncDeclListNode(funcTreeNode);

            return new RootNode(programNode, funcDeclsNode);
        }

        public ProgramNode BuildProgramNode(IConcreteTreeNode node)
        {
            EnsureValidNode(node, RuleProgram);

            var programTkn = node.GetFirstChild(TokenProgram);

            StmtBlockNode stmts = BuildStmtsBlock(node.TryFindFirstChild(RuleStmts));
            return new ProgramNode(stmts);
        }

        public FuncDeclListNode BuildFuncDeclListNode(IConcreteTreeNode node)
        {
            EnsureValidNode(node, RuleFuncDecls);

            var list = new FuncDeclListNode();

            AppendToFuncDeclListNode(node, list);

            return list;
        }

        private void AppendToFuncDeclListNode(IConcreteTreeNode node, FuncDeclListNode list)
        {
            EnsureValidNode(node, RuleFuncDecls);

            IConcreteTreeNode funcDeclTreeNode = node.TryFindFirstChild(RuleFuncDecl);
            IConcreteTreeNode funcDeclsTreeNode = node.TryFindFirstChild(RuleFuncDecls);

            if (funcDeclTreeNode != null)
                list.Add(BuildFuncDeclNode(funcDeclTreeNode));

            if (funcDeclsTreeNode != null)
                AppendToFuncDeclListNode(funcDeclsTreeNode, list);
        }

        public FuncDeclNode BuildFuncDeclNode(IConcreteTreeNode node)
        {
            EnsureValidNode(node, RuleFuncDecl);

            var idTreeNode = node.GetFirstChild(TokenId);
            var paramsTreeNode = node.GetFirstChild(RuleFormalParams);
            var returnTypesTreeNode = node.GetFirstChild(RuleReturnTypes);
            var stmtsTreeNode = node.GetFirstChild(RuleStmts);

            IdentifierNode id = new IdentifierNode(idTreeNode.Data);
            StmtBlockNode funcBodyNode = BuildStmtsBlock(stmtsTreeNode);
            FuncDeclNode funcDelc = new FuncDeclNode(id, funcBodyNode);

            AppendFormalParameters(paramsTreeNode, funcDelc);

            AppendReturnTypes(returnTypesTreeNode, funcDelc);

            return funcDelc;
        }

        private void AppendReturnTypes(IConcreteTreeNode node, FuncDeclNode funcDeclNode)
        {
            EnsureValidNode(node, RuleReturnTypes, RuleMultiReturnTypes);
            
            if (node.ChildCount > 1) // Ignore token 'Nothing'
            {
                IConcreteTreeNode singleToken = node.TryFindFirstChild(RuleReturnType);
                if (singleToken != null)
                {
                    IConcreteTreeNode firstChild = singleToken[0];
                    HappyType returnType = ConvertHappyType(firstChild.Data);
                    funcDeclNode.ReturnTypes.Add(returnType);
                }

                IConcreteTreeNode multiToken = node.TryFindFirstChild(RuleMultiReturnTypes);
                if (multiToken != null)
                    AppendReturnTypes(multiToken, funcDeclNode);
            }
        }

        private void AppendFormalParameters(IConcreteTreeNode node, FuncDeclNode funcDeclNode)
        {
            EnsureValidNode(node, RuleFormalParams, RuleMultiFormalParams);
            
            if (node.ChildCount > 0)
            {
                IConcreteTreeNode singleToken = node.TryFindFirstChild(RuleFormalParam);
                if (singleToken != null)
                {
                    string typeSpec = singleToken[0].Data;
                    string paramName = singleToken[1].Data;
                    HappyType paramType = ConvertHappyType(typeSpec);

                    FormalParamNode paramNode = new FormalParamNode(paramName, paramType);
                    funcDeclNode.Parameters.Add(paramNode);
                }
                
                IConcreteTreeNode multiToken = node.TryFindFirstChild(RuleMultiFormalParams);
                if (multiToken != null)
                    AppendFormalParameters(multiToken, funcDeclNode);
            }
        }

        private StmtBlockNode BuildStmtsBlock(IConcreteTreeNode node)
        {
            EnsureValidNode(node, RuleStmts);

            StmtBlockNode stmtsBlockNode = new StmtBlockNode();

            AppendToStmtsBlocksNode(node, stmtsBlockNode);

            return stmtsBlockNode;
        }
        
        private void AppendToStmtsBlocksNode(IConcreteTreeNode node, StmtBlockNode statements)
        {
            EnsureValidNode(node, RuleStmts);

            IConcreteTreeNode stmtTreeNode = node.TryFindFirstChild(RuleStmt);
            IConcreteTreeNode stmtsTreeNode = node.TryFindFirstChild(RuleStmts);

            if (stmtTreeNode != null)
                statements.Add(BuildStmtNode(stmtTreeNode));

            if (stmtsTreeNode != null)
                AppendToStmtsBlocksNode(stmtsTreeNode, statements);
        }

        public StmtNode BuildStmtNode(IConcreteTreeNode node)
        {
            EnsureValidNode(node, RuleStmt);

            var childNode = node[0];

            switch (childNode.Name)
            {
                case RuleAssignStmt: return BuildAssignStmtNode(childNode);
                case RuleBreakStmt: return new BreakStmtNode(); ;
                case RuleDecl: return BuildDeclStmtNode(childNode);
                case RuleFuncCall: return BuildFuncCallStmtNode(childNode);
                case RuleIfStmt: return BuildIfStmtNode(childNode);
                case RuleRepeatStmt: return BuildRepeatStmtNode(childNode);
                case RuleReturnStmt: return BuildReturnStmtNode(childNode);
                default:
                    throw new ArgumentOutOfRangeException($"The node <{childNode.Name}> is unknown.");
            }
        }

        public FuncCallNode BuildFuncCallExprNode(IConcreteTreeNode node)
        {
            EnsureValidNode(node, RuleFuncCall);
            
            IConcreteTreeNode idTreeNode = node.GetFirstChild(TokenId);
            IConcreteTreeNode paramsTreeNode = node.GetFirstChild(RuleActualParams);

            string identifier = idTreeNode.Data;
            
            FuncCallNode funcCallNode = new FuncCallNode(identifier);

            AppendToFuncCallParameters(paramsTreeNode, funcCallNode);

            return funcCallNode;
        }

        private void AppendToFuncCallParameters(IConcreteTreeNode node, FuncCallNode astNode)
        {
            EnsureValidNode(node, RuleActualParams, RuleMultiActualParams);

            var exprTreeNode = node.TryFindFirstChild(RuleExpr);
            if (exprTreeNode != null)
                astNode.Parameters.Add(BuildExprNode(exprTreeNode));

            var mulParamsTreeNode = node.TryFindFirstChild(RuleMultiActualParams);
            if (mulParamsTreeNode != null)
                AppendToFuncCallParameters(mulParamsTreeNode, astNode);
        }

        public FuncCallStmtNode BuildFuncCallStmtNode(IConcreteTreeNode node)
        {
            return new FuncCallStmtNode(BuildFuncCallExprNode(node));
        }

        public ReturnStmtNode BuildReturnStmtNode(IConcreteTreeNode node)
        {
            EnsureValidNode(node, RuleReturnStmt);

            ExpressionListNode expressions = null;

            var exprListTreeNode = node.TryFindFirstChild(RuleExprList);
            if (exprListTreeNode != null)
                expressions = BuildExprList(exprListTreeNode);

            return new ReturnStmtNode(expressions);
        }

        public DeclStmtNode BuildDeclStmtNode(IConcreteTreeNode node)
        {
            EnsureValidNode(node, RuleDecl);

            var typeSpecTreeNode = node[0];
            if (typeSpecTreeNode == null)
                throw new ExceptedTokensNotFoundException(TokenBoolean, TokenNumber, TokenText);

            var idTreeNode = node.GetFirstChild(RuleId);

            IdentifierListNode idList = BuildIdentifierList(idTreeNode);
            HappyType type = ConvertHappyType(typeSpecTreeNode.Name);
            return new DeclStmtNode(idList, type);
        }

        private IdentifierListNode BuildIdentifierList(IConcreteTreeNode node)
        {
            EnsureValidNode(node, RuleId);

            var listNode = new IdentifierListNode();
            AppendToIdentifierList(node, listNode);
            return listNode;
        }

        private void AppendToIdentifierList(IConcreteTreeNode node, IdentifierListNode list)
        {
            EnsureValidNode(node, RuleId, RuleMultiId);

            var idTokenTreeNode = node.TryFindFirstChild(TokenId);
            if (idTokenTreeNode != null)
                list.Add(new IdentifierNode(idTokenTreeNode.Data));

            var multiIdTreeNode = node.TryFindFirstChild(RuleMultiId);
            if (multiIdTreeNode != null)
                AppendToIdentifierList(multiIdTreeNode, list);
        }

        private HappyType ConvertHappyType(string typeSpec)
        {
            string newTypeSpec = typeSpec.ToLower();
            if ("number".Equals(newTypeSpec))
                return HappyType.Number;
            if ("boolean".Equals(newTypeSpec))
                return HappyType.Boolean;
            if ("text".Equals(newTypeSpec))
                return HappyType.Text;

            throw new ExceptedTokensNotFoundException(TokenBoolean, TokenNumber, TokenText);
        }

        public StmtNode BuildRepeatStmtNode(IConcreteTreeNode node)
        {
            EnsureValidNode(node, RuleRepeatStmt);

            IConcreteTreeNode exprTreeNode = node.GetFirstChild(RuleExpr);
            IConcreteTreeNode stmtsTreeNode = node.GetFirstChild(RuleStmts);

            ExpressionNode exprNode = BuildExprNode(exprTreeNode);
            StmtBlockNode stmtsNode = BuildStmtsBlock(stmtsTreeNode);

            if (node.HasChild(TokenWhile))
                return new RepeatWhileStmtNode(exprNode, stmtsNode);

            return new RepeatStmtNode(exprNode, stmtsNode);
        }

        public IfStmtNode BuildIfStmtNode(IConcreteTreeNode node)
        {
            EnsureValidNode(node, RuleIfStmt);

            var exprToken = node.GetFirstChild(RuleExpr);
            var stmtsToken = node.GetFirstChild(RuleStmts);
            var elseIfToken = node.GetFirstChild(RuleElseIfStmts);
            var elseToken = node.GetFirstChild(RuleElseStmt);

            ExpressionNode predicate = BuildExprNode(exprToken);
            StmtBlockNode stmtsNode = BuildStmtsBlock(stmtsToken);

            IfStmtNode ifStmt;

            bool hasElseBody = elseToken.ChildCount > 0;
            if (hasElseBody)
            {
                var elseStmtsTreeNode = elseToken.GetFirstChild(RuleStmts);
                StmtBlockNode elseStmtsNode = BuildStmtsBlock(elseStmtsTreeNode);
                ifStmt = new IfElseStmtNode(predicate, stmtsNode, elseStmtsNode);
            }
            else
            {
                ifStmt = new IfStmtNode(predicate, stmtsNode);
            }

            AppendElseIfStmts(elseIfToken, ifStmt);
            
            return ifStmt;
        }

        private void AppendElseIfStmts(IConcreteTreeNode node, IfStmtNode ifStmtNode)
        {
            EnsureValidNode(node, RuleElseIfStmts);
            
            if (node.ChildCount > 0)
            {
                IConcreteTreeNode elseIfToken = node.TryFindFirstChild(RuleElseIfStmt);
                if (elseIfToken != null)
                {
                    ElseIfStmtNode elseIfNode = BuildElseIfStmtNode(elseIfToken);
                    ifStmtNode.ElseIfStatements.Add(elseIfNode);
                }

                IConcreteTreeNode elseIfStmtsTreeNode = node.TryFindFirstChild(RuleElseIfStmts);
                if (elseIfStmtsTreeNode != null)
                    AppendElseIfStmts(elseIfStmtsTreeNode, ifStmtNode);
            }
        }

        public ElseIfStmtNode BuildElseIfStmtNode(IConcreteTreeNode node)
        {
            EnsureValidNode(node, RuleElseIfStmt);

            IConcreteTreeNode exprToken = node.GetFirstChild(RuleExpr);
            IConcreteTreeNode stmtsToken = node.GetFirstChild(RuleStmts);

            ExpressionNode predicate = BuildExprNode(exprToken);
            StmtBlockNode stmtsNode = BuildStmtsBlock(stmtsToken);

            return new ElseIfStmtNode(predicate, stmtsNode);
        }

        public StmtNode BuildAssignStmtNode(IConcreteTreeNode node)
        {
            EnsureValidNode(node, RuleAssignStmt);

            StmtNode stmtToReturn;

            IConcreteTreeNode exprTreeNode = node.GetFirstChild(RuleExprList);

            ExpressionListNode exprListNode = BuildExprList(exprTreeNode);
            
            var idTreeNode = node.TryFindFirstChild(RuleId);
            if (idTreeNode != null)
            {
                IdentifierListNode identifiers = BuildIdentifierList(idTreeNode);
                stmtToReturn = new AssignmentStmtNode(identifiers, exprListNode);
            }
            else
            {
                IConcreteTreeNode declTreeNode = node.GetFirstChild(RuleDecl);
                var declNode = BuildDeclStmtNode(declTreeNode);
                stmtToReturn = new DeclAndAssignmentStmtNode(declNode.Type, declNode.Identifiers, exprListNode);
            }

            return stmtToReturn;
        }
        
        public ExpressionListNode BuildExprList(IConcreteTreeNode node)
        {
            EnsureValidNode(node, RuleExprList);

            ExpressionListNode list = new ExpressionListNode();

            AppendToExpressionList(node, list);

            return list;
        }

        private void AppendToExpressionList(IConcreteTreeNode node, ExpressionListNode list)
        {
            EnsureValidNode(node, RuleExprList, RuleMultiExpr);

            IConcreteTreeNode exprTreeNode = node.TryFindFirstChild(RuleExpr);
            if (exprTreeNode != null)
                list.Add(BuildExprNode(exprTreeNode));

            IConcreteTreeNode multiExprTreeNode = node.TryFindFirstChild(RuleMultiExpr);
            if (multiExprTreeNode != null)
                AppendToExpressionList(multiExprTreeNode, list);
        }

        private ExpressionNode BuildExprNode(IConcreteTreeNode node)
        {
            var childNode = node[0];
            var isUnary = node.Name.Equals(RuleUnary);
            return isUnary ? BuildUnaryExprNode(node) : BuildBinaryExpr(node);
        }
        
        private ExpressionNode BuildBinaryExpr(IConcreteTreeNode node)
        {
            if (node.ChildCount == 3)
            {
                ExpressionNode leftOperand = BuildExprNode(node[0]);
                BinaryOperatorType operatorType = ParseOperator(node[1].Data);
                ExpressionNode rightOperand = BuildExprNode(node[2]);
                return new BinaryOperationNode(leftOperand, operatorType, rightOperand);
            }

            return BuildExprNode(node[0]);
        }

        public LiteralValueNode BuildLiteralValueNode(IConcreteTreeNode node)
        {
            EnsureValidNode(node, RuleLiteral);

            var childNode = node[0];

            HappyType happyType;
            switch (childNode.Name)
            {
                case TokenBooleanLiteral:
                    happyType = HappyType.Boolean;
                    break;
                case TokenNumberLiteral:
                    happyType = HappyType.Number;
                    break;
                case TokenTextLiteral:
                    happyType = HappyType.Text;
                    break;
                default:
                    throw new ExceptedTokensNotFoundException(
                        TokenBooleanLiteral,
                        TokenNumberLiteral,
                        TokenTextLiteral
                    );
            }
            return new LiteralValueNode(childNode.Data, happyType);
        }

        private ExpressionNode BuildValueNode(IConcreteTreeNode node)
        {
            EnsureValidNode(node, RuleValue);
            
            var childNode = node[0];

            switch (childNode.Name)
            {
                case TokenId:
                    return new IdentifierNode(childNode.Data);

                case RuleLiteral:
                    return BuildLiteralValueNode(childNode);
                
                case RuleFuncCall:
                    return BuildFuncCallExprNode(childNode);

                default:
                    var exprTreeNode = node.TryFindFirstChild(RuleExpr);
                    if (exprTreeNode != null)
                        return BuildExprNode(exprTreeNode);

                    break;
            }

            throw new ExceptedTokensNotFoundException(TokenId, RuleLiteral, RuleFuncCall, RuleExpr);
        }

        private ExpressionNode BuildUnaryExprNode(IConcreteTreeNode node)
        {
            EnsureValidNode(node, RuleUnary);

            ExpressionNode retNode;
            IConcreteTreeNode treeNode = node[0];

            if (treeNode.Name.Equals(RuleValue))
            {
                retNode = BuildValueNode(treeNode);
            }
            else
            {
                var unaryTreeNode = node.GetFirstChild(RuleUnary);
                string operatorCode = treeNode.Data;
                return new UnaryOperationNode(
                    ParseUnaryOperator(operatorCode),
                    BuildUnaryExprNode(unaryTreeNode));
            }
            
            return retNode;
        }

        private UnaryOperatorType ParseUnaryOperator(string operatorCode)
        {
            operatorCode = operatorCode.ToLower();

            switch (operatorCode)
            {
                case "not": return UnaryOperatorType.Not;
                case "-": return UnaryOperatorType.Minus;
                case "+": return UnaryOperatorType.Plus;

                default:
                    throw new InvalidOperationException("Unknown unary operator " + operatorCode);
            }
        }

        private BinaryOperatorType ParseOperator(string operatorCode)
        {
            operatorCode = operatorCode.ToLower();

            switch (operatorCode)
            {
                case "or": return BinaryOperatorType.Or;
                case "and": return BinaryOperatorType.And;
                case ">": return BinaryOperatorType.GreaterThan;
                case ">=": return BinaryOperatorType.GreaterOrEqual;
                case "<": return BinaryOperatorType.LessThan;
                case "<=": return BinaryOperatorType.LessOrEqual;
                case "=": return BinaryOperatorType.Equals;
                case "+": return BinaryOperatorType.Plus;
                case "-": return BinaryOperatorType.Minus;
                case "*": return BinaryOperatorType.Times;
                case "/": return BinaryOperatorType.Division;
                case "%": return BinaryOperatorType.Modulo;
                default:
                    throw new InvalidOperationException("Unknown operator " + operatorCode);
            }
        }

        private void EnsureValidNode(IConcreteTreeNode node, string name)
        {
            if (node == null)
                throw new ArgumentNullException($"Node is null. Was expected node <{name}>.");
            if (!node.Name.Equals(name))
                throw new ArgumentException($"Was expected node <{name}>, but was <{node.Name}>.");
        }

        private void EnsureValidNode(IConcreteTreeNode node, params string[] expectedNames)
        {
            if (node == null)
                throw new ArgumentNullException($"Node is null.");
            if (!expectedNames.Contains(node.Name))
            {
                string allNames = string.Join(">, <", expectedNames);
                throw new ArgumentException($"Expected nodes <{allNames}>, but was <{node.Name}>.");
            }

        }
    }
}