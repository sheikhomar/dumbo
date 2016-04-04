using System;
using System.Diagnostics;
using System.Linq;
using dumbo.Compiler.AST;
using GOLD;

namespace dumbo.Compiler.SyntaxAnalysis
{
    public class AbstractSyntaxTreeBuilder
    {
        public RootNode Build(Reduction root)
        {
            Debug.Assert(root.Parent.Head().Name() == "Start");

            Token funcDeclsToken;
            Token programToken;

            if (root.Count() == 2)
            {
                programToken = root[0];
                funcDeclsToken = root[1];
            }
            else if (root.Count() == 3)
            {
                programToken = root[0].Parent.Name() == "nl" ? root[1] : root[0];
                funcDeclsToken = root[2];
            }
            else if (root.Count() == 4)
            {
                programToken = root[1];
                funcDeclsToken = root[3];
            }
            else
            {
                throw new InvalidOperationException("Unexpected <Start> production.");
            }

            ProgramNode programNode = BuildProgram(programToken);
            RootNode rootNode = new RootNode(programNode);

            if (funcDeclsToken != null)
            {
                AppendFuncDecls(funcDeclsToken, rootNode.FuncDecls);
            }
            
            return rootNode;
        }

        private ProgramNode BuildProgram(Token programSymbol)
        {
            Debug.Assert(programSymbol.Parent.Name() == "Program");
            Reduction lhs = (Reduction)programSymbol.Data;

            Debug.Assert(lhs.Count() == 5);
            SourcePosition srcPos = BuildSourcePosition(lhs[0], lhs[4]);

            Token stmtsToken = lhs[2];
            StmtBlockNode stmts = BuildStmtsBlock(stmtsToken);

            return new ProgramNode(stmts, srcPos);
        }

        private StmtBlockNode BuildStmtsBlock(Token stmtsToken)
        {
            Debug.Assert(stmtsToken.Parent.Name() == "Stmts");
            Reduction lhs = (Reduction)stmtsToken.Data;
            
            StmtBlockNode stmtsBlockNode = new StmtBlockNode();
            
            AppendStatements(stmtsToken, stmtsBlockNode);

            return stmtsBlockNode;
        }

        private void AppendStatements(Token stmtsToken, StmtBlockNode statements)
        {
            Debug.Assert(stmtsToken.Parent.Name() == "Stmts");
            Reduction lhs = (Reduction)stmtsToken.Data;

            if (lhs.Count() > 0)
            {
                Token stmtToken = lhs[0];
                
                StmtNode stmtNode = BuildStmt(stmtToken);
                statements.Add(stmtNode);

                Token stmtsToken2 = lhs[2];
                AppendStatements(stmtsToken2, statements);
            }
        }
        
        private StmtNode BuildStmt(Token stmtToken)
        {
            Debug.Assert(stmtToken.Parent.Name() == "Stmt");
            Reduction lhs = (Reduction)stmtToken.Data;

            string nameOfProduction = lhs[0].Parent.Name();
            StmtProduction production = ConvertToEnum<StmtProduction>(nameOfProduction);
            
            switch (production)
            {
                case StmtProduction.AssignStmt:
                    return BuildAssignStmt(lhs[0]);
                case StmtProduction.IfStmt:
                    return BuildIfStmt(lhs[0]);
                case StmtProduction.RepeatStmt:
                    return BuildRepeatStmtNode(lhs[0]);
                case StmtProduction.Decl:
                    return BuildDeclStmt(lhs[0]);
                case StmtProduction.ReturnStmt:
                    return BuildReturnStmtNode(lhs[0]);
                case StmtProduction.FuncCall:
                    return BuildFuncCallStmt(lhs[0]);
                case StmtProduction.BreakStmt:
                    return new BreakStmtNode();
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private StmtNode BuildRepeatStmtNode(Token token)
        {
            Debug.Assert(token.Parent.Name() == "RepeatStmt");
            Reduction lhs = (Reduction)token.Data;

            if (lhs.Count() == 6)
            {
                Token exprToken = lhs[1];
                Token stmtsToken = lhs[3];

                SourcePosition srcPos = BuildSourcePosition(lhs[0], lhs[5]);
                ExpressionNode exprNode = BuildExprNode(exprToken);
                StmtBlockNode stmtsNode = BuildStmtsBlock(stmtsToken);
                
                return new RepeatStmtNode(exprNode, stmtsNode, srcPos);
            }

            if (lhs.Count() == 7)
            {
                Token exprToken = lhs[2];
                Token stmtsToken = lhs[4];

                SourcePosition srcPos = BuildSourcePosition(lhs[0], lhs[6]);
                ExpressionNode exprNode = BuildExprNode(exprToken);
                StmtBlockNode stmtsNode = BuildStmtsBlock(stmtsToken);

                return new RepeatWhileStmtNode(exprNode, stmtsNode, srcPos);
            }

            throw new InvalidOperationException("Expected another production.");
        }

        private ReturnStmtNode BuildReturnStmtNode(Token token)
        {
            Debug.Assert(token.Parent.Name() == "ReturnStmt");
            Reduction lhs = (Reduction)token.Data;
            ExpressionListNode expressions = null;

            SourcePosition srcPos;

            Reduction data = lhs[1].Data as Reduction;
            if (data != null)
            {
                expressions = BuildExprList(lhs[1]);
                srcPos = BuildSourcePosition(lhs[0], expressions);
            }
            else
            {
                srcPos = BuildSourcePosition(lhs[0], lhs[1]);
            }
            
            return new ReturnStmtNode(expressions, srcPos);
        }

        private FuncCallStmtNode BuildFuncCallStmt(Token token)
        {
            return new FuncCallStmtNode(BuildFuncCallExprNode(token));
        }

        private FuncCallExprNode BuildFuncCallExprNode(Token token)
        {
            Debug.Assert(token.Parent.Name() == "FuncCall");
            Reduction lhs = (Reduction)token.Data;

            string identifier = GetSpelling(lhs[0]);

            var srcPos = BuildSourcePosition(lhs[0], lhs[3]);

            FuncCallExprNode funcCallNode = new FuncCallExprNode(identifier, srcPos);

            Token actualParams = lhs[2];
            AppendFuncCallParameters(actualParams, funcCallNode);

            return funcCallNode;
        }

        private string GetSpelling(Token token)
        {
            return ((TokenData) token.Data).Spelling;
        }

        private void AppendFuncCallParameters(Token token, FuncCallExprNode funcCallNode)
        {
            Debug.Assert(token.Parent.Name() == "ActualParams" || token.Parent.Name() == "MultiActualParams");
            Reduction lhs = (Reduction)token.Data;
            
            if (lhs.Count() > 0)
            {
                Token exprToken = lhs.Count() == 2 ? lhs[0] : lhs[1];
                Token multiExprToken = lhs.Count() == 2 ? lhs[1] : lhs[2];
                
                ExpressionNode exprNode = BuildExprNode(exprToken);
                funcCallNode.Parameters.Add(exprNode);
                AppendFuncCallParameters(multiExprToken, funcCallNode);
            }
        }

        private AssignmentStmtNode BuildAssignStmt(Token token)
        {
            Debug.Assert(token.Parent.Name() == "AssignStmt");
            Reduction lhs = (Reduction)token.Data;

            string firstLhfName = lhs[0].Parent.Name();

            var assignToken = lhs[1].Data as TokenData;
            int line = assignToken.LineNumber;
            int column = assignToken.Column;

            ExpressionListNode exprListNode = BuildExprList(lhs[2]);
            
            if ("Id".Equals(firstLhfName, StringComparison.InvariantCultureIgnoreCase))
            {
                var idListNode = BuildIdentifierList(lhs[0]);
                var srcPos = BuildSourcePosition(idListNode, exprListNode);
                return new AssignmentStmtNode(idListNode, exprListNode, srcPos);
            }

            var declNode = BuildDeclStmt(lhs[0]);
            var srcPos2 = BuildSourcePosition(declNode, exprListNode);
            return new DeclAndAssignmentStmtNode(
                declNode.Type, declNode.Identifiers, exprListNode,
                srcPos2);
        }
        
        private ExpressionListNode BuildExprList(Token token)
        {
            Debug.Assert(token.Parent.Name() == "ExprList");
            Reduction lhs = (Reduction)token.Data;

            var  list = new ExpressionListNode();

            list.Add(BuildExprNode(lhs[0]));

            Token multiToken = lhs[1];
            AppendExpression(multiToken, list);

            if (list.Any())
            {
                var firstItem = list.First();
                var lastItem = list.Last();
                var srcPos = BuildSourcePosition(firstItem, lastItem);
                list.SourcePosition = srcPos;
            }

            return list;
        }

        private void AppendExpression(Token token, ExpressionListNode list)
        {
            Debug.Assert(token.Parent.Name() == "MultiExpr");
            Reduction lhs = (Reduction)token.Data;

            if (lhs.Count() > 0)
            {
                list.Add(BuildExprNode(lhs[1]));

                Token multiIdToken2 = lhs[2];
                AppendExpression(multiIdToken2, list);
            }
        }

        private IfStmtNode BuildIfStmt(Token ifStmToken)
        {
            Debug.Assert(ifStmToken.Parent.Name() == "IfStmt");
            Reduction lhs = (Reduction)ifStmToken.Data;
            Debug.Assert(lhs.Count() == 9);

            Token exprToken = lhs[1];
            Token stmtsToken = lhs[4];
            Token elseIfToken = lhs[5];
            Token elseToken = lhs[6];

            var srcPos = BuildSourcePosition(lhs[0], lhs[8]);

            ExpressionNode predicate = BuildExprNode(exprToken);
            StmtBlockNode stmtsNode = BuildStmtsBlock(stmtsToken);

            IfStmtNode ifStmt;

            Reduction elseReduction = (Reduction) elseToken.Data;
            bool hasElseBody = elseReduction.Count() > 0;

            if (hasElseBody)
            {
                StmtBlockNode  elseStmtsNode = BuildStmtsBlock(elseReduction[2]);
                ifStmt = new IfElseStmtNode(predicate, stmtsNode, elseStmtsNode, srcPos);
            }
            else
            {
                ifStmt = new IfStmtNode(predicate, stmtsNode, srcPos);
            }

            Reduction elseIfReduction = (Reduction)elseIfToken.Data;
            bool hasElseIfStmts = elseIfReduction.Count() > 0;
            if (hasElseIfStmts)
            {
                AppendElseIfStmts(elseIfToken, ifStmt);
            }
            
            return ifStmt;
        }

        private void AppendElseIfStmts(Token token, IfStmtNode ifStmtNode)
        {
            Debug.Assert(token.Parent.Name() == "ElseIfStmts");
            Reduction lhs = (Reduction)token.Data;
            if (lhs.Count() > 0)
            {
                Reduction elseIfReduction = (Reduction) lhs[0].Data;

                Token exprToken = elseIfReduction[1];
                Token stmtsToken = elseIfReduction[4];
                
                ExpressionNode predicate = BuildExprNode(exprToken);
                StmtBlockNode stmtsNode = BuildStmtsBlock(stmtsToken);

                SourcePosition srcPos = BuildSourcePosition(elseIfReduction[0], stmtsNode);

                ElseIfStmtNode elseIfStmt = new ElseIfStmtNode(predicate, stmtsNode, srcPos);
                ifStmtNode.ElseIfStatements.Add(elseIfStmt);

                AppendElseIfStmts(lhs[1], ifStmtNode);
            }
        }

        private ExpressionNode BuildExprNode(Token exprToken)
        {
            string name = exprToken.Parent.Name();
            Reduction lhs = (Reduction)exprToken.Data;

            if (name == "Unary")
            {
                return BuildUnaryExprNode(exprToken);
            }
            else
            {
                return BuildBinaryExpr(exprToken);
            }
        }

        private ExpressionNode BuildBinaryExpr(Token andToken)
        {
            string name = andToken.Parent.Name();
            Reduction lhs = (Reduction) andToken.Data;

            if (name.Equals("Unary"))
                throw new ArgumentException("Unary not allowed here.");

            if (lhs.Count() == 3)
            {
                ExpressionNode leftOperand = BuildExprNode(lhs[0]);
                BinaryOperatorType operatorType = ParseOperator(GetSpelling(lhs[1]));
                ExpressionNode rightOperand = BuildExprNode(lhs[2]);

                var srcPos = BuildSourcePosition(leftOperand, rightOperand);

                return new BinaryOperationNode(leftOperand, operatorType, rightOperand, srcPos);
            }

            return BuildExprNode(lhs[0]);
        }

        private ExpressionNode BuildUnaryExprNode(Token unaryToken)
        {
            Debug.Assert(unaryToken.Parent.Name() == "Unary");
            Reduction lhs = (Reduction)unaryToken.Data;

            if (lhs.Count() == 2)
            {
                string operatorCode = GetSpelling(lhs[0]).ToLower();
                ExpressionNode unaryExprNode = BuildUnaryExprNode(lhs[1]);
                SourcePosition srcPos = BuildSourcePosition(lhs[0], unaryExprNode);

                return new UnaryOperationNode(
                    ParseUnaryOperator(operatorCode),
                    unaryExprNode,
                    srcPos);
            }

            return BuildValueNode(lhs[0]);
        }

        private ExpressionNode BuildValueNode(Token valueToken)
        {
            Debug.Assert(valueToken.Parent.Name() == "Value");
            Reduction lhs = (Reduction)valueToken.Data;

            string lhsName = lhs[0].Parent.Name();

            switch (lhsName)
            {
                case "Literal":
                    Reduction lhs2 = (Reduction) lhs[0].Data;
                    string value = GetSpelling(lhs2[0]);
                    string literalType = lhs2[0].Parent.Name();
                    HappyType happyType;

                    if (literalType == "NumberLiteral")
                        happyType = HappyType.Number;
                    else if (literalType == "TextLiteral")
                        happyType = HappyType.Text;
                    else if (literalType == "BooleanLiteral")
                        happyType = HappyType.Boolean;
                    else
                        throw new InvalidOperationException("Invalid literal type.");

                    SourcePosition srcPos = BuildSourcePosition(lhs2[0], lhs2[0]);

                    return new LiteralValueNode(value, happyType, srcPos);
                case "Id":
                    string name = GetSpelling(lhs[0]);
                    SourcePosition srcPos2 = BuildSourcePosition(lhs[0], lhs[0]);
                    return new IdentifierNode(name, srcPos2);
                case "FuncCall":
                    return BuildFuncCallExprNode(lhs[0]);
                case "(":
                    return BuildExprNode(lhs[1]);
                default:
                    throw new ArgumentException("Unexpected value node.");
            }
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

        private DeclStmtNode BuildDeclStmt(Token declStmtToken)
        {
            Debug.Assert(declStmtToken.Parent.Name() == "Decl");
            Reduction lhs = (Reduction)declStmtToken.Data;

            Token underToken = lhs[0];

            string name = GetSpelling(underToken);

            IdentifierListNode idList = BuildIdentifierList(lhs[1]);
            HappyType type = ConvertHappyType(name);

            SourcePosition srcPos = BuildSourcePosition(lhs[0], idList);

            return new DeclStmtNode(idList, type, srcPos);
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

            throw new InvalidOperationException("Invalid type found: " + typeSpec);
        }

        private IdentifierListNode BuildIdentifierList(Token idToken)
        {
            Debug.Assert(idToken.Parent.Name() == "Id");
            Reduction lhs = (Reduction)idToken.Data;

            var listNode = new IdentifierListNode();

            string name = GetSpelling(lhs[0]);

            var srcPos = BuildSourcePosition(lhs[0], lhs[0]);

            listNode.Add(new IdentifierNode(name, srcPos));
            
            Token multiIdentToken = lhs[1];
            AppendIdentifier(multiIdentToken, listNode);

            if (listNode.Any())
            {
                var firstNode = listNode.First();
                var lastNode = listNode.Last();
                listNode.SourcePosition = BuildSourcePosition(firstNode, lastNode);
            }

            return listNode;
        }

        private void AppendIdentifier(Token multiIdToken, IdentifierListNode list)
        {
            Debug.Assert(multiIdToken.Parent.Name() == "MultiId");
            Reduction lhs = (Reduction)multiIdToken.Data;

            if (lhs.Count() > 0)
            {
                var srcPos = BuildSourcePosition(lhs[1], lhs[1]);

                string name = GetSpelling(lhs[1]);
                list.Add(new IdentifierNode(name, srcPos));

                Token multiIdToken2 = lhs[2];
                AppendIdentifier(multiIdToken2, list);
            }
        }
        
        private void AppendFuncDecls(Token token, FuncDeclListNode list)
        {
            Debug.Assert(token.Parent.Name() == "FuncDecls");
            Reduction lhs = (Reduction)token.Data;

            if (lhs.Count() > 0)
            {
                list.Add(BuildFuncDeclNode(lhs[0]));

                if (lhs.Count() == 3)
                {
                    AppendFuncDecls(lhs[2], list);
                }
            }
        }

        private FuncDeclNode BuildFuncDeclNode(Token token)
        {
            Debug.Assert(token.Parent.Name() == "FuncDecl");
            Reduction lhs = (Reduction)token.Data;

            string funcName = GetSpelling(lhs[1]);

            var srcPos = BuildSourcePosition(lhs[0], lhs[10]);

            StmtBlockNode funcBodyNode = BuildStmtsBlock(lhs[8]);
            FuncDeclNode funcDelc = new FuncDeclNode(funcName, funcBodyNode, srcPos);
            
            AppendFormalParameters(lhs[3], funcDelc);

            AppendReturnTypes(lhs[6], funcDelc);

            return funcDelc;
        }

        private void AppendReturnTypes(Token token, FuncDeclNode funcDeclNode)
        {
            Debug.Assert(token.Parent.Name() == "ReturnTypes" || token.Parent.Name() == "MultiReturnTypes");
            Reduction lhs = (Reduction)token.Data;

            if (lhs.Count() > 1)
            {
                Token singleToken = lhs.Count() == 2 ? lhs[0] : lhs[1];
                Token multiToken = lhs.Count() == 2 ? lhs[1] : lhs[2];

                Reduction retReduction = (Reduction) singleToken.Data;
                string retTypeRef = GetSpelling(retReduction[0]);
                HappyType retType = ConvertHappyType(retTypeRef);
                funcDeclNode.ReturnTypes.Add(retType);

                AppendReturnTypes(multiToken, funcDeclNode);
            }
        }
        
        private void AppendFormalParameters(Token token, FuncDeclNode funcCallNode)
        {
            Debug.Assert(token.Parent.Name() == "FormalParams" || token.Parent.Name() == "MultiFormalParams");
            Reduction lhs = (Reduction)token.Data;

            if (lhs.Count() > 0)
            {
                Token singleToken = lhs.Count() == 2 ? lhs[0] : lhs[1];
                Token multiToken = lhs.Count() == 2 ? lhs[1] : lhs[2];

                Reduction formalParmReduction = (Reduction) singleToken.Data;
                string typeSpec =  GetSpelling(formalParmReduction[0]);
                string paramName = GetSpelling(formalParmReduction[1]);
                HappyType paramType = ConvertHappyType(typeSpec);

                SourcePosition srcPos = BuildSourcePosition(formalParmReduction[0], formalParmReduction[1]);

                FormalParamNode paramNode = new FormalParamNode(paramName, paramType, srcPos);
                funcCallNode.Parameters.Add(paramNode);

                AppendFormalParameters(multiToken, funcCallNode);
            }
        }

        private T ConvertToEnum<T>(string input) where T : struct, IComparable, IFormattable, IConvertible
        {
            T output;
            
            if (!typeof(T).IsEnum)
            {
                throw new ArgumentException("The type T, is invalid in the given context");
            }
            else
            {
                if (Enum.TryParse(input, out output))
                {
                    return output;
                }
                else
                {
                    throw new ArgumentException(input + " is not of the type T");
                }
            }
        }
        
        private SourcePosition BuildSourcePosition(Token startToken, Token endToken)
        {
            var startTokenData = startToken.Data as TokenData;
            if (startTokenData == null)
                throw new ArgumentException("Is not a terminal node.", nameof(startToken));

            var endTokenData = endToken.Data as TokenData;
            if (endTokenData == null)
                throw new ArgumentException("Is not a terminal node.", nameof(endToken));

            int startLine = startTokenData.LineNumber + 1;
            int startColumn = startTokenData.Column + 1;
            int endLine = endTokenData.LineNumber + 1;
            int endColumn = endTokenData.Column + endTokenData.Spelling.Length + 2;
            return new SourcePosition(startLine, startColumn, endLine, endColumn);
        }

        private SourcePosition BuildSourcePosition(BaseNode startNode, BaseNode endNode)
        {
            int startLine = startNode.SourcePosition.StartLine;
            int startColumn = startNode.SourcePosition.StartColumn;
            int endLine = endNode.SourcePosition.EndLine;
            int endColumn = endNode.SourcePosition.EndColumn;
            return new SourcePosition(startLine, startColumn, endLine, endColumn);
        }
        
        private SourcePosition BuildSourcePosition(Token startToken, BaseNode endNode)
        {
            var startSrcPos = BuildSourcePosition(startToken, startToken);

            int startLine = startSrcPos.StartLine;
            int startColumn = startSrcPos.StartColumn;
            int endLine = endNode.SourcePosition.EndLine;
            int endColumn = endNode.SourcePosition.EndColumn;
            return new SourcePosition(startLine, startColumn, endLine, endColumn);
        }
    }
}