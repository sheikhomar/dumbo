﻿using System;
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
            Reduction lhs = (Reduction)stmtsToken.Data;

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
                case "AssignStmt":
                    return BuildAssignStmt(rhs[0]);
                case "IfStmt":
                    return BuildIfStmt(rhs[0]);
                case "RepeatStmt":
                    return BuildRepeatStmtNode(rhs[0]);
                case "PrimitiveDecl":
                    return BuildPrimitiveDeclStmt(rhs[0]);
                case "ReturnStmt":
                    return BuildReturnStmtNode(rhs[0]);
                case "FuncCall":
                    return BuildFuncCallStmt(rhs[0]);
                case "BreakStmt":
                    return BuildBreakStmtNode(rhs[0]);
                case "ArrayDecl":
                    return BuildArrayDeclStmt(rhs[0]);
                case "ContinueStmt":
                    return BuildContinueStmt(rhs[0]);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private StmtNode BuildContinueStmt(Token token)
        {
            Debug.Assert(token.Parent.Name() == "ContinueStmt");
            Reduction rhs = (Reduction)token.Data;

            
            var srcPos = BuildSourcePosition(rhs[0], rhs[0]);
            return new ContinueStmtNode(srcPos);
        }

        private ArrayDeclStmtNode BuildArrayDeclStmt(Token token)
        {
            Debug.Assert(token.Parent.Name() == "ArrayDecl");
            Reduction rhs = (Reduction)token.Data;

            var typeNode = BuildArrayTypeNode(rhs[0]);
            var ids = BuildIdentifierList(rhs[1]);

            return new ArrayDeclStmtNode(typeNode, ids);
        }

        private StmtNode BuildBreakStmtNode(Token token)
        {
            Debug.Assert(token.Parent.Name() == "BreakStmt");
            Reduction rhs = (Reduction)token.Data;

            //Reduction rhs2 = (Reduction)rhs[0].Data;
            var srcPos = BuildSourcePosition(rhs[0], rhs[0]);
            return new BreakStmtNode(srcPos);
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
            return ((TokenData)token.Data).Spelling;
        }

        private void AppendFuncCallParameters(Token token, FuncCallExprNode funcCallNode)
        {
            Debug.Assert(token.Parent.Name() == "ActualParams" || token.Parent.Name() == "ActualParamsList");
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

            var first = rhs[0].Parent.Name();
            
            switch (first)
            {
                case "Id":
                {
                    var id = BuildIdentifierList(rhs[0]);
                    var expr = BuildExprNode(rhs[2]);
                    var sp = new SourcePosition(id, expr);
                    return new AssignmentStmtNode(id, expr, sp);
                }
                case "PrimitiveDecl":
                {
                    var primDecl = BuildPrimitiveDeclStmt(rhs[0]);
                    var expr = BuildExprNode(rhs[2]);
                    var sp = new SourcePosition(primDecl, expr);
                    return new DeclAndAssignmentStmtNode(primDecl.Type, primDecl.Identifiers, expr, sp);
                }
                case "ArrayDecl":
                {
                    var declStmt = BuildArrayDeclStmt(rhs[0]);
                    var arrayVal = BuildArrayValueNode(rhs[2], declStmt.Type);
                    var sp = new SourcePosition(declStmt, arrayVal);
                    return new DeclAndAssignmentStmtNode(declStmt.Type, declStmt.Identifiers, arrayVal, sp);
                }
                case "ArrayId":
                {
                    var arrayIdentifierNode = BuildArrayIdentifierNode(rhs[0]);
                    var ids = new IdentifierListNode();
                    ids.Add(arrayIdentifierNode);
                    var expr = BuildExprNode(rhs[2]);
                    var sp = new SourcePosition(arrayIdentifierNode, expr);
                    return new AssignmentStmtNode(ids, expr, sp);
                }
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private ArrayIdentifierNode BuildArrayIdentifierNode(Token token)
        {
            Debug.Assert(token.Parent.Name() == "ArrayId");
            Reduction rhs = (Reduction)token.Data;

            var id = BuildIdentifierNode(rhs[0]);
            var rhs2 = (Reduction)rhs[1].Data;
            var sizes = BuildArraySizes(rhs2[1]);
            var sp = BuildSourcePosition(rhs[0], rhs2[2]);
            return new ArrayIdentifierNode(id.Name, sizes, sp);
        }

        private ExpressionListNode BuildExprList(Token token)
        {
            Debug.Assert(token.Parent.Name() == "ExprList");
            Reduction rhs = (Reduction)token.Data;

            var list = new ExpressionListNode();

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
            Reduction rhs = (Reduction) ifStmToken.Data;

            var srcPos = BuildSourcePosition(rhs[0], rhs[7]);

            var predicate = BuildExprNode(rhs[1]);
            var stmtsNode = BuildStmtsBlock(rhs[4]);

            StmtBlockNode elseStmtsNode = null;

            ElseIfStmtListNode list = new ElseIfStmtListNode();
            var elseIf = AppendElseIfStmts(rhs[5], list);
            var rhs3 = (Reduction) elseIf.Data;
            if (rhs3.Count() > 0)
            {
                elseStmtsNode = BuildStmtsBlock(rhs3[2]);
            }

            IfStmtNode ifNode = null;
            if (elseStmtsNode == null)
                ifNode = new IfStmtNode(predicate, stmtsNode, list, srcPos);
            else
                ifNode = new IfElseStmtNode(predicate, stmtsNode, elseStmtsNode, list, srcPos);

            return ifNode;
        }

        private Token AppendElseIfStmts(Token token, ElseIfStmtListNode list)
        {
            Debug.Assert(token.Parent.Name() == "ElseIfStmt");
            Reduction rhs = (Reduction)token.Data;
            if (rhs.Count() > 1)
            {
                var predicate = BuildExprNode(rhs[2]);
                var stmtsNode = BuildStmtsBlock(rhs[5]);
                var srcPos = BuildSourcePosition(rhs[0], stmtsNode);

                var elseIfStmt = new ElseIfStmtNode(predicate, stmtsNode, srcPos);
                list.Add(elseIfStmt);

                return AppendElseIfStmts(rhs[6], list);
            }

            return rhs[0];
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
                case "ArrayId":
                    return BuildArrayIdentifierNode(rhs[0]);
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

        private PrimitiveDeclStmtNode BuildPrimitiveDeclStmt(Token declStmtToken)
        {
            Debug.Assert(declStmtToken.Parent.Name() == "PrimitiveDecl");
            Reduction rhs = (Reduction)declStmtToken.Data;

            var type = BuildPrimitiveTypeNode(rhs[0]);
            var idList = BuildIdentifierList(rhs[1]);
            
            var srcPos = new SourcePosition(type, idList);


            return new PrimitiveDeclStmtNode(idList, type, srcPos);
        }

        private TypeNode BuildTypeNode(Token token)
        {
            Debug.Assert(token.Parent.Name() == "Type");
            Reduction rhs = (Reduction)token.Data;

            if (rhs[0].Parent.Name() == "PrimitiveTypes")
            {
                return BuildPrimitiveTypeNode(rhs[0]);
            }
            else
            {
                return BuildArrayTypeNode(rhs[0]);
            }
        }

        private IdentifierListNode BuildIdentifierList(Token idToken)
        {
            Debug.Assert(idToken.Parent.Name() == "Id");
            Reduction rhs = (Reduction)idToken.Data;

            var listNode = new IdentifierListNode();

            listNode.Add(BuildIdentifierNode(rhs[0]));
            
            Token idListToken = rhs[1];
            AppendIdentifier(idListToken, listNode);
            
            listNode.UpdateSourcePosition();

            return listNode;
        }

        private IdentifierNode BuildIdentifierNode(Token token)
        {
            Debug.Assert(token.Parent.Name() == "Id");

            var name = GetSpelling(token);
            var srcPos = BuildSourcePosition(token, token);

            return new IdentifierNode(name, srcPos);
        }

        private void AppendIdentifier(Token token, IdentifierListNode list)
        {
            Debug.Assert(token.Parent.Name() == "IdList");
            Reduction rhs = (Reduction)token.Data;

            if (rhs.Count() > 0)
            {
                list.Add(BuildIdentifierNode(rhs[1]));
                AppendIdentifier(rhs[2], list);
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

            var srcPos = BuildSourcePosition(rhs[0], rhs[2]);

            var arrayValueNode = new ArrayValueNode(node, srcPos);

            AppendArrayAssign(token, arrayValueNode);

            arrayValueNode.Values.UpdateSourcePosition();

            return arrayValueNode;
        }

        private void AppendArrayAssign(Token token, ArrayValueNode node)
        {
            Debug.Assert(token.Parent.Name() == "ArrayAssign");
            Reduction rhs = (Reduction)token.Data;

            node.CreateLevel();

            AppendVarInitList(rhs[1], node);

            node.CloseLevel();
        }

        private void AppendVarInitList(Token token, ArrayValueNode node)
        {
            Debug.Assert(token.Parent.Name() == "VarInitList");
            Reduction rhs = (Reduction)token.Data;

            if (rhs.Count() == 1)
            {
                AppendLiteralInput(rhs[0], node);
            }
            else
            {
                AppendArrayAssign(rhs[0], node);
                AppendArrayAssignList(rhs[1], node);
            }
        }

        private void AppendArrayAssignList(Token token, ArrayValueNode node)
        {
            Debug.Assert(token.Parent.Name() == "ArrayAssignList");
            Reduction rhs = (Reduction)token.Data;

            if (rhs.Count() != 0)
            {
                AppendArrayAssign(rhs[1], node);
                AppendArrayAssignList(rhs[2], node);
            }
        }

        private void AppendLiteralInput(Token token, ArrayValueNode node)
        {
            Debug.Assert(token.Parent.Name() == "LiteralInput");
            Reduction rhs = (Reduction)token.Data;

            if (rhs[0].Parent.Name() == "Literal")
            {
                node.AddExpression(BuildLiteralValueNode(rhs[0]));

                AppendLiteralInputList(rhs[1], node);
            }
            else
            {
                node.AddExpression(BuildIdentifierNode(rhs[0]));

                AppendLiteralInputList(rhs[1], node);
            }
        }

        private void AppendLiteralInputList(Token token, ArrayValueNode node)
        {
            Debug.Assert(token.Parent.Name() == "LiteralInputList");
            Reduction rhs = (Reduction)token.Data;

            if (rhs.Count() != 0)
            {
                AppendLiteralInput(rhs[1], node);
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
            Debug.Assert(token.Parent.Name() == "ReturnTypes" || token.Parent.Name() == "ReturnTypesList");
            Reduction rhs = (Reduction)token.Data;

            if (rhs.Count() == 2)
            {
                var type = BuildTypeNode(rhs[0]);
                funcDeclNode.ReturnTypes.Add(type);
                AppendReturnTypes(rhs[1], funcDeclNode);
            }
            else if (rhs.Count() == 3)
            {
                var type = BuildTypeNode(rhs[1]);
                funcDeclNode.ReturnTypes.Add(type);
                AppendReturnTypes(rhs[2], funcDeclNode);
            }
        }

        private void AppendFormalParameters(Token token, FuncDeclNode funcCallNode)
        {
            Debug.Assert(token.Parent.Name() == "FormalParams" || token.Parent.Name() == "FormalParamsList");
            Reduction rhs = (Reduction)token.Data;

            if (rhs.Count() > 0)
            {
                Token singleToken = rhs.Count() == 2 ? rhs[0] : rhs[1];
                Token multiToken = rhs.Count() == 2 ? rhs[1] : rhs[2];

                Reduction formalParmReduction = (Reduction)singleToken.Data;
                TypeNode paramType = BuildTypeNode(formalParmReduction[0]);
                string paramName = GetSpelling(formalParmReduction[1]);

                SourcePosition srcPos = BuildSourcePosition(paramType, formalParmReduction[1]);

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

        private SourcePosition BuildSourcePosition(BaseNode startNode, Token endToken)
        {
            var startSrcPos = new SourcePosition(startNode, startNode);
            var endSrcPos = BuildSourcePosition(endToken, endToken);

            int startLine = startSrcPos.StartLine;
            int startColumn = startSrcPos.StartColumn;
            int endLine = endSrcPos.EndLine;
            int endColumn = endSrcPos.EndColumn;
            return new SourcePosition(startLine, startColumn, endLine, endColumn);
        }
    }
}