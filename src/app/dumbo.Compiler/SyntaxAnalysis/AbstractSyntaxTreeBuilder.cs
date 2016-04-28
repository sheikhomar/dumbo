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

            Token constDeclsToken = root[1];
            Token programToken = root[2];
            Token funcDeclsToken = root[4];
            
            ProgramNode programNode = BuildProgram(programToken);
            RootNode rootNode = new RootNode(programNode);
            AppendFuncDecls(funcDeclsToken, rootNode.FuncDecls);
            AppendConstDecls(constDeclsToken, rootNode.ConstDecls);
            
            return rootNode;
        }

        private ProgramNode BuildProgram(Token programSymbol)
        {
            Debug.Assert(programSymbol.Parent.Name() == "Program");
            Reduction rhs = (Reduction)programSymbol.Data;

            Debug.Assert(rhs.Count() == 5);
            SourcePosition srcPos = BuildSourcePosition(rhs[0], rhs[4]);

            Token stmtsToken = rhs[2];
            StmtBlockNode stmts = BuildStmtsBlock(stmtsToken);

            return new ProgramNode(stmts, srcPos);
        }

        private StmtBlockNode BuildStmtsBlock(Token stmtsToken)
        {
            Debug.Assert(stmtsToken.Parent.Name() == "Stmts");
            Reduction rhs = (Reduction)stmtsToken.Data;
            
            StmtBlockNode stmtsBlockNode = new StmtBlockNode();
            
            AppendStatements(stmtsToken, stmtsBlockNode);

            return stmtsBlockNode;
        }

        private void AppendStatements(Token stmtsToken, StmtBlockNode statements)
        {
            Debug.Assert(stmtsToken.Parent.Name() == "Stmts");
            Reduction rhs = (Reduction)stmtsToken.Data;

            if (rhs.Count() > 0)
            {
                Token stmtToken = rhs[0];
                
                StmtNode stmtNode = BuildStmt(stmtToken);
                statements.Add(stmtNode);

                Token stmtsToken2 = rhs[2];
                AppendStatements(stmtsToken2, statements);
            }
        }
        
        private StmtNode BuildStmt(Token stmtToken)
        {
            Debug.Assert(stmtToken.Parent.Name() == "Stmt");
            Reduction rhs = (Reduction)stmtToken.Data;

            string nameOfProduction = rhs[0].Parent.Name();
            switch (nameOfProduction)
            {
                case "AssignStmt": return BuildAssignStmt(rhs[0]);
                case "IfStmt": return BuildIfStmt(rhs[0]);
                case "RepeatStmt": return BuildRepeatStmtNode(rhs[0]);
                case "Decl": return BuildDeclStmt(rhs[0]);
                case "ReturnStmt": return BuildReturnStmtNode(rhs[0]);
                case "FuncCall": return BuildFuncCallStmt(rhs[0]);
                case "BreakStmt":
                    Reduction rhs2 = (Reduction)rhs[0].Data;
                    var srcPos = BuildSourcePosition(rhs2[0], rhs2[0]);
                    return new BreakStmtNode(srcPos);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private StmtNode BuildRepeatStmtNode(Token token)
        {
            Debug.Assert(token.Parent.Name() == "RepeatStmt");
            Reduction rhs = (Reduction)token.Data;

            if (rhs.Count() == 6)
            {
                Token exprToken = rhs[1];
                Token stmtsToken = rhs[3];

                SourcePosition srcPos = BuildSourcePosition(rhs[0], rhs[5]);
                ExpressionNode exprNode = BuildExprNode(exprToken);
                StmtBlockNode stmtsNode = BuildStmtsBlock(stmtsToken);
                
                return new RepeatStmtNode(exprNode, stmtsNode, srcPos);
            }

            if (rhs.Count() == 7)
            {
                Token exprToken = rhs[2];
                Token stmtsToken = rhs[4];

                SourcePosition srcPos = BuildSourcePosition(rhs[0], rhs[6]);
                ExpressionNode exprNode = BuildExprNode(exprToken);
                StmtBlockNode stmtsNode = BuildStmtsBlock(stmtsToken);

                return new RepeatWhileStmtNode(exprNode, stmtsNode, srcPos);
            }

            throw new InvalidOperationException("Expected another production.");
        }

        private ReturnStmtNode BuildReturnStmtNode(Token token)
        {
            Debug.Assert(token.Parent.Name() == "ReturnStmt");
            Reduction rhs = (Reduction)token.Data;
            ExpressionListNode expressions = null;

            SourcePosition srcPos;

            Reduction data = rhs[1].Data as Reduction;
            if (data != null)
            {
                expressions = BuildExprList(rhs[1]);
                srcPos = BuildSourcePosition(rhs[0], expressions);
            }
            else
            {
                srcPos = BuildSourcePosition(rhs[0], rhs[1]);
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
            Reduction rhs = (Reduction)token.Data;

            string identifier = GetSpelling(rhs[0]);

            var srcPos = BuildSourcePosition(rhs[0], rhs[3]);

            FuncCallExprNode funcCallNode = new FuncCallExprNode(identifier, srcPos);

            Token actualParams = rhs[2];
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
            Reduction rhs = (Reduction)token.Data;
            
            if (rhs.Count() > 0)
            {
                Token exprToken = rhs.Count() == 2 ? rhs[0] : rhs[1];
                Token multiExprToken = rhs.Count() == 2 ? rhs[1] : rhs[2];
                
                ExpressionNode exprNode = BuildExprNode(exprToken);
                funcCallNode.Parameters.Add(exprNode);
                AppendFuncCallParameters(multiExprToken, funcCallNode);
            }
        }

        private AssignmentStmtNode BuildAssignStmt(Token token)
        {
            Debug.Assert(token.Parent.Name() == "AssignStmt");
            Reduction rhs = (Reduction)token.Data;

            string firstLhfName = rhs[0].Parent.Name();

            var assignToken = rhs[1].Data as TokenData;

            ExpressionListNode exprListNode = BuildExprList(rhs[2]);
            
            if ("Id".Equals(firstLhfName, StringComparison.InvariantCultureIgnoreCase))
            {
                var idListNode = BuildIdentifierList(rhs[0]);
                var srcPos = new SourcePosition(idListNode, exprListNode);
                return new AssignmentStmtNode(idListNode, exprListNode, srcPos);
            }

            var declNode = BuildDeclStmt(rhs[0]);
            var srcPos2 = new SourcePosition(declNode, exprListNode);
            return new DeclAndAssignmentStmtNode(
                declNode.Type, declNode.Identifiers, exprListNode,
                srcPos2);
        }
        
        private ExpressionListNode BuildExprList(Token token)
        {
            Debug.Assert(token.Parent.Name() == "ExprList");
            Reduction rhs = (Reduction)token.Data;

            var  list = new ExpressionListNode();

            list.Add(BuildExprNode(rhs[0]));

            Token multiToken = rhs[1];
            AppendExpression(multiToken, list);

            list.UpdateSourcePosition();

            return list;
        }

        private void AppendExpression(Token token, ExpressionListNode list)
        {
            Debug.Assert(token.Parent.Name() == "MultiExpr");
            Reduction rhs = (Reduction)token.Data;

            if (rhs.Count() > 0)
            {
                list.Add(BuildExprNode(rhs[1]));

                Token multiIdToken2 = rhs[2];
                AppendExpression(multiIdToken2, list);
            }
        }

        private IfStmtNode BuildIfStmt(Token ifStmToken)
        {
            Debug.Assert(ifStmToken.Parent.Name() == "IfStmt");
            Reduction rhs = (Reduction)ifStmToken.Data;
            Debug.Assert(rhs.Count() == 9);

            Token exprToken = rhs[1];
            Token stmtsToken = rhs[4];
            Token elseIfToken = rhs[5];
            Token elseToken = rhs[6];

            var srcPos = BuildSourcePosition(rhs[0], rhs[8]);

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
            Reduction rhs = (Reduction)token.Data;
            if (rhs.Count() > 0)
            {
                Reduction elseIfReduction = (Reduction) rhs[0].Data;

                Token exprToken = elseIfReduction[1];
                Token stmtsToken = elseIfReduction[4];
                
                ExpressionNode predicate = BuildExprNode(exprToken);
                StmtBlockNode stmtsNode = BuildStmtsBlock(stmtsToken);

                SourcePosition srcPos = BuildSourcePosition(elseIfReduction[0], stmtsNode);

                ElseIfStmtNode elseIfStmt = new ElseIfStmtNode(predicate, stmtsNode, srcPos);
                ifStmtNode.ElseIfStatements.Add(elseIfStmt);

                AppendElseIfStmts(rhs[1], ifStmtNode);
            }
        }

        private ExpressionNode BuildExprNode(Token exprToken)
        {
            string name = exprToken.Parent.Name();
            Reduction rhs = (Reduction)exprToken.Data;

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
            Reduction rhs = (Reduction) andToken.Data;

            if (name.Equals("Unary"))
                throw new ArgumentException("Unary not allowed here.");

            if (rhs.Count() == 3)
            {
                ExpressionNode leftOperand = BuildExprNode(rhs[0]);
                BinaryOperatorType operatorType = ParseOperator(GetSpelling(rhs[1]));
                ExpressionNode rightOperand = BuildExprNode(rhs[2]);

                var srcPos = new SourcePosition(leftOperand, rightOperand);

                return new BinaryOperationNode(leftOperand, operatorType, rightOperand, srcPos);
            }

            return BuildExprNode(rhs[0]);
        }

        private ExpressionNode BuildUnaryExprNode(Token unaryToken)
        {
            Debug.Assert(unaryToken.Parent.Name() == "Unary");
            Reduction rhs = (Reduction)unaryToken.Data;

            if (rhs.Count() == 2)
            {
                string operatorCode = GetSpelling(rhs[0]).ToLower();
                ExpressionNode unaryExprNode = BuildUnaryExprNode(rhs[1]);
                SourcePosition srcPos = BuildSourcePosition(rhs[0], unaryExprNode);

                return new UnaryOperationNode(
                    ParseUnaryOperator(operatorCode),
                    unaryExprNode,
                    srcPos);
            }

            return BuildValueNode(rhs[0]);
        }

        private ExpressionNode BuildValueNode(Token valueToken)
        {
            Debug.Assert(valueToken.Parent.Name() == "Value");
            Reduction rhs = (Reduction)valueToken.Data;

            string rhsName = rhs[0].Parent.Name();

            switch (rhsName)
            {
                case "Literal":
                    return BuildLiteralValueNode(rhs[0]);
                case "Id":
                    string name = GetSpelling(rhs[0]);
                    SourcePosition srcPos2 = BuildSourcePosition(rhs[0], rhs[0]);
                    return new IdentifierNode(name, srcPos2);
                case "FuncCall":
                    return BuildFuncCallExprNode(rhs[0]);
                case "(":
                    return BuildExprNode(rhs[1]);
                default:
                    throw new ArgumentException("Unexpected value node.");
            }
        }

        private LiteralValueNode BuildLiteralValueNode(Token token)
        {
            Debug.Assert(token.Parent.Name() == "Literal");
            Reduction rhs = (Reduction)token.Data;

            string value = GetSpelling(rhs[0]);
            string literalType = rhs[0].Parent.Name();
            PrimitiveTypeNode happyType;

            if (literalType == "NumberLiteral")
            {
                happyType = new PrimitiveTypeNode(PrimitiveType.Number);
            }
            else if (literalType == "TextLiteral")
            {
                happyType = new PrimitiveTypeNode(PrimitiveType.Text);
                value = value.Substring(1, value.Length - 2);
            }
            else if (literalType == "BooleanLiteral")
            {
                happyType = new PrimitiveTypeNode(PrimitiveType.Boolean);
            }
            else
            {
                throw new InvalidOperationException("Invalid literal type.");
            }
                
            SourcePosition srcPos = BuildSourcePosition(rhs[0], rhs[0]);

            return new LiteralValueNode(value, happyType, srcPos);
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
            Reduction rhs = (Reduction)declStmtToken.Data;

            TypeNode type = BuildTypeNode(rhs[0]);
            IdentifierListNode idList = BuildIdentifierList(rhs[1]);
            
            SourcePosition srcPos = BuildSourcePosition(rhs[0], idList);

            return new DeclStmtNode(idList, type, srcPos);
        }

        private TypeNode BuildTypeNode(Token token)
        {
            string typeSpec = GetSpelling(token).ToLower();
            if ("number".Equals(typeSpec))
                return new PrimitiveTypeNode(PrimitiveType.Number);
            if ("boolean".Equals(typeSpec))
                return new PrimitiveTypeNode(PrimitiveType.Boolean);
            if ("text".Equals(typeSpec))
                return new PrimitiveTypeNode(PrimitiveType.Text);

            throw new InvalidOperationException("Invalid type found: " + typeSpec);
        }

        private IdentifierListNode BuildIdentifierList(Token idToken)
        {
            Debug.Assert(idToken.Parent.Name() == "Id");
            Reduction rhs = (Reduction)idToken.Data;

            var listNode = new IdentifierListNode();

            listNode.Add(BuildIdentifierNode(rhs[0]));
            
            Token multiIdentToken = rhs[1];
            AppendIdentifier(multiIdentToken, listNode);
            
            listNode.UpdateSourcePosition();

            return listNode;
        }

        private IdentifierNode BuildIdentifierNode(Token token)
        {
            Debug.Assert(token.Parent.Name() == "Id");
            Reduction rhs = (Reduction)token.Data;

            var name = GetSpelling(rhs[0]);
            var srcPos = BuildSourcePosition(rhs[0], rhs[0]);

            return new IdentifierNode(name, srcPos);
        }

        private void AppendIdentifier(Token multiIdToken, IdentifierListNode list)
        {
            Debug.Assert(multiIdToken.Parent.Name() == "MultiId");
            Reduction rhs = (Reduction)multiIdToken.Data;

            if (rhs.Count() > 0)
            {
                var srcPos = BuildSourcePosition(rhs[1], rhs[1]);

                string name = GetSpelling(rhs[1]);
                list.Add(new IdentifierNode(name, srcPos));

                Token multiIdToken2 = rhs[2];
                AppendIdentifier(multiIdToken2, list);
            }
        }
        
        private void AppendFuncDecls(Token token, FuncDeclListNode list)
        {
            Debug.Assert(token.Parent.Name() == "FuncDecls");
            Reduction rhs = (Reduction)token.Data;

            if (rhs.Count() > 0)
            {
                list.Add(BuildFuncDeclNode(rhs[0]));

                if (rhs.Count() == 3)
                {
                    AppendFuncDecls(rhs[2], list);
                }
            }
        }

        private void AppendConstDecls(Token token, ConstDeclListNode list)
        {
            Debug.Assert(token.Parent.Name() == "ConstantDecls");
            Reduction rhs = (Reduction)token.Data;

            if (rhs.Count() > 0)
            {
                list.Add(BuildConstDeclNode(rhs[0]));
                AppendConstDecls(rhs[2], list);
            }
        }

        private ConstDeclNode BuildConstDeclNode(Token token)
        {
            Debug.Assert(token.Parent.Name() == "Constant");
            Reduction rhs = (Reduction)token.Data;

            string name = GetSpelling(rhs[2]);

            TypeNode typeNode;
            ValueNode valueNode;

            if (rhs[1].Parent.Name() == "PrimitiveTypes")
            {
                typeNode = BuildPrimitiveTypeNode(rhs[1]);
                valueNode = BuildLiteralValueNode(rhs[4]);
            }
            else
            {
                typeNode = BuildArrayTypeNode(rhs[1]);
                valueNode = BuildArrayValueNode(rhs[4], (ArrayTypeNode)typeNode);
            }
            
            var srcPos = BuildSourcePosition(rhs[0], rhs[0]);

            return new ConstDeclNode(name, typeNode, valueNode, srcPos);
        }

        private ArrayValueNode BuildArrayValueNode(Token token, ArrayTypeNode node)
        {
            Debug.Assert(token.Parent.Name() == "ArrayAssign");
            Reduction rhs = (Reduction)token.Data;

            var arrayValueNode = new ArrayValueNode(node);
            AppendVariableInitializer(rhs[1], arrayValueNode);

            return arrayValueNode;
        }

        private void AppendArrayValueNode(Token token, ArrayValueNode node)
        {
            Debug.Assert(token.Parent.Name() == "ArrayAssign");
            Reduction rhs = (Reduction)token.Data;

            AppendVariableInitializer(rhs[1], node);

        }

        private void AppendVariableInitializer(Token token, ArrayValueNode node)
        {
            Debug.Assert(token.Parent.Name() == "VarInitList");
            Reduction rhs = (Reduction) token.Data;
            var rhs2 = (Reduction) rhs[0].Data;

            if (rhs2[0].Parent.Name() == "LiteralInput")
            {
                var rhs3 = (Reduction) rhs2[0].Data;

                if (rhs3[0].Parent.Name() == "Id")
                {
                    var identifier = GetSpelling(rhs3[0]);
                    var sp = BuildSourcePosition(rhs3[0], rhs3[0]);
                    node.Values.Add(new IdentifierNode(identifier, sp));
                }
                else
                {
                    var val = BuildLiteralValueNode(rhs3[0]);
                    node.Values.Add(val);
                }
            }
            else
            {
                AppendArrayValueNode(rhs2[0], node);
            }
            if (rhs.Count() > 1)
            {
                AppendVariableInitializer(rhs[2], node);
            }
        }

        private FuncDeclNode BuildFuncDeclNode(Token token)
        {
            Debug.Assert(token.Parent.Name() == "FuncDecl");
            Reduction rhs = (Reduction)token.Data;

            string funcName = GetSpelling(rhs[1]);

            var srcPos = BuildSourcePosition(rhs[0], rhs[10]);

            StmtBlockNode funcBodyNode = BuildStmtsBlock(rhs[8]);
            FuncDeclNode funcDelc = new FuncDeclNode(funcName, funcBodyNode, srcPos);
            
            AppendFormalParameters(rhs[3], funcDelc);

            AppendReturnTypes(rhs[6], funcDelc);

            return funcDelc;
        }

        private ArrayTypeNode BuildArrayTypeNode(Token token)
        {
            Debug.Assert(token.Parent.Name() == "ArrayType");
            Reduction rhs = (Reduction)token.Data;

            SourcePosition srcPos = null;
            ExpressionListNode sizes = BuildArraySizes(rhs[2]);
            PrimitiveTypeNode typeNode = null;
            if (rhs.Count() == 8)
            {
                typeNode = BuildPrimitiveTypeNode(rhs[6]);
                srcPos = BuildSourcePosition(rhs[0], rhs[7]);
            }
            else
            {
                typeNode = BuildPrimitiveTypeNode(rhs[5]);
                srcPos = BuildSourcePosition(rhs[0], typeNode);
            }
            
            return new ArrayTypeNode(typeNode, sizes, srcPos);
        }

        private ExpressionListNode BuildArraySizes(Token token)
        {
            Debug.Assert(token.Parent.Name() == "ArraySize");

            var list = new ExpressionListNode();

            AppendArraySize(token, list);
            
            list.UpdateSourcePosition();

            return list;
        }
        
        private void AppendArraySize(Token token, ExpressionListNode list)
        {
            Debug.Assert(token.Parent.Name() == "ArraySize" || token.Parent.Name() == "SizeList");
            Reduction rhs = (Reduction)token.Data;

            if (rhs.Count() == 0)
                return;

            Token single = rhs.Count() == 2 ? rhs[0] : rhs[1];
            Token multi = rhs.Count() == 2 ? rhs[1] : rhs[2];
            var srcPos = BuildSourcePosition(single, single);

            if (single.Parent.Name() == "NumberLiteral")
            {
                var value = GetSpelling(single);
                var type = new PrimitiveTypeNode(PrimitiveType.Number);
                list.Add(new LiteralValueNode(value, type, srcPos));
            }
            else
            {
                var name = GetSpelling(single);
                list.Add(new IdentifierNode(name, srcPos));
            }

            AppendArraySize(multi, list);
        }

        private PrimitiveTypeNode BuildPrimitiveTypeNode(Token token)
        {
            Debug.Assert(token.Parent.Name() == "PrimitiveTypes");
            Reduction rhs = (Reduction)token.Data;

            string typeSpec = GetSpelling(rhs[0]).ToLower();
            var srcPos = BuildSourcePosition(rhs[0], rhs[0]);
            if ("number".Equals(typeSpec))
                return new PrimitiveTypeNode(PrimitiveType.Number, srcPos);
            if ("boolean".Equals(typeSpec))
                return new PrimitiveTypeNode(PrimitiveType.Boolean, srcPos);
            if ("text".Equals(typeSpec))
                return new PrimitiveTypeNode(PrimitiveType.Text, srcPos);

            throw new InvalidOperationException("Invalid type found: " + typeSpec);
        }

        private void AppendReturnTypes(Token token, FuncDeclNode funcDeclNode)
        {
            Debug.Assert(token.Parent.Name() == "ReturnTypes" || token.Parent.Name() == "MultiReturnTypes");
            Reduction rhs = (Reduction)token.Data;

            if (rhs.Count() > 1)
            {
                Token singleToken = rhs.Count() == 2 ? rhs[0] : rhs[1];
                Token multiToken = rhs.Count() == 2 ? rhs[1] : rhs[2];

                Reduction retReduction = (Reduction) singleToken.Data;
                TypeNode retType = BuildTypeNode(retReduction[0]);
                funcDeclNode.ReturnTypes.Add(retType);

                AppendReturnTypes(multiToken, funcDeclNode);
            }
        }
        
        private void AppendFormalParameters(Token token, FuncDeclNode funcCallNode)
        {
            Debug.Assert(token.Parent.Name() == "FormalParams" || token.Parent.Name() == "MultiFormalParams");
            Reduction rhs = (Reduction)token.Data;

            if (rhs.Count() > 0)
            {
                Token singleToken = rhs.Count() == 2 ? rhs[0] : rhs[1];
                Token multiToken = rhs.Count() == 2 ? rhs[1] : rhs[2];

                Reduction formalParmReduction = (Reduction) singleToken.Data;
                TypeNode paramType = BuildTypeNode(formalParmReduction[0]);
                string paramName = GetSpelling(formalParmReduction[1]);

                SourcePosition srcPos = BuildSourcePosition(formalParmReduction[0], formalParmReduction[1]);

                FormalParamNode paramNode = new FormalParamNode(paramName, paramType, srcPos);
                funcCallNode.Parameters.Add(paramNode);

                AppendFormalParameters(multiToken, funcCallNode);
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